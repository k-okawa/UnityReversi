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
        [Inject] private ReversiBoard _reversiBoard;

        void IStartable.Start()
        {
            _stonePutSubscriber.Subscribe(OnPutStone).AddTo(_reversiBoard.GetCancellationTokenOnDestroy());
            _reversiService.ResetBoard();
        }

        private void OnPutStone(CellStateParams param)
        {
            _reversiBoard.PutStone(param.row, param.col, param.cellState);
        }
    }
}