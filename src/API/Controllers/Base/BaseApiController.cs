using API.Helpers.HttpHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult CustomResult(
            string message,
            HttpStatusCode code = HttpStatusCode.OK)
        {
            var response = new ApiResponse(
                message,
                success: IsSuccess(code)
            )
            {
                StatusCode = (int)code
            };

            return new HttpCustomResponse(Request, code, response);
        }

        protected IActionResult CustomResult(
            object data,
            HttpStatusCode code = HttpStatusCode.OK)
        {
            var response = new ApiResponse(
                message: string.Empty,
                data: data,
                success: IsSuccess(code)
            )
            {
                StatusCode = (int)code
            };

            return new HttpCustomResponse(Request, code, response);
        }

        protected IActionResult CustomResult(
            string message,
            object data,
            HttpStatusCode code = HttpStatusCode.OK)
        {
            var response = new ApiResponse(
                message,
                data,
                success: IsSuccess(code)
            )
            {
                StatusCode = (int)code
            };

            return new HttpCustomResponse(Request, code, response);
        }

        protected IActionResult CustomResult(
            HttpStatusCode code = HttpStatusCode.OK)
        {
            var response = new ApiResponse(
                string.Empty,
                success: IsSuccess(code)
            )
            {
                StatusCode = (int)code
            };

            return new HttpCustomResponse(Request, code, response);
        }

        private static bool IsSuccess(HttpStatusCode code)
        {
            return (int)code >= 200 && (int)code < 300;
        }
    }
}
