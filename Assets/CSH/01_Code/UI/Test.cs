using Lib.ObjectPool.RunTime;
using UnityEngine;
using Work.Code.SoundSystem;
namespace CSH._01_Code.UI
{
    public class Test : MonoBehaviour
    {
        private Canvas canvas;
        [SerializeField] private SoundSO sound;
        [SerializeField] private PoolManagerMono poolManager;
        [SerializeField] private PoolItemSO poolItem;
        private void Awake()
        {
            poolManager.Pop<SoundPlayer>(poolItem).PlaySound(sound);
        }
    }
}
