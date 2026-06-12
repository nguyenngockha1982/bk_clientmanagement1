using Heralabs.ClientManagement.Core.Helpers;
using Heralabs.ClientManagement.Core.Interfaces;
using Heralabs.ClientManagement.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace Heralabs.ClientManagement.Api.Filters
{
    public class HmacAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
    {   
        public bool UseCommonKey { get; set; } = false;

        // Đổi tên hàm theo interface mới (không còn tham số 'next')
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;

            // Bật Buffering NGAY LẬP TỨC ở đầu luồng
            request.EnableBuffering();

            // 1. Kiểm tra Headers
            if (!request.Headers.TryGetValue(SystemContants.HmacSignatureHeader, out var clientSignature) ||
                !request.Headers.TryGetValue(SystemContants.HmacTimestampHeader, out var timestampString))
            {
                context.Result = new UnauthorizedObjectResult("Missing HMAC headers.");
                return;
            }

            // 2. Chống Replay Attack (Tạm thời bạn có thể comment đoạn này khi debug nếu sợ gõ Postman chậm bị hết hạn 5 phút)
            if (!long.TryParse(timestampString, out var timestamp))
            {
                context.Result = new UnauthorizedObjectResult("Invalid timestamp format.");
                return;
            }

            var requestTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            if (Math.Abs((DateTime.UtcNow - requestTime).TotalMinutes) > SystemContants.HmacAllowedTimeDifferenceMinutes)
            {
                context.Result = new UnauthorizedObjectResult("Request expired.");
                return;
            }

            // ==========================================
            // 3. ĐỌC BODY VÀ DEBUG
            // ==========================================
            string requestBody = string.Empty;
            if (request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
            {
                request.Body.Position = 0; // Chắc chắn ở vị trí 0
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                request.Body.Position = 0; // Trả lại vị trí 0 cho Model Binding phía sau chạy
            }

            // 4. Lấy Secret Key
            string? secretKeyToUse = null;
            if (UseCommonKey)
            {
                var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                secretKeyToUse = configuration["HmacSecurity:CommonBootstrapKey"];
            }
            else
            {
                // Lấy ClientCode từ RouteData thay vì ActionArguments (vì AuthorizationFilter chạy trước ModelBinding)
                var clientCode = context.RouteData.Values["clientCode"]?.ToString();

                if (string.IsNullOrEmpty(clientCode))
                {
                    context.Result = new BadRequestObjectResult("Missing clientCode in route.");
                    return;
                }

                var cacheService = context.HttpContext.RequestServices.GetRequiredService<IClientCacheService>();
                var clientInfo = await cacheService.GetClientInfoAsync(clientCode);

                if (clientInfo == null || string.IsNullOrEmpty(clientInfo.ClientSecretKey))
                {
                    context.Result = new UnauthorizedObjectResult("Invalid client or missing secret key.");
                    return;
                }
                secretKeyToUse = clientInfo.ClientSecretKey;
            }

            // ==========================================
            // 5. TẠO PAYLOAD VÀ IN RA CONSOLE ĐỂ SO SÁNH
            // ==========================================
            var payload = $"{request.Method.ToUpper()}{request.Path}{timestamp}{requestBody}";
            string serverSignature = HmacHelper.GenerateSignature(request.Method.ToUpper(), request.Path, timestamp, requestBody, secretKeyToUse);

            // 6. So sánh
            if (serverSignature != clientSignature.ToString())
            {
                context.Result = new UnauthorizedObjectResult("Invalid HMAC signature.");
                return;
            }

            // Nếu mọi thứ OK, context.Result giữ nguyên là null, request sẽ đi tiếp vào Controller
        }
    }
}