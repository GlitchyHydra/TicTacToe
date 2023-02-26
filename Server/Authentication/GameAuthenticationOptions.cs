using Microsoft.AspNetCore.Authentication;

namespace Server.Authentication
{
    public class GameAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; }
    }
}
