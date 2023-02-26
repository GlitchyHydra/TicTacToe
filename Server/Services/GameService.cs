using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Server.Services
{
    public class GameService : IGameService
    {
        public readonly ConcurrentDictionary<string, Player> PlayingPlayers =
                new ConcurrentDictionary<string, Player>();

        public readonly ConcurrentDictionary<string, GameInstance> Games =
            new ConcurrentDictionary<string, GameInstance>();

        public readonly ConcurrentQueue<Player> WaitingPlayers =
            new ConcurrentQueue<Player>();

        public readonly ConcurrentDictionary<string, string> UsernameToIdDictionary =
            new ConcurrentDictionary<string, string>();

        public void MapUsernameWithId(string username, string id)
        {
            UsernameToIdDictionary[username] = id;
        }

        public async Task<bool> IsUserInGame(string username)
        {
            var player = await Task.Run(async delegate
            {
                return PlayingPlayers
                    .FirstOrDefault(it => it.Value.Username == username)
                    .Value;
            });
            if (player == null)
                return false;
            return true;
        }

        public Player GeneratePlayer(string username, string connectionId)
        {
            var player = new Player(username, connectionId);
            PlayingPlayers[connectionId] = player;
            return player;
        }

        public void SetTimerDelegate(GameInstance game, ChangeTurnDelegate changeTurnDelegate)
        {
            game.ChangeTurnDelegate = changeTurnDelegate;
        }

        public void EnqueuePlayer(Player player)
            => WaitingPlayers.Enqueue(player);


        public GameInstance GenerateNewGame(Player player1, Player player2)
        {
            var game = new GameInstance(player1, player2);
            Games[game.Id] = game;
            return game;
        }

        public void RemoveGame(string id)
        {
            Games.TryRemove(id, out GameInstance? foundGame);
            if (foundGame is null)
                return;

            foundGame.timer.Dispose();
            PlayingPlayers.TryRemove(foundGame.Player1.Id, out Player? foundPlayer);
            PlayingPlayers.TryRemove(foundGame.Player2.Id, out foundPlayer);
        }

        public void SetReady(Player player)
        {
            player.IsReadyToPlay = true;
        }

        public bool IsAllReady(GameInstance game)
        {
            return (game.Player1.IsReadyToPlay && game.Player2.IsReadyToPlay);
        }

        public void StartMatch(GameInstance game)
        {
            game.IsGameStarted = true;
            game.timer.Change(15000, 15000);
        }
        
        /*--------------------------------------Getters----------------------------------------*/
        public Player GetPlayer(string playerId)
        {
            Player foundPlayer;
            if (!PlayingPlayers.TryGetValue(playerId, out foundPlayer))
            {
                return null;
            }

            return foundPlayer;
        }

        public Player? GetWaitingPlayer()
        {
            Player foundPlayer;
            while (!WaitingPlayers.IsEmpty)
            {
                if (!WaitingPlayers.TryDequeue(out foundPlayer))
                {
                    return null;
                }
                var id = UsernameToIdDictionary[foundPlayer.Username];
                if (foundPlayer.Id == id)
                    return foundPlayer;
            }

            return null;
        }

        public GameInstance GetGame(Player player, out Player opponent)
        {
            opponent = null;
            GameInstance foundGame = Games.Values
                .FirstOrDefault(g => g.Id == player.GameId);

            if (foundGame == null)
            {
                return null;
            }

            opponent = (player.Id == foundGame.Player1.Id) ?
                foundGame.Player2 :
                foundGame.Player1;

            return foundGame;
        }
    }
}
