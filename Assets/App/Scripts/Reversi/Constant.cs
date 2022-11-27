using UnityEngine;

namespace App.Reversi
{
    public static class Direction
    {
        public static readonly Vector2Int Up = new Vector2Int(0, 1);
        public static readonly Vector2Int UpRight = new Vector2Int(1, 1);
        public static readonly Vector2Int Right = new Vector2Int(1, 0);
        public static readonly Vector2Int DownRight = new Vector2Int(1, -1);
        public static readonly Vector2Int Down = new Vector2Int(0, -1);
        public static readonly Vector2Int DownLeft = new Vector2Int(-1, -1);
        public static readonly Vector2Int Left = new Vector2Int(-1, 0);
        public static readonly Vector2Int UpLeft = new Vector2Int(-1, 1);
    }
}