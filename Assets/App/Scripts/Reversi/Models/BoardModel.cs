namespace App.Reversi
{
    public class BoardModel
    {
        public const int RowCount = 8;
        public const int ColCount = 8;

        public CellState[,] cells = new CellState[RowCount, ColCount];
        
        public int turnCount;
        public bool isGameOver;
        public CellState currentTurnState;

        public int GetBlackStoneCount()
        {
            int ret = 0;
            foreach (var cell in cells)
            {
                if (cell == CellState.Black) ret++;
            }

            return ret;
        }
        
        public int GetWhiteStoneCount()
        {
            int ret = 0;
            foreach (var cell in cells)
            {
                if (cell == CellState.White) ret++;
            }

            return ret;
        }
    }
}
