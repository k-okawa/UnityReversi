using System.Collections.Generic;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace App.Reversi
{
    public class ReversiService
    {
       [Inject] private BoardModel _boardModel;
       [Inject] private IPublisher<CellStateParams> _cellStatePublisher;
       
       public void ResetBoard()
        {
            for (int row = 0; row < BoardModel.RowCount; row++)
            {
                for (int col = 0; col < BoardModel.ColCount; col++)
                {
                    _boardModel.cells[row, col] = CellState.None;
                }
            }

            _boardModel.turnCount = 0;
            _boardModel.isGameOver = false;
            _boardModel.currentTurnState = CellState.Black;

            SetCellState(3, 3, CellState.Black, StoneAction.Put);
            SetCellState(3, 4, CellState.White, StoneAction.Put);
            SetCellState(4, 3, CellState.White, StoneAction.Put);
            SetCellState(4, 4, CellState.Black, StoneAction.Put);
        }

       private void SetCellState(int row, int col, CellState cellState, StoneAction stoneAction)
        {
            _boardModel.cells[row, col] = cellState;
            _cellStatePublisher?.Publish(new CellStateParams(stoneAction, row, col, cellState));
        }

        public CellState GetCellState(int row, int col)
        {
            return _boardModel.cells[row, col];
        }

        public bool CanPutStone(int row, int col, CellState cellState)
        {
            return GetReversibleDirections(row, col, cellState).Count > 0;
        }

        public void PutStone(int row, int col)
        {
            if (_boardModel.isGameOver)
            {
                Debug.LogWarning($"The game is already over");
                return;
            }
            var directions = GetReversibleDirections(row, col, _boardModel.currentTurnState);
            if (directions.Count <= 0)
            {
                Debug.Log($"Can't put stone. State:{_boardModel.currentTurnState} row:{row} col:{col}");
                return;
            }
            
            _boardModel.turnCount++;
            SetCellState(row, col, _boardModel.currentTurnState, StoneAction.Put);
            foreach (var dir in directions)
            {
                ReverseRecursively(row, col, dir, _boardModel.currentTurnState);
            }

            CheckGameState();
        }

        private void CheckGameState()
        {
            if (_boardModel.turnCount >= BoardModel.RowCount * BoardModel.ColCount)
            {
                _boardModel.isGameOver = false;
                return;
            }

            CellState nextState = _boardModel.currentTurnState;
            switch (_boardModel.currentTurnState)
            {
                case CellState.Black:
                    nextState = CellState.White;
                    break;
                case CellState.White:
                    nextState = CellState.Black;
                    break;
            }

            if (GetPossibleToPutPositions(nextState).Count > 0)
            {
                _boardModel.currentTurnState = nextState;
                return;
            }

            if (GetPossibleToPutPositions(_boardModel.currentTurnState).Count <= 0)
            {
                _boardModel.isGameOver = true;
            }
        }

        public List<(int row, int col)> GetPossibleToPutPositions(CellState cellState)
        {
            var ret = new List<(int row, int col)>();
            if (cellState == CellState.None)
            {
                return ret;
            }

            for (int row = 0; row < BoardModel.RowCount; row++)
            {
                for (int col = 0; col < BoardModel.ColCount; col++)
                {
                    if (_boardModel.cells[row, col] != CellState.None)
                    {
                        continue;
                    }

                    if (CanPutStone(row, col, cellState))
                    {
                        ret.Add((row, col));
                    }
                }
            }

            return ret;
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
            
            SetCellState(row, col, setState, StoneAction.Reverse);
            ReverseRecursively(row, col, direction, setState);
        }
    }
}