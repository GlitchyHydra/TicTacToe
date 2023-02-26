using Microsoft.AspNetCore.SignalR;
using System.Web;
using System;
using Microsoft.AspNetCore.Authorization;
using Server.Model.Db;
using Server.Services;

namespace Server
{
    

    [Authorize(Roles = PlayerRole)]
    public class GameHub : Hub<IGameClient>
    {
        const string PlayerRole = "Player";

        private IGameService GameService;
        private IMatchStatisticsService MatchStatisticsService;

        public GameHub(IGameService gameService, IMatchStatisticsService matchStatisticsService)
        {
            GameService = gameService;
            MatchStatisticsService = matchStatisticsService;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public async Task SearchGame(string username)
        {
            Player joinedPlayer = GameService.GeneratePlayer(username, Context.ConnectionId);
            GameService.MapUsernameWithId(username, Context.ConnectionId);
            await Clients.Caller.PlayerJoined(joinedPlayer.Id);

            var waitingPlayer = GameService.GetWaitingPlayer();
            if (waitingPlayer is null)
            {
                GameService.EnqueuePlayer(joinedPlayer);
                await Clients.Caller.PlaceInWaitList();
                return;
            }
            
            GameInstance gameInstance = GameService.GenerateNewGame(waitingPlayer, joinedPlayer);
            var changeTurnDelegate = new ChangeTurnDelegate(Clients, gameInstance.Id);
            await Groups.AddToGroupAsync(waitingPlayer.Id, gameInstance.Id);
            await Groups.AddToGroupAsync(joinedPlayer.Id, gameInstance.Id);
            GameService.SetTimerDelegate(gameInstance, changeTurnDelegate);
        }
        
        public async Task Play(string playerId)
        {
            var playerHitPlay = GameService.GetPlayer(Context.ConnectionId);
            if (playerHitPlay is null)
                return;

            GameService.SetReady(playerHitPlay);
            var game = GameService.GetGame(playerHitPlay, out Player waitingPlayer);
            
            //no opponent present
            if (game is null)
            {
                await Clients.Caller.NeedToWait();
                return;
            }

            if (GameService.IsAllReady(game))
            {
                await Clients.Groups(game.Id).StartGame();
                GameService.StartMatch(game);
            }
        }

        public async Task TakeTurn(int row, int col)
        {
            var playerMakingTurn = GameService.GetPlayer(Context.ConnectionId);
            if (playerMakingTurn is null)
            {
                await Clients.Caller.NeedToStartAgain();
                return;
            }
            
            var game = GameService.GetGame(playerMakingTurn, out Player opponent);

            if (game is null)
            {
                await Clients.Caller.NeedToWait();
                return;
            }

            if (!game.IsGameStarted)
            {
                await Clients.Caller.NeedToWait();
                return;
            }

            if (!game.PlayerByTurn.Equals(playerMakingTurn))
            {
                await Clients.Caller.NotPlayerTurn();
                return;
            }

            if (!game.IsMoveValid(row, col))
            {
                await Clients.Caller.NotValidMove();
                return;
            }
            
            game.MakeMove(row, col);
            await Clients.Group(game.Id).PlaceShape(row, col, playerMakingTurn.Shape);
            
            if (!game.IsGameOver)
            {
                game.timer.Change(15000, 15000);
                await Clients.Group(game.Id).UpdateTurn();
                return;
            }

            MatchResult matchResult;
            if (game.IsTie && !game.IsSomeoneWin)
            {
                await Clients.Group(game.Id).TieGame();
                matchResult = MatchResult.Tie;
            }
            else
            {
                await Clients.Group(game.Id).GameOver(playerMakingTurn.Id);
                matchResult = playerMakingTurn.Id == game.Player1.Id
                    ? MatchResult.FirstPlayerWin
                    : MatchResult.SecondPlayerWin;
            }

            await MatchStatisticsService.SaveMatchStats(game.Player1.Username,
                game.Player2.Username, matchResult, game.History);

            GameService.RemoveGame(game.Id);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var leavingPlayer = GameService.GetPlayer(Context.ConnectionId);
            
            if (leavingPlayer is null)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }
            
            var ongoingGame = GameService.GetGame(leavingPlayer, out Player opponent);
            if (ongoingGame is not null)
            {
                await Clients.Group(ongoingGame.Id).OponnentLeft();
                GameService.RemoveGame(ongoingGame.Id);
            }

            await base.OnDisconnectedAsync(exception);
        }
        
    }
}
