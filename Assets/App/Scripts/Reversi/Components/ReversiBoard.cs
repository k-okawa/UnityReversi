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
                int row = int.Parse(cell.name.Substring(4, 1));
                int col = int.Parse(cell.name.Substring(5, 1));
                cells[row, col] = cell;
            }
        }

        public void PutStone(int row, int col, CellState cellState)
        {
            var cell = cells[row, col];
            cell.PutStone(cellState);
        }
    }
}