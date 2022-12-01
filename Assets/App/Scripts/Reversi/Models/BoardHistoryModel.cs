using System.Collections.Generic;

namespace App.Reversi
{
    public class BoardHistoryModel
    {
        public BoardModel postBoardModel { get; }
        public IReadOnlyList<CellStateHistoryModel> histories => _histories;
        private readonly CellStateHistoryModel[] _histories;

        public BoardHistoryModel(BoardModel postBoardModel, CellStateHistoryModel[] histories)
        {
            this.postBoardModel = postBoardModel;
            _histories = histories;
        }
    }
}