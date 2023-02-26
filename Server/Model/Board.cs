namespace Server
{
    public class Board
    {
        /// <summary>
        /// The number of pieces that have been placed on the board.
        /// </summary>
        private int totalPiecesPlaced;

        private const int RowMaxLength = 3;
        private const int ColumnMaxLength = 3;

        public Board()
        {
            Pieces = new string[RowMaxLength, ColumnMaxLength];
            totalPiecesPlaced = 0;
        }

        public int RowLength => Pieces.GetLength(0);
        public int ColumnLength => Pieces.GetLength(1);

        public string[,] Pieces { get; private set; }

        public bool IsThreeInRow
        {
            get
            {
                for (int row = 0; row < RowLength; row++)
                {
                    if (!string.IsNullOrWhiteSpace(Pieces[row, 0]) &&
                        Pieces[row, 0] == Pieces[row, 1] &&
                        Pieces[row, 1] == Pieces[row, 2])
                    {
                        return true;
                    }
                }
                
                for (int col = 0; col < ColumnLength; col++)
                {
                    if (!string.IsNullOrWhiteSpace(Pieces[0, col]) &&
                        Pieces[0, col] == Pieces[1, col] &&
                        Pieces[1, col] == Pieces[2, col])
                    {
                        return true;
                    }
                }

                // Check main diagonal
                if (!string.IsNullOrWhiteSpace(Pieces[1, 1]) &&
                    Pieces[2, 0] == Pieces[1, 1] &&
                    Pieces[1, 1] == Pieces[0, 2])
                {
                    return true;
                }

                // Check reverse diagonal
                if (!string.IsNullOrWhiteSpace(Pieces[1, 1]) &&
                    Pieces[0, 0] == Pieces[1, 1] &&
                    Pieces[1, 1] == Pieces[2, 2])
                {
                    return true;
                }

                return false;
            }
        }
        
        public bool IsNoSpaceLeft
        {
            get
            {
                return totalPiecesPlaced >= Pieces.Length;
            }
        }

        public void PlacePiece(int row, int col, string pieceToPlace)
        {
            Pieces[row, col] = pieceToPlace;
            totalPiecesPlaced++;
        }

        public override string ToString()
        {
            return string.Join(", ", this.Pieces);
        }
    }
}
