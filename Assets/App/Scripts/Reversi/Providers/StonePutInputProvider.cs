using System;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App.Reversi
{
    public class StonePutInputProvider : ITickable
    {
        [Inject] private ReversiService _reversiService;
        [Inject] private ReversiBoard _reversiBoard;

        private CellState _currentTurnState = CellState.Black;

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool breakFlag = false;
                for (int row = 0; row < BoardModel.RowCount; row++)
                {
                    for (int col = 0; col < BoardModel.ColCount; col++)
                    {
                        if (_reversiBoard.cells[row,col].collider.Raycast(ray, out _, Single.PositiveInfinity))
                        {
                            if (_reversiService.CanPutStone(row, col, _currentTurnState))
                            {
                                _reversiService.PutStone(row, col, _currentTurnState);
                                switch (_currentTurnState)
                                {
                                    case CellState.Black:
                                        _currentTurnState = CellState.White;
                                        break;
                                    case CellState.White:
                                        _currentTurnState = CellState.Black;
                                        break;
                                }
                            }

                            breakFlag = true;
                            break;
                        }
                    }

                    if (breakFlag) break;
                }
            }
        }
    }
}