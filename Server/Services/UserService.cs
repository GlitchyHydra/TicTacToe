using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Model.Db;

namespace Server.Services
{
    public class UserService : IUserService
    {
        private GameDbContext GameDbContext;

        public UserService(GameDbContext gameDbContext)
        {
            GameDbContext = gameDbContext;
        }

        public async Task<PlayerData?> GetUser(string username)
        {
            return await GameDbContext.Players
                .FirstOrDefaultAsync(it => it.Username == username);
        }

        private bool IsTokenExpired(string token)
        {
            var token_claims = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claim = token_claims.Claims.First(c => c.Type == "exp").Value;
            long unixDate = long.Parse(claim);
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime exp_date = start.AddMilliseconds(unixDate);
            var delta_time = DateTime.Now.Subtract(exp_date);
            double delta_in_days = delta_time.TotalDays;
            return delta_in_days < 7;
        }

        private int GetIdByToken(string token)
        {
            var token_claims = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claim = token_claims.Claims.First(c => c.Type == "Id").Value;
            return int.Parse(claim);
        }

        private PlayerRole GetRoleByToken(string token)
        {
            var token_claims = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claim = token_claims.Claims.First(c => c.Type == "Role").Value;
            return Enum.GetNames(typeof(PlayerRole)).Contains(claim) ? (PlayerRole)Enum.Parse(typeof(PlayerRole), claim, true) : PlayerRole.Spectator;
        }


        private void ParseToken(string token, out int id, out PlayerRole role)
        {
            bool isTokenReadable = new JwtSecurityTokenHandler().CanReadToken(token);
            if (!isTokenReadable)
            {
                role = PlayerRole.Spectator;
                id = -1;
                return;
            }

            if (IsTokenExpired(token))
            {
                role = PlayerRole.Spectator;
                id = -1;
                return;
            }

            role = GetRoleByToken(token);
            id = GetIdByToken(token);
        }

        public async Task<bool> IsUserPlayer(string token)
        {
            ParseToken(token, out int id, out PlayerRole role);
            var user = await GameDbContext.Players
                .FirstOrDefaultAsync(it => it.Id == id);
            if (user is null)
                return false;
            return user.Role == PlayerRole.Player;
        }

        public class AuthOptions
        {
            public const string ISSUER = "TiacTacToeGameServer";  
            public const string AUDIENCE = "TicTacToe_Players";          
            const string KEY = "TicTacToe_secretkey"; 
            public const int LIFETIME = 7;// token TTL (in days)
            public static SymmetricSecurityKey GetSymmetricSecurityKey()
            {
                return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
            }
        }

        private async Task<string> GenerateJwtToken(string username)
        {
            var user = await GetUser(username);
            if (user is null)
                return "Invalid";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = AuthOptions.ISSUER,
                Audience = AuthOptions.AUDIENCE,
                Subject = new ClaimsIdentity(new []
                {
                    new Claim("username", username),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Role", user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(AuthOptions.LIFETIME),
                SigningCredentials = new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<bool> WriteTokenToUser(string username, string token)
        {
            var user = await GameDbContext.Players
                .FirstOrDefaultAsync(it => it.Username == username);
            if (user is null)
                return false;

            user.Token = token;
            var res = await GameDbContext.SaveChangesAsync();
            return res != 0;
        }

        public async Task<string> GenerateNewToken(string username)
        {
            string token = await GenerateJwtToken(username);
            return await WriteTokenToUser(username, token) ? token : "Invalid";
        }
    }
}
