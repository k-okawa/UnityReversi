using UnityEngine;

namespace App.Reversi
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private GameObject _stonePrefab;
        
        public Collider collider => _collider;
        
        public Stone stone { get; private set; }

        public void PutStone(CellState cellState)
        {
            if (stone != null)
            {
                return;
            }

            var go = Instantiate(_stonePrefab, this.transform);
            stone = go.GetComponent<Stone>();
            stone.Set(cellState);
        }
    }
}