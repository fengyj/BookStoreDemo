namespace BookStoreService.DtoModels {
    /// <summary>
    /// The data container object of pagination.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginationResult<T> {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pageInfo"></param>
        public PaginationResult(T data, PaginationInfo pageInfo) {
            this.Data = data;
            this.PageInfo = pageInfo;
        }

        /// <summary>
        /// The data of current page.
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// Page info
        /// </summary>
        public PaginationInfo PageInfo { get; set; }
    }

    /// <summary>
    /// Information of pagination.
    /// </summary>
    public class PaginationInfo {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy"></param>
        /// <param name="isAscend"></param>
        /// <param name="pageCount"></param>
        public PaginationInfo(uint page, uint pageSize, string sortBy, bool isAscend = true, uint? pageCount = null) {
            this.Page = page;
            this.PageSize = pageSize;
            this.PageCount = pageCount;
            this.SortBy = sortBy;
            this.IsAscend = isAscend;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalRecordCount"></param>
        public PaginationInfo(PaginationFilter filter, int? totalRecordCount = null)
            : this(filter.Page, filter.PageSize, filter.SortBy ?? string.Empty, filter.IsAscend!.Value, null) {

            this.PageCount = this.CalcPageCount(totalRecordCount);
        }

        /// <summary>
        /// Current page
        /// </summary>
        public uint Page { get; set; }
        /// <summary>
        /// Page size
        /// </summary>
        public uint PageSize { get; set; }
        /// <summary>
        /// Page count
        /// </summary>
        public uint? PageCount { get; set; }
        /// <summary>
        /// The field for sorting
        /// </summary>
        public string SortBy { get; set; }
        /// <summary>
        /// Ascend or descend.
        /// </summary>
        public bool IsAscend { get; set; }

        private uint? CalcPageCount(int? totalRecordCount) {
            if (totalRecordCount == null || totalRecordCount < 0) return null;
            var count = totalRecordCount.Value / this.PageSize;
            if (totalRecordCount.Value % this.PageSize != 0) count++;
            return (uint)count;
        }
    }
}
