
using System.Linq.Expressions;


namespace AdnTestingSystem.Core.Base
{
    public class PagingRequestModel<T>
    {
        public int PageIndex { get; set; } = PagedResult<T>.DefaultPageIndex;
        public int PageSize { get; set; } = PagedResult<T>.UpperPageSize;
        public Expression<Func<T, bool>>? Predicate { get; set; }

        public PagingRequestModel(int pageIndex = PagedResult<T>.DefaultPageIndex,
                                    int pageSize = PagedResult<T>.UpperPageSize,
                                    Expression<Func<T, bool>>? predicate = null)
        {
            PageIndex = pageIndex > 0 ? pageIndex : PagedResult<T>.DefaultPageIndex;
            PageSize = pageSize > 0
                ? (pageSize > PagedResult<T>.UpperPageSize ? PagedResult<T>.UpperPageSize : pageSize)
                : PagedResult<T>.DefaultPageSize;
            Predicate = predicate;
        }
    } 
}
