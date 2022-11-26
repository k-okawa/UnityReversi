namespace App.Reversi
{
    public class BoardModel
    {
        public const int RowCount = 8;
        public const int ColCount = 8;

        public CellState[,] cells = new CellState[RowCount, ColCount];
    }
}
