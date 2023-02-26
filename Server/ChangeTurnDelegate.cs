using Microsoft.AspNetCore.SignalR;

namespace Server
{
    public class ChangeTurnDelegate
    {
        private IHubCallerClients<IGameClient> Clients;
        private string GameId;

        public ChangeTurnDelegate(IHubCallerClients<IGameClient> clients,
            string gameId)
        {
            Clients = clients;
            GameId = gameId;
        }

        public void Call()
        {
            Clients.Group(GameId).UpdateTurn();
        }
    }
}
