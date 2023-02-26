using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Server.Model.Db;
using Server.Services;

namespace Server.Authentication
{
    public interface IGameAuthenticationManager
    {
        string Authenticate(string username, string password);
    }

    public class GameAuthenticationScheme : AuthenticationScheme
    {
        public GameAuthenticationScheme(string name, string displayName, Type handlerType)
            : base(name, displayName, handlerType)
        {

        }
    }

    public class GameAuthenticationSchemeHandler : AuthenticationHandler<GameAuthenticationOptions>
    {
        private const string AccessTokenField = "Authorization";

        private readonly IUserService UserService;

        public GameAuthenticationSchemeHandler(IOptionsMonitor<GameAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService)
            : base(options, logger, encoder, clock)
        {
            UserService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AccessTokenField))
            {
                return AuthenticateResult.Fail("No token was provided");
            }

            var rawHead = Request.Headers[AccessTokenField];
            if (rawHead.Count <= 0 || rawHead[0] is null)
            {
                return AuthenticateResult.Fail("No token was provided");
            }

            var isStartWithBearer = rawHead[0].StartsWith("Bearer");
            var token = isStartWithBearer ? rawHead[0].Split(" ")[1] : rawHead[0];
            var isRolePresent = await UserService.IsUserPlayer(token);
            if (!isRolePresent)
            {
                return AuthenticateResult.Fail("You are not allowed to play");
            }

            IList<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Role, PlayerRole.Player.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
