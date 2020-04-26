using Microsoft.AspNetCore.Http;

namespace Framework.Infrastructure.Interfaces.Core
{
    public static class ContextAccessorExtensions
    {
        public static string GetHeader(this IHttpContextAccessor httpContextAccessor, string key)
        {
            httpContextAccessor?.HttpContext?.Request.Headers.TryGetValue(key, out var value);
            return !string.IsNullOrWhiteSpace(value) ? value[0] : "";
        }
    }
}
