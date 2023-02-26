using System.Data.SqlTypes;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Server
{
    public class GameInstance
    {
        private bool Turn { get; set; }
        public string Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        private Board Board { get; set; }
        public List<string> History { get; private set; }
        public Timer timer;
        public ChangeTurnDelegate ChangeTurnDelegate { get; set; }
        public bool IsGameStarted { get; set; }
        public Player PlayerByTurn => Turn ? Player1 : Player2;
        public bool IsSomeoneWin => Board.IsThreeInRow;
        public bool IsGameOver => Board.IsNoSpaceLeft || Board.IsThreeInRow;
        public bool IsTie => Board.IsNoSpaceLeft;

        public void EndTimerDelegate(Object? obj)
        {
            if (ChangeTurnDelegate is null)
            {
                return;
            }

            Turn = !Turn;
            ChangeTurnDelegate.Call();
        }

        public GameInstance(Player p1, Player p2)
        {
            Id = Guid.NewGuid().ToString("d");
            Board = new Board();
            History = new List<string>(10);
            Turn = true;
            var autoEvent = new AutoResetEvent(false);
            timer = new Timer(EndTimerDelegate);

            Player1 = p1;
            Player2 = p2;
            Player1.GameId = Id;
            Player2.GameId = Id;
            Player1.Shape = "X";
            Player2.Shape = "0";

            History.Add($"{Player1.Username}: X, {Player2.Username}: 0\n");
        }

        public void MakeMove(int row, int col)
        {
            Board.PlacePiece(row, col, PlayerByTurn.Shape);
            History.Add($"({row},{col}):{PlayerByTurn.Shape}\n");
            Turn = !Turn;
        }

        public bool IsMoveValid(int row, int col)
        {
            bool isRowValid = row > -1 && row < Board.RowLength;
            bool isColValid = col > -1 && col < Board.ColumnLength;
            return isRowValid && isColValid && string.IsNullOrWhiteSpace(Board.Pieces[row, col]);
        }
    }
}
