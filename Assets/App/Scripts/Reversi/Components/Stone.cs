using UnityEngine;

namespace App.Reversi
{
    public class Stone : MonoBehaviour
    {
        public void Set(CellState state)
        {
            switch (state)
            {
                case CellState.Black:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case CellState.White:
                    transform.rotation = Quaternion.Euler(180, 0, 0);
                    break;
            }
        }
    }
}