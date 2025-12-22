using Lib.ObjectPool.RunTime;
using UnityEngine;

namespace Work.Code.Effects
{
    public class PoolingEffect : MonoBehaviour, IPoolable
    {
        [field:SerializeField] public PoolItemSO PoolItem {get; private set; }
        public GameObject GameObject => gameObject;

        private Pool _myPool;
        [SerializeField] private GameObject effectObject;

        private IPlayableVFX _playableVFX;

        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
            _playableVFX = effectObject.GetComponent<IPlayableVFX>();
            Debug.Assert(_playableVFX != null, "effect object must have IPlayableVFX component");
        }
                
        public void ResetItem()
        {
            _playableVFX.StopVFX();
        }

        public void PlayVFX(Vector3 position, Quaternion rotation)
        {
            _playableVFX.PlayVFX(position, rotation);
        }

        public void PushSelf()
        {
            _myPool.Push(this);
        }
        
        private void OnValidate()
        {
            if (effectObject == null) return;
            _playableVFX = effectObject.GetComponent<IPlayableVFX>();
            if (_playableVFX == null)
            {
                effectObject = null;
                Debug.LogError("effect object must have IPlayableVFX component");
            }
        }
    }
}