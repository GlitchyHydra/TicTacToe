using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controller
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IGameService GameService;
        private IUserService UserService;

        public LoginController(IGameService gameService,
            IUserService userService)
        {
            GameService = gameService;
            UserService = userService;
        }

        [HttpGet("/")]
        public string Index()
        {
            return "TicTacToe game main page";
        }

        [HttpPost("/login/{username?}")]
        public async Task<IActionResult> Login(string username)
        {
            if (await GameService.IsUserInGame(username))
            {
                return Ok(new { Token = "Invalid" });
            }
            
            var token = await UserService.GenerateNewToken(username);
            return Ok(new {Token = token});
        }
    }
}
