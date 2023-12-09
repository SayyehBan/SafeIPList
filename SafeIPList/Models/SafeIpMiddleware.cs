using System.Net;

namespace SafeIPList.Models;

// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class SafeIpMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string ipAddressSafe;

    public SafeIpMiddleware(RequestDelegate next, string IpAddressSafe)
    {
        _next = next;
        ipAddressSafe = IpAddressSafe;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var userIP = httpContext.Connection.RemoteIpAddress;
        string[] safeIP = ipAddressSafe.Split(';');
        var userIPbytes = userIP.GetAddressBytes();
        bool IsBlock = true;
        foreach (var item in safeIP)
        {
            var tempIP = IPAddress.Parse(item);
            if (tempIP.GetAddressBytes().SequenceEqual(userIPbytes))
            {
                IsBlock = false;
                break;
            }
        }
        if (IsBlock == true)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }
        await _next(httpContext);
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class SafeIpMiddlewareExtensions
{
    public static IApplicationBuilder UseSafeIpMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SafeIpMiddleware>();
    }
}
