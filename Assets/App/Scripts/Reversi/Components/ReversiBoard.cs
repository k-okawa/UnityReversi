using UnityEngine;

namespace App.Reversi
{
    public class ReversiBoard : MonoBehaviour
    {
        [SerializeField] private Transform _cellRoot;

        public Cell[,] cells { get; } = new Cell[8, 8];

        void Awake()
        {
            var cellObjects = _cellRoot.GetComponentsInChildren<Cell>();
            foreach (var cell in cellObjects)
            {
                cells[cell.row, cell.col] = cell;
            }
        }

        public void ResetBoard()
        {
            foreach (var cell in cells)
            {
                cell.RemoveStone();
            }
        }

        public void PutStone(int row, int col, CellState cellState)
        {
            var cell = cells[row, col];
            cell.PutStone(cellState);
        }

        public void ReverseStone(int row, int col, CellState cellState)
        {
            cells[row, col].stone.Set(cellState);
        }
    }
}