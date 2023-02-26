using Microsoft.Extensions.Options;

namespace Server.Authentication
{
    public class GameAuthenticationPostConfigureOptions : IPostConfigureOptions<GameAuthenticationOptions>
    {
        public void PostConfigure(string? name, GameAuthenticationOptions options)
        {
            if (string.IsNullOrEmpty(options.Realm))
            {
                throw new InvalidOperationException("Realm must be provided in options");
            }
        }
    }
}
