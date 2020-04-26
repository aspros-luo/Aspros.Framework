using System.Collections.Generic;

namespace Framework.Common.Core
{
    public class PagingResult<T>
    {
        public PagingResult(IEnumerable<T> data, int totalCount, int pageSize)
        {
            Data = data;
            TotalCount = totalCount;
            PageSize = pageSize;
        }

        public IEnumerable<T> Data { get; }

        public int TotalCount { get; }

        public int PageSize { get; }

        public int TotalPage
        {
            get
            {
                if (TotalCount % PageSize > 0)
                {
                    return TotalCount / PageSize + 1;
                }

                return TotalCount / PageSize;
            }
        }
    }
    public class PagingParams
    {
        public PagingParams(int pageNo, int pageSize = 10)
        {
            PageNo = pageNo < 0 ? 1 : pageNo;
            PageSize = pageSize;
        }

        public int PageNo { get; set; }

        public int PageSize { get; set; }

        public int Skip => (PageNo - 1) * PageSize;
    }
}
