using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Server.Model.Db;


namespace Server.Services
{
    public enum MatchResult
    {
        FirstPlayerWin = 0,
        SecondPlayerWin = 1,
        Tie = 2
    }

    public interface IMatchStatisticsService
    {
        public Task SaveMatchStats(string player1Username, string player2Username,
            MatchResult result, IList<string> matchHistory);

    }

    public class MatchStatisticsService : IMatchStatisticsService
    {
        private readonly GameDbContext GameDbContext;

        public MatchStatisticsService(GameDbContext gameDbContext)
        {
            GameDbContext = gameDbContext;
        }

        public async Task SaveMatchStats(string player1Username, string player2Username,
            MatchResult result, IList<string> matchHistory)
        {
            var playerData = await GameDbContext.Players
                .FirstOrDefaultAsync(i => i.Username == player1Username);
            if (playerData is null)
            {
                return;
            }

            var id1 = playerData.Id;

            playerData = await GameDbContext.Players
                .FirstOrDefaultAsync(i => i.Username == player2Username);
            if (playerData is null)
            {
                return;
            }

            var id2 = playerData.Id;
            var history = String.Join("", matchHistory.ToArray());
            var match = new GameData
            {
                History = history
            };

            var gameData = await GameDbContext.Games.AddAsync(match);
            if (gameData is null)
                return;
            await GameDbContext.SaveChangesAsync();
            string resultForFirstPlayer;
            string resultForSecondPlayer;
            if (result == MatchResult.Tie)
            {
                resultForFirstPlayer = "Tie";
                resultForSecondPlayer = "Tie";
            }
            else
            {
                resultForFirstPlayer = result == MatchResult.FirstPlayerWin ? "Win" : "Lose";
                resultForSecondPlayer = result == MatchResult.SecondPlayerWin ? "Win" : "Lose";
            }

            await GameDbContext.Statistics.AddAsync(new Stats
            {
                Result = resultForFirstPlayer,
                GameId = gameData.Entity.Id,
                PlayerId = id1
            });
            await GameDbContext.Statistics.AddAsync(new Stats
            {
                Result = resultForSecondPlayer,
                GameId = gameData.Entity.Id,
                PlayerId = id2
            });
            await GameDbContext.SaveChangesAsync();
        }
    }
}
