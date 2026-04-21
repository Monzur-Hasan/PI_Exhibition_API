using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Domain.Shared.Helpers
{
    public static class PaginationHeaderExtensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddPagination(this HttpResponse response, int currentPage, int itemPerPage, long totalItems, long totalPages)
        {
            var paginationHeader = new PaginationHeaderTotal(currentPage, itemPerPage, totalItems, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");

        }

        //public static void AddPagination(this HttpResponse response, int pageNumber, int pageSize, int totalRows, int totalPages)
        //{
        //    var paginationHeader = new
        //    {
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalRows = totalRows,
        //        TotalPages = totalPages
        //    };

        //    response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationHeader));
        //}

    }
}
