using SayyehBanTools.AccessDenied;
using SayyehBanTools.Converter;

namespace SafeIPList.Models
{
    public class IpRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedIps;

        public IpRestrictionMiddleware(RequestDelegate next)
        {
            _next = next;

            string filePath = "wwwroot/file/IpIran.txt";
            _allowedIps = File.ReadAllLines(filePath);
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress.ToString();
            if (_allowedIps.Any(allowedIp => IPAccess.IsInRange(StringExtensions.AnonymousIP(ipAddress), allowedIp.Split(',')[0], allowedIp.Split(',')[1])))
            {
                await _next(context);
            }
            else
            {
                // The IP address is not allowed.
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Access Denied " + ipAddress);
                return;
            }
        }

    }


    public static class IpRestrictionMiddlewareExtensions
    {
        public static IApplicationBuilder UseIpRestrictionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IpRestrictionMiddleware>();
        }
    }
}
