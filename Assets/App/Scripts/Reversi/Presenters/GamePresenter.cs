using Cysharp.Threading.Tasks;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace App.Reversi
{
    public class GamePresenter : IStartable
    {
        [Inject] private ReversiService _reversiService;
        [Inject] private ISubscriber<CellStateParams> _stonePutSubscriber;
        [Inject] private ISubscriber<BoardInputParams> _boardInputSubscriber;
        [Inject] private ReversiBoard _reversiBoard;

        void IStartable.Start()
        {
            _stonePutSubscriber.Subscribe(OnCellChanged).AddTo(_reversiBoard.GetCancellationTokenOnDestroy());
            _boardInputSubscriber.Subscribe(OnPutStone);
            _reversiService.ResetBoard();
        }

        private void OnCellChanged(CellStateParams param)
        {
            if (param.isPut)
            {
                _reversiBoard.PutStone(param.row, param.col, param.cellState);
            }
            else
            {
                _reversiBoard.ReverseStone(param.row, param.col, param.cellState);
            }
        }

        private void OnPutStone(BoardInputParams param)
        {
            _reversiService.PutStone(param.row, param.col);
        }
    }
}