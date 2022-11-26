using MessagePipe;
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

            SetCellState(3, 3, CellState.Black);
            SetCellState(3, 4, CellState.White);
            SetCellState(4, 3, CellState.White);
            SetCellState(4, 4, CellState.Black);
        }

        public void SetCellState(int row, int col, CellState cellState)
        {
            _boardModel.cells[row, col] = cellState;
            _stonePutPublisher.Publish(new CellStateParams(row, col, cellState));
        }

        public CellState GetCellState(int row, int col)
        {
            return _boardModel.cells[row, col];
        }
    }
}