using DG.Tweening;
using Lib.ObjectPool.RunTime;
using UnityEngine;
using UnityEngine.UI;

namespace CSH._01_Code.UI
{
    public class MovingImage : MonoBehaviour, IPoolable
    {
        [field:SerializeField] public PoolItemSO PoolItem { get; set; }

        public GameObject GameObject => gameObject;
        private Pool _pool;
        private Image _image;
        private RectTransform _rectTrm;

        public void ResetItem()
        {
            
        }

        public void SetUpPool(Pool pool)
        {
            _pool = pool;
            _image = GetComponent<Image>();
            _rectTrm = GetComponent<RectTransform>();

            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// RectTransform Àü¿ë
        /// </summary>
        public void SetImageAndMoveToTarget(Sprite sprite, Vector2 start, Transform target)
        {
            transform.localScale = Vector3.one;

            _image.sprite = sprite;
            _rectTrm.anchoredPosition = start;
            transform.DOMove(target.position, 1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _pool.Push(this);
            });
        }

        public void SetImageAndMoveToTarget(Sprite sprite, Transform start, Transform target)
        {
            transform.localScale = Vector3.one;

            _image.sprite = sprite;
            transform.position = start.position;
            transform.DOMove(target.position, 1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                _pool.Push(this);
            });
        }
    }
}