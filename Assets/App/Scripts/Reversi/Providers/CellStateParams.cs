using System;

namespace App.Reversi
{
    public struct CellStateParams
    {
        public int row { get; }
        public int col { get; }
        public CellState cellState { get; }

        public CellStateParams(int row, int col, CellState cellState)
        {
            this.row = row;
            this.col = col;
            this.cellState = cellState;
        }
    }
}