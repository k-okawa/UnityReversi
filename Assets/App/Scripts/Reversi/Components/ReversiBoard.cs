using UnityEngine;

namespace App.Reversi
{
    public class ReversiBoard : MonoBehaviour
    {
        [SerializeField] private Transform _cellRoot;

        private Cell[,] _cells { get; } = new Cell[8, 8];

        void Awake()
        {
            var cellObjects = _cellRoot.GetComponentsInChildren<Cell>();
            foreach (var cell in cellObjects)
            {
                _cells[cell.row, cell.col] = cell;
            }
        }

        public void ResetBoard()
        {
            foreach (var cell in _cells)
            {
                cell.RemoveStone();
            }
        }

        public void PutStone(int row, int col, CellState cellState)
        {
            var cell = _cells[row, col];
            cell.PutStone(cellState);
        }
        
        public void RemoveStone(int row, int col)
        {
            var cell = _cells[row, col];
            cell.RemoveStone();
        }

        public void ReverseStone(int row, int col, CellState cellState)
        {
            _cells[row, col].stone.Set(cellState);
        }
    }
}