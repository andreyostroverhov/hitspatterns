using Microsoft.EntityFrameworkCore;
using Common.Monitoring;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MonitoringDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("MonitoringDatabase"),
        b => b.MigrationsAssembly("Common") 
    ));


builder.Services.AddHostedService<MonitoringEventConsumer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<MonitoringDbContext>();
    dbContext.Database.Migrate();
}

app.UseRouting();


app.UseSwagger();
app.UseSwaggerUI();


app.UseStaticFiles();

app.UseEndpoints(endpoints => {

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllers();
});

app.Run();