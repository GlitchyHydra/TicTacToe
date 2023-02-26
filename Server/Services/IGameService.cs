using Microsoft.AspNetCore.SignalR;

namespace Server
{
    /// <summary>
    /// Service for generic session game
    /// </summary>
    public interface IGameService
    {
        public void MapUsernameWithId(string username, string id);
        public Task<bool> IsUserInGame(string username);
        public Player GeneratePlayer(string username, string connectionId);
        public void SetTimerDelegate(GameInstance game, ChangeTurnDelegate changeTurnDelegate);
        public void EnqueuePlayer(Player player);
        public GameInstance GenerateNewGame(Player player1, Player player2);
        public void RemoveGame(string id);
        public void SetReady(Player player);
        public bool IsAllReady(GameInstance game);
        public void StartMatch(GameInstance game);
        public Player? GetPlayer(string playerId);
        public Player? GetWaitingPlayer();
        public GameInstance? GetGame(Player player, out Player opponent);
    }
}
