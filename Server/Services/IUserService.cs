using Server.Model.Db;

namespace Server.Services
{
    public interface IUserService
    {
        Task<bool> IsUserPlayer(string username);
        Task<string> GenerateNewToken(string username);
    }
}
