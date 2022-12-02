using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Reversi
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private GameObject _stonePrefab;
        [SerializeField] private GameObject _hintCube;

        public int row { get; private set; }
        public int col { get; private set; }

        public Stone stone { get; private set; }

        private void Awake()
        {
            row = int.Parse(gameObject.name.Substring(4, 1));
            col = int.Parse(gameObject.name.Substring(5, 1));
            
            var go = Instantiate(_stonePrefab, this.transform);
            go.SetActive(false);
            stone = go.GetComponent<Stone>();
        }

        public async UniTask PutStone(CellState cellState)
        {
            stone.gameObject.SetActive(true);
            stone.Set(cellState);
            await stone.PlayPutAnimation();
        }

        public async UniTask RemoveStone()
        {
            if (stone != null)
            {
                await stone.PlayRemoveAnimation();
            }
        }

        public void SetHint(bool isOn)
        {
            _hintCube.SetActive(isOn);
        }
    }
}