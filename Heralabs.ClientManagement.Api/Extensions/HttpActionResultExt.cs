using Heralabs.ClientManagement.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Heralabs.ClientManagement.Api.Extensions
{
    public static class HttpActionResultExt
    {
        public static IActionResult InternalServerError(this ControllerBase api, Exception ex)
        {
            return api.StatusCode((int)HttpStatusCode.InternalServerError, new ApiSimpleResponse
            {
                Result = HttpStatusCode.InternalServerError.ToString(),
                Message = ex.Message
            });
        }
    }
}
