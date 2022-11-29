using Cysharp.Threading.Tasks;
using MessagePipe;
using UniRx;
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
        [Inject] private UiManager _uiManager;

        void IStartable.Start()
        {
            _stonePutSubscriber.Subscribe(OnCellChanged).AddTo(_reversiBoard.GetCancellationTokenOnDestroy());
            _boardInputSubscriber.Subscribe(OnPutStone);
            _reversiService.currentTurnState.Subscribe(_uiManager.SetCurrentTurnText).AddTo(_uiManager);
            _reversiService.isGameOver.Subscribe(isOver =>
            {
                if (isOver)
                {
                    _uiManager.SetResultText(_reversiService.blackStoneCount.Value, _reversiService.whiteStoneCount.Value);
                }
                else
                {
                    _uiManager.UnsetResultText();
                }
            }).AddTo(_uiManager);
            _uiManager.resetButton.onClick.AddListener(() =>
            {
                _reversiBoard.ResetBoard();
                _reversiService.ResetBoard();
            });
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