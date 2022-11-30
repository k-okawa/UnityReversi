using System;
using System.Collections.Generic;
using System.Linq;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace App.Reversi
{
    public class ReversiService
    {
        [Inject] private IPublisher<CellStateParams> _cellStatePublisher;

        private BoardModel _boardModel;
        private Stack<BoardHistoryModel> _boardHistoryModels = new();

        public int turnCount => _boardModel.turnCount;
        public int blackStoneCount => _boardModel.GetBlackStoneCount();
        public int whiteStoneCount => _boardModel.GetWhiteStoneCount();
        public bool isGameOver => _boardModel.isGameOver;
        public CellState currentTurnState => _boardModel.currentTurnState;

        public void ResetBoard()
        {
            _boardModel = new BoardModel();
            _boardModel.turnCount = 0;
            _boardModel.isGameOver = false;
            _boardModel.currentTurnState = CellState.Black;

            SetCellState(3, 3, CellState.Black, StoneAction.Put);
            SetCellState(3, 4, CellState.White, StoneAction.Put);
            SetCellState(4, 3, CellState.White, StoneAction.Put);
            SetCellState(4, 4, CellState.Black, StoneAction.Put);

            _boardHistoryModels.Clear();
            _boardHistoryModels.Push(new BoardHistoryModel(_boardModel.Clone(), Array.Empty<CellStateHistoryModel>()));
        }

        private void SetCellState(int row, int col, CellState cellState, StoneAction stoneAction)
        {
            _boardModel.cells[row, col] = cellState;
            _cellStatePublisher?.Publish(new CellStateParams(stoneAction, row, col, cellState));
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

            List<CellStateHistoryModel> cellStateHistoryModels = new List<CellStateHistoryModel>();
            var clonedBoardModel = _boardModel.Clone();

            _boardModel.turnCount++;
            SetCellState(row, col, _boardModel.currentTurnState, StoneAction.Put);
            cellStateHistoryModels.Add(new CellStateHistoryModel(row, col, StoneAction.Put,
                _boardModel.currentTurnState));
            foreach (var dir in directions)
            {
                var reverseStones = GetReverseStones(row, col, dir, _boardModel.currentTurnState);
                foreach (var history in reverseStones)
                {
                    SetCellState(history.row, history.col, history.cellState, StoneAction.Reverse);
                }

                cellStateHistoryModels.AddRange(reverseStones);
            }

            CheckGameState();

            _boardHistoryModels.Push(new BoardHistoryModel(clonedBoardModel, cellStateHistoryModels.ToArray()));
        }

        public void Undo()
        {
            if (_boardHistoryModels.Count <= 1)
            {
                Debug.Log("Can't undo");
                return;
            }

            var boardHistory = _boardHistoryModels.Pop();
            var reversedHistories = boardHistory.history.Reverse();
            foreach (var history in reversedHistories)
            {
                switch (history.stoneAction)
                {
                    case StoneAction.Put:
                        SetCellState(history.row, history.col, CellState.None, StoneAction.Remove);
                        break;
                    case StoneAction.Reverse:
                        SetCellState(history.row, history.col, history.cellState.GetReversed(), StoneAction.Reverse);
                        break;
                }
            }

            _boardModel = boardHistory.boardModel;
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

            if (CheckRecursively(row, col, Direction.Up, false, cellState)) ret.Add(Direction.Up);
            if (CheckRecursively(row, col, Direction.UpRight, false, cellState)) ret.Add(Direction.UpRight);
            if (CheckRecursively(row, col, Direction.Right, false, cellState)) ret.Add(Direction.Right);
            if (CheckRecursively(row, col, Direction.DownRight, false, cellState)) ret.Add(Direction.DownRight);
            if (CheckRecursively(row, col, Direction.Down, false, cellState)) ret.Add(Direction.Down);
            if (CheckRecursively(row, col, Direction.DownLeft, false, cellState)) ret.Add(Direction.DownLeft);
            if (CheckRecursively(row, col, Direction.Left, false, cellState)) ret.Add(Direction.Left);
            if (CheckRecursively(row, col, Direction.UpLeft, false, cellState)) ret.Add(Direction.UpLeft);

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

        private List<CellStateHistoryModel> GetReverseStones(int row, int col, Vector2Int direction, CellState setState)
        {
            var ret = new List<CellStateHistoryModel>();
            GetReverseStonesRecursively(row, col, direction, setState, ret);
            return ret;
        }

        private void GetReverseStonesRecursively(int row, int col, Vector2Int direction, CellState setState,
            List<CellStateHistoryModel> list)
        {
            row += direction.y;
            col += direction.x;

            if (_boardModel.cells[row, col] == setState)
            {
                return;
            }

            list.Add(new CellStateHistoryModel(row, col, StoneAction.Reverse, setState));
            GetReverseStonesRecursively(row, col, direction, setState, list);
        }
    }
}