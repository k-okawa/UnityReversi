﻿using System.Collections.Generic;

namespace App.Reversi
{
    public class BoardHistoryModel
    {
        public BoardModel prevBoardModel { get; }
        public BoardModel postBoardModel { get; }
        public IReadOnlyList<CellStateHistoryModel> histories => _histories;
        private readonly CellStateHistoryModel[] _histories;

        public BoardHistoryModel(BoardModel prevBoardModel, BoardModel postBoardModel, CellStateHistoryModel[] histories)
        {
            this.prevBoardModel = prevBoardModel;
            this.postBoardModel = postBoardModel;
            _histories = histories;
        }
    }
}