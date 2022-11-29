using System;
using VContainer;

namespace App.Reversi
{
    public struct CellStateParams
    {
        public StoneAction stoneAction { get; }
        public int row { get; }
        public int col { get; }
        public CellState cellState { get; }

        [Inject]
        public CellStateParams(StoneAction stoneAction, int row, int col, CellState cellState)
        {
            this.stoneAction = stoneAction;
            this.row = row;
            this.col = col;
            this.cellState = cellState;
        }
    }
}