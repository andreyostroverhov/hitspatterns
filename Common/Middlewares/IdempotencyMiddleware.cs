using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Common.Middlewares;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDistributedCache _cache;

    public IdempotencyMiddleware(RequestDelegate next, IDistributedCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        //Проверяем является ли запрос POST, тк только он не идепотентен по спецификации http
        if (context.Request.Method != HttpMethod.Post.Method)
        {
            await _next(context);
            return;
        }

        //Достаем ключ
        var idempotencyKey = context.Request.Headers["Idempotency-Key"].FirstOrDefault();
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Idempotency-Key header is required");
            return;
        }

        // Проверяем, есть ли сохранённый ответ для этого ключа
        var cachedResponse = await _cache.GetStringAsync(idempotencyKey);
        if (cachedResponse != null)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(cachedResponse);
            return;
        }

        // Если ключа нет, выполняем запрос и сохраняем ответ
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        // Сохраняем ответ только при успешном выполнении
        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            var response = Encoding.UTF8.GetString(responseBody.ToArray());
            await _cache.SetStringAsync(idempotencyKey, response, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            });
        }

        await responseBody.CopyToAsync(originalBodyStream);
    }
}

public static class IdempotencyMiddlewareExtensions
{
    public static IApplicationBuilder UseIdempotencyMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<IdempotencyMiddleware>();
    }
}