namespace App.Reversi
{
    public class CellStateHistoryModel
    {
        public int row { get; }
        public int col { get; }
        public StoneAction stoneAction { get; }
        public CellState cellState { get; }

        public CellStateHistoryModel(int row, int col, StoneAction stoneAction, CellState cellState)
        {
            this.row = row;
            this.col = col;
            this.stoneAction = stoneAction;
            this.cellState = cellState;
        }
    }
}