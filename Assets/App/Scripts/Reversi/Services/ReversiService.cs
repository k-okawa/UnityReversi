using System.Collections.Generic;
using MessagePipe;
using UniRx;
using UnityEngine;
using VContainer;

namespace App.Reversi
{
    public class ReversiService
    {
       [Inject] private BoardModel _boardModel;
       [Inject] private IPublisher<CellStateParams> _cellStatePublisher;

       public IReadOnlyReactiveProperty<int> turnCount => _turnCount;
       public IReadOnlyReactiveProperty<int> blackStoneCount => _blackStoneCount;
       public IReadOnlyReactiveProperty<int> whiteStoneCount => _whiteStoneCount;
       public IReadOnlyReactiveProperty<bool> isGameOver => _isGameOver;
       public IReadOnlyReactiveProperty<CellState> currentTurnState => _currentTurnState;

       private IntReactiveProperty _turnCount { get; } = new();
       private IntReactiveProperty _blackStoneCount { get; } = new();
       private IntReactiveProperty _whiteStoneCount { get; } = new();
       private BoolReactiveProperty _isGameOver { get; } = new();
       private ReactiveProperty<CellState> _currentTurnState { get; } = new();

       public void ResetBoard()
        {
            for (int row = 0; row < BoardModel.RowCount; row++)
            {
                for (int col = 0; col < BoardModel.ColCount; col++)
                {
                    _boardModel.cells[row, col] = CellState.None;
                }
            }

            _turnCount.Value = 0;
            _blackStoneCount.Value = 0;
            _whiteStoneCount.Value = 0;
            _isGameOver.Value = false;
            _currentTurnState.Value = CellState.Black;

            SetCellState(3, 3, CellState.Black, true);
            SetCellState(3, 4, CellState.White, true);
            SetCellState(4, 3, CellState.White, true);
            SetCellState(4, 4, CellState.Black, true);
        }

        private void SetCellState(int row, int col, CellState cellState, bool isPut)
        {
            if (cellState == CellState.Black && _boardModel.cells[row, col] != CellState.Black)
            {
                _blackStoneCount.Value++;
                if (_boardModel.cells[row, col] != CellState.None)
                {
                    _whiteStoneCount.Value--;
                }
            }

            if (cellState == CellState.White && _boardModel.cells[row, col] != CellState.White)
            {
                _whiteStoneCount.Value++;
                if (_boardModel.cells[row, col] != CellState.None)
                {
                    _blackStoneCount.Value--;
                }
            }
            
            _boardModel.cells[row, col] = cellState;
            _cellStatePublisher?.Publish(new CellStateParams(isPut, row, col, cellState));
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
            if (isGameOver.Value)
            {
                Debug.LogWarning($"The game is already over");
                return;
            }
            var directions = GetReversibleDirections(row, col, _currentTurnState.Value);
            if (directions.Count <= 0)
            {
                Debug.Log($"Can't put stone. State:{_currentTurnState} row:{row} col:{col}");
                return;
            }
            
            _turnCount.Value++;
            SetCellState(row, col, _currentTurnState.Value, true);
            foreach (var dir in directions)
            {
                ReverseRecursively(row, col, dir, _currentTurnState.Value);
            }

            CheckGameState();
        }

        private void CheckGameState()
        {
            if (turnCount.Value >= BoardModel.RowCount * BoardModel.ColCount)
            {
                _isGameOver.Value = false;
                return;
            }

            CellState nextState = _currentTurnState.Value;
            switch (_currentTurnState.Value)
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
                _currentTurnState.Value = nextState;
                return;
            }

            if (GetPossibleToPutPositions(_currentTurnState.Value).Count <= 0)
            {
                _isGameOver.Value = true;
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
            
            SetCellState(row, col, setState, false);
            ReverseRecursively(row, col, direction, setState);
        }
    }
}