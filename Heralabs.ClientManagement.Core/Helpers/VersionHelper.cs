namespace Heralabs.ClientManagement.Core.Helpers
{
    public static class VersionHelper
    {
        public static long CalculateVersionOrder(string? version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return 0;

            var parts = version.Split('.');

            long x = parts.Length > 0 && long.TryParse(parts[0], out var parsedX) ? parsedX : 0;
            long y = parts.Length > 1 && long.TryParse(parts[1], out var parsedY) ? parsedY : 0;
            long z = parts.Length > 2 && long.TryParse(parts[2], out var parsedZ) ? parsedZ : 0;

            return (x * 10000) + (y * 100) + z;
        }
    }
}
