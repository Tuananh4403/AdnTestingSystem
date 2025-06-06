using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AdnTestingSystem.Core.Base
{
    public  class PagedResult<T>
    {
        // PHAN TRANG
        public const int UpperPageSize = 100; // size maxximum
        public const int DefaultPageSize = 10; // size default 
        public const int DefaultPageIndex = 1; // default page index 
       
        
        /// <summary>
        /// This function returns a paged result after filtering.
        /// </summary>
        /// <param name="items">List of items after filtering.</param>
        /// <param name="pageIndex">Current page index.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="totalCount">Total number of items.</param>
        private PagedResult(List<T> items, int pageIndex, int pageSize, int totalCount)
        {
            Items = items;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
        public List<T> Items { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public bool HasNextPage => PageIndex * PageSize < TotalCount;
        public bool HasPreviousPage => PageIndex > 1;

        public static async Task<PagedResult<T>> CreateAsync(IQueryable<T> query, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex <= 0 ? DefaultPageIndex : pageIndex;
            pageSize = pageSize <= 0
                ? DefaultPageSize
                : pageSize > UpperPageSize
                ? UpperPageSize : pageSize;

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new(items, pageIndex, pageSize, totalCount);
        }

        public static PagedResult<T> Create(List<T> items, int pageIndex, int pageSize, int totalCount)
            => new(items, pageIndex, pageSize, totalCount);
    }
}
