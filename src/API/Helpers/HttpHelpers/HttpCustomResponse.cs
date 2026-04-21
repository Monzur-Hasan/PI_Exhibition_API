using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Helpers.HttpHelpers
{
    public class HttpCustomResponse : IActionResult
    {
        public HttpRequest Request { get; }

        public HttpStatusCode Code { get; }

        public ApiResponse Response { get; }

        public HttpCustomResponse(HttpRequest request)
            : this(request, HttpStatusCode.OK)
        {
        }

        public HttpCustomResponse(HttpRequest request, HttpStatusCode code)
            : this(request, code, null)
        {
        }

        public HttpCustomResponse(HttpRequest request, HttpStatusCode code, ApiResponse response)
        {
            Request = request;
            Code = code;
            Response = response;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            ObjectResult obg = new ObjectResult(Response)
            {
                StatusCode = (int)Code
            };
            await obg.ExecuteResultAsync(context);
        }
    }
}
