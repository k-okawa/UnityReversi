using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        async void IStartable.Start()
        {
            _uiManager.SetButtonEnable(false);
            _stonePutSubscriber.Subscribe(OnCellChanged).AddTo(_reversiBoard.GetCancellationTokenOnDestroy());
            _boardInputSubscriber.Subscribe(OnPutStone);
            _uiManager.resetButton.onClick.AddListener(OnReset);
            _uiManager.undoButton.onClick.AddListener(OnUndo);
            _uiManager.redoButton.onClick.AddListener(OnRedo);
            _reversiService.ResetBoard();
            await PlayQueuedBoardAnimation(_reversiBoard.GetCancellationTokenOnDestroy());
            _uiManager.SetButtonEnable(true);
            SetHint();
        }

        private Queue<CellStateParams> _cellStateQueue = new();
        private void OnCellChanged(CellStateParams param)
        {
            _cellStateQueue.Enqueue(param);
        }

        private bool _isPlayingAnimation = false;
        private async UniTask PlayQueuedBoardAnimation(CancellationToken ct)
        {
            _isPlayingAnimation = true;
            List<UniTask> tasks = new List<UniTask>(_cellStateQueue.Count);
            int delayCounter = 0;
            while (_cellStateQueue.TryDequeue(out CellStateParams param))
            {
                switch (param.stoneAction)
                {
                    case StoneAction.Put:
                        tasks.Add(UniTask.Create(async () =>
                        {
                            await UniTask.Delay(TimeSpan.FromSeconds(0.05f * delayCounter++), cancellationToken: ct);
                            await _reversiBoard.PutStone(param.row, param.col, param.cellState);
                        }));
                        break;
                    case StoneAction.Remove:
                        tasks.Add(UniTask.Create(async () =>
                        {
                            await UniTask.Delay(TimeSpan.FromSeconds(0.05f * delayCounter++), cancellationToken: ct);
                            await _reversiBoard.RemoveStone(param.row, param.col);
                        }));
                        break;
                    case StoneAction.Reverse:
                        tasks.Add(UniTask.Create(async () =>
                        {
                            await UniTask.Delay(TimeSpan.FromSeconds(0.05f * delayCounter++), cancellationToken: ct);
                            await _reversiBoard.ReverseStone(param.row, param.col, param.cellState);
                        }));
                        break;
                }
            }
            await UniTask.WhenAll(tasks);
            _isPlayingAnimation = false;
        }

        private void UpdateUi()
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

        private void SetHint()
        {
            var hints = _reversiService.GetPossibleToPutPositions(_reversiService.currentTurnState);
            foreach (var hint in hints)
            {
                _reversiBoard.SetHint(hint.row, hint.col);
            }
        }

        private async void OnPutStone(BoardInputParams param)
        {
            if (_cellStateQueue.Any() || _isPlayingAnimation)
            {
                return;
            }
            _reversiBoard.SetAllHintOff();
            _uiManager.SetButtonEnable(false);
            _reversiService.PutStone(param.row, param.col);
            await PlayQueuedBoardAnimation(_reversiBoard.GetCancellationTokenOnDestroy());
            UpdateUi();
            _uiManager.SetButtonEnable(true);
            SetHint();
        }

        private async void OnReset()
        {
            _uiManager.SetButtonEnable(false);
            _reversiBoard.ResetBoard();
            _reversiService.ResetBoard();
            await PlayQueuedBoardAnimation(_reversiBoard.GetCancellationTokenOnDestroy());
            UpdateUi();
            _uiManager.SetButtonEnable(true);
            SetHint();
        }

        private async void OnUndo()
        {
            _reversiBoard.SetAllHintOff();
            _uiManager.SetButtonEnable(false);
            _reversiService.Undo();
            await PlayQueuedBoardAnimation(_reversiBoard.GetCancellationTokenOnDestroy());
            UpdateUi();
            _uiManager.SetButtonEnable(true);
            SetHint();
        }

        private async void OnRedo()
        {
            _reversiBoard.SetAllHintOff();
            _uiManager.SetButtonEnable(false);
            _reversiService.Redo();
            await PlayQueuedBoardAnimation(_reversiBoard.GetCancellationTokenOnDestroy());
            UpdateUi();
            _uiManager.SetButtonEnable(true);
            SetHint();
        }
    }
}