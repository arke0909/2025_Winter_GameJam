using UnityEngine;

namespace Work.Code.Effects
{
    public class PlayAnimationVFX : MonoBehaviour, IPlayableVFX
    {
        [field:SerializeField] public string VFXName { get; private set; }
        [SerializeField] private bool isOnPosition;
        [SerializeField] private Animator particle;
        
        public void PlayVFX(Vector3 position, Quaternion rotation)
        {
            // 아무것도 안한다.
        }

        public void StopVFX()
        {
        }
    }
}