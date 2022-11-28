using VContainer;

namespace App.Reversi
{
    public struct BoardInputParams
    {
        public int row { get; }
        public int col { get; }
        
        [Inject]
        public BoardInputParams(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
}