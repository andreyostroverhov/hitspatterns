using Common.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace Common.Other;

public class PagedList<T>
{
    public List<T> Items { get; private set; }
    public Pagination Pagination { get; set; } = new Pagination();


    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        Pagination.TotalCount = count;
        Pagination.PageSize = pageSize;
        Pagination.CurrentPage = pageNumber;
        Pagination.HasPrevious = pageNumber > 1;
        Pagination.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Pagination.HasNext = pageNumber < (int)Math.Ceiling(count / (double)pageSize);
        Items = items;
    }

    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
