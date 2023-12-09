using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace SafeIPList.Models
{
    public class IpRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedIps;

        public IpRestrictionMiddleware(RequestDelegate next)
        {
            _next = next;

            // Read the list of allowed IPs from the file
            //string filePath = "wwwroot/file/IpIran.txt";
            //_allowedIps = File.ReadAllLines(filePath);

            //for (int i = 0; i < _allowedIps.Length; i++)
            //{
            //    // Hash the IP address
            //    _allowedIps[i] = GetHashForIp(_allowedIps[i]);
            //}

            // اینجا لیست مجاز ip ها را از فایل می‌خوانیم
            string filePath = "wwwroot/file/IpIran.txt";
            _allowedIps = File.ReadAllLines(filePath);
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress;

            // اینجا آدرس آی‌پی را به رشته تبدیل می‌کنیم تا بتوانیم بخش‌هایش را مقایسه کنیم
            string[] ipParts = ipAddress.ToString().Split('.');

            // اگر دو بخش اول از آدرس ip موجود در لیست مجاز باشد، اجازه می‌دهیم
            if (_allowedIps.Any(allowedIp => allowedIp.StartsWith(ipParts[0] + "." + ipParts[1])))
            {
                await _next(context);
            }
            else
            {
                // اگر آدرس مجاز نبود، پیامی برمی‌گردانیم
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Access Denied " + ipAddress.ToString());
                return;
            }
        }

        private string GetHashForIp(string ip)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(ip));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
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
