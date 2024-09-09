using System.Text.Json.Serialization;

namespace BookStoreService.DtoModels {
    /// <summary>
    /// Page info for list data request
    /// </summary>
    public class PaginationFilter {
        /// <summary>
        /// the page requested.
        /// </summary>
        [JsonPropertyName("page")]
        public uint Page { get; set; } = 1;
        /// <summary>
        /// Page size
        /// </summary>
        [JsonPropertyName("page_size")]
        public uint PageSize { get; set; }
        /// <summary>
        /// Sort by
        /// </summary>
        [JsonPropertyName("sort_by")]
        public string? SortBy { get; set; }
        /// <summary>
        /// Ascend or descend
        /// </summary>
        [JsonPropertyName("is_ascend")]
        public bool? IsAscend { get; set; }

        /// <summary>
        /// helper function, for the Linq Skip function.
        /// </summary>
        /// <param name="defaultPageSize"></param>
        /// <returns></returns>
        public int GetSkipCount(uint defaultPageSize = 20) {

            if (this.PageSize == 0) this.PageSize = defaultPageSize;
            return (int)((this.Page - 1) * this.PageSize);
        }

        /// <summary>
        /// helper function, for get the field for sorting
        /// </summary>
        /// <param name="defaultSortBy"></param>
        /// <returns></returns>
        public string GetSortBy(string defaultSortBy) {

            if (this.SortBy == null) this.SortBy = defaultSortBy;
            return this.SortBy;
        }

        /// <summary>
        /// Get the page size in Int type.
        /// </summary>
        /// <returns></returns>
        public int GetPageSize() {
            return (int)this.PageSize;
        }

        /// <summary>
        /// Ascend or descend
        /// </summary>
        /// <param name="defaultAscend"></param>
        /// <returns></returns>
        public bool GetIsAscend(bool defaultAscend) {
            if (this.IsAscend == null) this.IsAscend = defaultAscend;
            return this.IsAscend.Value;
        }
    }
}
