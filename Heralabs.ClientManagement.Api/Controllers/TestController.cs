using Heralabs.ClientManagement.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Heralabs.ClientManagement.Api.Controllers
{
    [ApiController]
    [Route("api/Test")]
    public class TestController : ControllerBase
    {
        [HttpGet(Name = "Status")]
        public async Task<IActionResult> Get()
        {
            string buildDate = string.Empty;
            string version = string.Empty;
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                buildDate = assembly
                    .GetCustomAttributes<AssemblyMetadataAttribute>()
                    .FirstOrDefault(a => a.Key == "BuildDate")?.Value;
                version = assembly.GetName().Version?.ToString();
            }
            catch
            {
            }
            return Ok($"Heralabs Client Management API is running.\r\n- Version: {version}\r\n- Build Date: {buildDate}");
        }

        [HttpGet("/random-hmac-key")]
        public async Task<IActionResult> GetRandomHmacKey()
        {
            return Ok($"Random HMAC Key: {HmacHelper.GenerateSecretKey()}");
        }
    }
}
