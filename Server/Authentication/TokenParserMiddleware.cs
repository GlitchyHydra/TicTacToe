using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Server.Authentication
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TokenParserMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenParserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;

            // web sockets cannot pass headers so we must take the access token from query param and
            // add it to the header before authentication middleware runs
            if (request.Path.StartsWithSegments("/game", StringComparison.OrdinalIgnoreCase))
            {
                if (request.Query.TryGetValue("access_token", out var accessToken))
                {
                    request.Headers.Add("Authorization", $"{accessToken}");
                }
            }
            
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class UseTokenParserExtensions
    {
        public static IApplicationBuilder UseTokenParserMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenParserMiddleware>();
        }
    }
}
