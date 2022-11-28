using System;
using UnityEngine;

namespace App.Reversi
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private GameObject _stonePrefab;

        public int row { get; private set; }
        public int col { get; private set; }

        public Collider collider => _collider;
        
        public Stone stone { get; private set; }

        private void Awake()
        {
            row = int.Parse(gameObject.name.Substring(4, 1));
            col = int.Parse(gameObject.name.Substring(5, 1));
        }

        public void PutStone(CellState cellState)
        {
            if (stone != null)
            {
                stone.gameObject.SetActive(true);
                return;
            }

            var go = Instantiate(_stonePrefab, this.transform);
            stone = go.GetComponent<Stone>();
            stone.Set(cellState);
        }
    }
}