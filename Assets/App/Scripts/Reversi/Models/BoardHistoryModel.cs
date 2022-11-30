using System.Collections.Generic;

namespace App.Reversi
{
    public class BoardHistoryModel
    {
        public BoardModel boardModel { get; }
        public IReadOnlyList<CellStateHistoryModel> history => _history;
        private CellStateHistoryModel[] _history;

        public BoardHistoryModel(BoardModel boardModel, CellStateHistoryModel[] history)
        {
            this.boardModel = boardModel;
            _history = history;
        }
    }
}