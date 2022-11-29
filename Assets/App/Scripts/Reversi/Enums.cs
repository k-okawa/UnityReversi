using UnityEngine;

namespace App.Reversi
{
    public enum CellState
    {
        None,
        Black,
        White
    }

    public enum StoneAction
    {
        Put,
        Remove,
        Reverse
    }

    public static class EnumExtensions
    {
        public static CellState GetReversed(this CellState cellState)
        {
            switch (cellState)
            {
                case CellState.Black:
                    return CellState.White;
                case CellState.White:
                    return CellState.Black;
            }

            return CellState.None;
        }
    }
}