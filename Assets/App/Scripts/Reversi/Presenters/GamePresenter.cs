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
        [Inject] private UiManager _uiManager;

        void IStartable.Start()
        {
            _stonePutSubscriber.Subscribe(OnCellChanged).AddTo(_reversiBoard.GetCancellationTokenOnDestroy());
            _boardInputSubscriber.Subscribe(OnPutStone);
            _uiManager.resetButton.onClick.AddListener(OnReset);
            _uiManager.undoButton.onClick.AddListener(OnUndo);
            _uiManager.redoButton.onClick.AddListener(OnRedo);
            _reversiService.ResetBoard();
        }

        private void OnCellChanged(CellStateParams param)
        {
            switch (param.stoneAction)
            {
                case StoneAction.Put:
                    _reversiBoard.PutStone(param.row, param.col, param.cellState);
                    break;
                case StoneAction.Remove:
                    _reversiBoard.RemoveStone(param.row, param.col);
                    break;
                case StoneAction.Reverse:
                    _reversiBoard.ReverseStone(param.row, param.col, param.cellState);
                    break;
            }
        }

        private void UpdateBoard()
        {
            _uiManager.SetCurrentTurnText(_reversiService.currentTurnState);
            if (_reversiService.isGameOver)
            {
                _uiManager.SetResultText(_reversiService.blackStoneCount, _reversiService.whiteStoneCount);
            }
            else
            {
                _uiManager.UnsetResultText();
            }
        }

        private void OnPutStone(BoardInputParams param)
        {
            _reversiService.PutStone(param.row, param.col);
            UpdateBoard();
        }

        private void OnReset()
        {
            _reversiBoard.ResetBoard();
            _reversiService.ResetBoard();
            UpdateBoard();
        }

        private void OnUndo()
        {
            _reversiService.Undo();
            UpdateBoard();
        }

        private void OnRedo()
        {
            _reversiService.Redo();
            UpdateBoard();
        }
    }
}