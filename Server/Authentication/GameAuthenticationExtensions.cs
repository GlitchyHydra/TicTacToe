using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Server.Services;

namespace Server.Authentication
{
    public static class GameAuthenticationExtensions
    {
        public static AuthenticationBuilder AddGameAuthentication<TAuthService>(this AuthenticationBuilder builder)
            where TAuthService : class, IUserService
        {
            return AddGameAuthentication<TAuthService>(builder, GameAuthenticationDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddGameAuthentication<TAuthService>(this AuthenticationBuilder builder, string authenticationScheme)
            where TAuthService : class, IUserService
        {
            return AddGameAuthentication<TAuthService>(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddGameAuthentication<TAuthService>(this AuthenticationBuilder builder, Action<GameAuthenticationOptions> configureOptions)
            where TAuthService : class, IUserService
        {
            return AddGameAuthentication<TAuthService>(builder, GameAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddGameAuthentication<TAuthService>(this AuthenticationBuilder builder, string authenticationScheme, Action<GameAuthenticationOptions> configureOptions)
            where TAuthService : class, IUserService
        {
            builder.Services.AddSingleton<IPostConfigureOptions<GameAuthenticationOptions>, GameAuthenticationPostConfigureOptions>();
            builder.Services.AddTransient<IUserService, TAuthService>();

            return builder.AddScheme<GameAuthenticationOptions, GameAuthenticationSchemeHandler>(authenticationScheme, configureOptions);
        }
    }
}
