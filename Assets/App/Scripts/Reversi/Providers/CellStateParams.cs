using System;
using VContainer;

namespace App.Reversi
{
    public struct CellStateParams
    {
        public bool isPut { get; }
        public int row { get; }
        public int col { get; }
        public CellState cellState { get; }

        [Inject]
        public CellStateParams(bool isPut, int row, int col, CellState cellState)
        {
            this.isPut = isPut;
            this.row = row;
            this.col = col;
            this.cellState = cellState;
        }
    }
}