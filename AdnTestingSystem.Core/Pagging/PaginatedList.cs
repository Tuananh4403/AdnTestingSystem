using Microsoft.EntityFrameworkCore;

namespace AdnTestingSystem.Contract.Repositories.PaggingItems;

public class PaginatedList<T>
{
    public int TotalRecords { get; private set; }
    public int Page { get; private set; }  
    public int PageSize { get; private set; }  
    public int TotalPages { get; private set; }  
    public List<T> Data { get; private set; }

    public bool HasPreviousPage => Page > 1;         
    public bool HasNextPage => Page < TotalPages;  

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalRecords = count;
        Page = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize < 1 ? 10 : pageSize;
        TotalPages = (int)Math.Ceiling((double)count / pageSize);
        Data = items;
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    public static PaginatedList<T> Create(List<T> source, int pageNumber, int pageSize)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalCount = source.Count;
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedList<T>(items, totalCount, pageNumber, pageSize);
    }
}
