using Cysharp.Threading.Tasks;
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
                if(cell.stone == null) continue;
                cell.stone.Set(CellState.None);
            }
            SetAllHintOff();
        }

        public async UniTask PutStone(int row, int col, CellState cellState)
        {
            var cell = _cells[row, col];
            await cell.PutStone(cellState);
        }
        
        public async UniTask RemoveStone(int row, int col)
        {
            var cell = _cells[row, col];
            await cell.RemoveStone();
        }

        public async UniTask ReverseStone(int row, int col, CellState cellState)
        {
            await _cells[row, col].stone.PlayReverseAnimation(cellState);
        }

        public void SetHint(int row, int col)
        {
            _cells[row, col].SetHint(true);
        }

        public void SetAllHintOff()
        {
            foreach (var cell in _cells)
            {
                cell.SetHint(false);
            }
        }
    }
}