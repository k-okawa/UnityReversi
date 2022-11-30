using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Reversi
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _currentTurnText;
        
        [SerializeField] private CanvasGroup _resultRoot;
        [SerializeField] private TMP_Text _resultText;
        [SerializeField] private TMP_Text _blackCountText;
        [SerializeField] private TMP_Text _whiteCountText;

        [SerializeField] private Button _resetButton;
        public Button resetButton => _resetButton;
        [SerializeField] private Button _undoButton;
        public Button undoButton => _undoButton;
        [SerializeField] private Button _redoButton;
        public Button redoButton => _redoButton;

        void Start()
        {
            _resultRoot.alpha = 0f;
            _resultRoot.gameObject.SetActive(false);
        }

        public void SetCurrentTurnText(CellState currentState)
        {
            switch (currentState)
            {
                case CellState.Black:
                    _currentTurnText.text = "Turn:Black";
                    break;
                case CellState.White:
                    _currentTurnText.text = "Turn:White";
                    break;
                default:
                    _currentTurnText.text = string.Empty;
                    break;
            }
        }

        public void SetResultText(int blackCount, int whiteCount)
        {
            if (blackCount == whiteCount)
            {
                _resultText.text = "Draw";
            }

            if (blackCount < whiteCount)
            {
                _resultText.text = "White Wins";
            }

            if (blackCount > whiteCount)
            {
                _resultText.text = "Black Wins";
            }

            _blackCountText.text = $"Black:{blackCount:D2}";
            _whiteCountText.text = $"White:{whiteCount:D2}";
            
            _resultRoot.gameObject.SetActive(true);
            _resultRoot.DOFade(1f, 0.5f);
        }

        public void UnsetResultText()
        {
            _resultRoot.alpha = 0f;
            _resultRoot.gameObject.SetActive(false);
        }
    }
}