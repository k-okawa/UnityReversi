using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace App.Reversi
{
    public class Stone : MonoBehaviour
    {
        private Quaternion GetStateRotation(CellState state)
        {
            switch (state)
            {
                case CellState.Black:
                    return Quaternion.Euler(0, 0, 0);
                case CellState.White:
                    return Quaternion.Euler(180, 0, 0);
            }
            return Quaternion.identity;
        }
        
        public void Set(CellState state)
        {
            switch (state)
            {
                case CellState.None:
                    gameObject.SetActive(false);
                    break;
                default:
                    transform.rotation = GetStateRotation(state);
                    break;
            }
        }

        public async UniTask PlayPutAnimation()
        {
            transform.localPosition = new Vector3(0, 1f, 0f);
            await transform.DOLocalMove(Vector3.zero, 0.3f).ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        public async UniTask PlayRemoveAnimation()
        {
            transform.localPosition = new Vector3(0, 0f, 0f);
            await transform.DOLocalMove(new Vector3(0, 1f, 0f), 0.3f).ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            Set(CellState.None);
            transform.localPosition = new Vector3(0, 0f, 0f);
        }

        public async UniTask PlayReverseAnimation(CellState state)
        {
            transform.rotation = GetStateRotation(state.GetReversed());
            var seq = DOTween.Sequence();
            seq.Append(transform.DOLocalRotate(GetStateRotation(state).eulerAngles, 0.3f));
            seq.Join(transform.DOLocalMove(new Vector3(0, 1f, 0f), 0.15f).SetLoops(2, LoopType.Yoyo));
            await seq.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }
}