namespace Server
{
    public interface IGameClient
    {
        Task PlayerJoined(string id);
        Task PlaceInWaitList();
        Task StartGame();
        Task NeedToStartAgain();
        Task NeedToWait();
        Task NotPlayerTurn();
        Task NotValidMove();
        Task PlaceShape(int row, int col, string shape);
        Task UpdateTurn();
        Task TieGame();
        Task GameOver(string winnerPlayerId);
        Task OponnentLeft();
    }
}
