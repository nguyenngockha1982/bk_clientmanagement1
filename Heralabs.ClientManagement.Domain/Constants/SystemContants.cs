using System;

namespace Heralabs.ClientManagement.Domain.Constants
{
    public class SystemContants
    {
        public const string ClientCacheKeyTemplate = "ClientInfo_{0}";
        public const int ClientCacheTimeOutDays = 7;
        public const string WebApiVersionCacheTemplate = "WebApiVer_{0}";
        public const string AllMobileVersionsCacheKey = "AllActiveMobileVersions";
        public const int AppVersionCacheTimeOutDays = 7;
        public const string HmacSignatureHeader = "X-Hmac-Signature";
        public const string HmacTimestampHeader = "X-Timestamp";
        public const int HmacAllowedTimeDifferenceMinutes = 5;
    }
}
