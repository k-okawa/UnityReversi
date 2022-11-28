using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App.Reversi
{
    public class BoardInputProvider : ITickable
    {
        [Inject] private IPublisher<BoardInputParams> _inputPublisher;

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hitInfo))
                {
                    var cell = hitInfo.collider.gameObject.GetComponent<Cell>();
                    if (cell == null)
                    {
                        return;
                    }
                    _inputPublisher.Publish(new BoardInputParams(cell.row, cell.col));
                }
            }
        }
    }
}