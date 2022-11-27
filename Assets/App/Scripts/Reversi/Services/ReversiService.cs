using System.Collections.Generic;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace App.Reversi
{
    public class ReversiService
    {
       [Inject] private BoardModel _boardModel;
       [Inject] private IPublisher<CellStateParams> _stonePutPublisher;
       
        public void ResetBoard()
        {
            for (int row = 0; row < BoardModel.RowCount; row++)
            {
                for (int col = 0; col < BoardModel.ColCount; col++)
                {
                    _boardModel.cells[row, col] = CellState.None;
                }
            }

            SetCellState(3, 3, CellState.Black, true);
            SetCellState(3, 4, CellState.White, true);
            SetCellState(4, 3, CellState.White, true);
            SetCellState(4, 4, CellState.Black, true);
        }

        private void SetCellState(int row, int col, CellState cellState, bool isPut)
        {
            _boardModel.cells[row, col] = cellState;
            _stonePutPublisher.Publish(new CellStateParams(isPut, row, col, cellState));
        }

        public CellState GetCellState(int row, int col)
        {
            return _boardModel.cells[row, col];
        }

        public bool CanPutStone(int row, int col, CellState cellState)
        {
            return GetReversibleDirections(row, col, cellState).Count > 0;
        }

        public void PutStone(int row, int col, CellState cellState)
        {
            var directions = GetReversibleDirections(row, col, cellState);
            if (directions.Count > 0)
            {
                SetCellState(row, col, cellState, true);
            }
            foreach (var dir in directions)
            {
                ReverseRecursively(row, col, dir, cellState);
            }
        }

        public List<Vector2Int> GetReversibleDirections(int row, int col, CellState cellState)
        {
            var ret = new List<Vector2Int>();
            if (cellState == CellState.None)
            {
                return ret;
            }

            if (_boardModel.cells[row, col] != CellState.None)
            {
                return ret;
            }
            
            if(CheckRecursively(row, col, Direction.Up, false, cellState)) ret.Add(Direction.Up);
            if(CheckRecursively(row, col, Direction.UpRight, false, cellState)) ret.Add(Direction.UpRight);
            if(CheckRecursively(row, col, Direction.Right, false, cellState)) ret.Add(Direction.Right);
            if(CheckRecursively(row, col, Direction.DownRight, false, cellState)) ret.Add(Direction.DownRight);
            if(CheckRecursively(row, col, Direction.Down, false, cellState)) ret.Add(Direction.Down);
            if(CheckRecursively(row, col, Direction.DownLeft, false, cellState)) ret.Add(Direction.DownLeft);
            if(CheckRecursively(row, col, Direction.Left, false, cellState)) ret.Add(Direction.Left);
            if(CheckRecursively(row, col, Direction.UpLeft, false, cellState)) ret.Add(Direction.UpLeft);

            return ret;
        }

        private bool CheckRecursively(int row, int col, Vector2Int direction, bool insertOnce, CellState checkCellState)
        {
            row += direction.y;
            col += direction.x;
            
            if (row < 0 || row >= BoardModel.RowCount)
            {
                return false;
            }

            if (col < 0 || col >= BoardModel.ColCount)
            {
                return false;
            }
            
            if (_boardModel.cells[row, col] == CellState.None)
            {
                return false;
            }
            
            if (insertOnce && _boardModel.cells[row, col] == checkCellState)
            {
                return true;
            }
            
            if (!insertOnce && _boardModel.cells[row, col] == checkCellState)
            {
                return false;
            }
            
            return CheckRecursively(row, col, direction, true, checkCellState);
        }

        private void ReverseRecursively(int row, int col, Vector2Int direction, CellState setState)
        {
            row += direction.y;
            col += direction.x;

            if (_boardModel.cells[row, col] == setState)
            {
                return;
            }
            
            SetCellState(row, col, setState, false);
            ReverseRecursively(row, col, direction, setState);
        }
    }
}