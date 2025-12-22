using UnityEngine;

namespace Work.Code.Effects
{
    public class PlayParticleVFX : MonoBehaviour, IPlayableVFX
    {
        [field:SerializeField] public string VFXName { get; private set; }
        [SerializeField] private bool isOnPosition;
        [SerializeField] private ParticleSystem particle;
        

        public void PlayVFX(Vector3 position, Quaternion rotation)
        {
            if(isOnPosition == false)
                transform.SetPositionAndRotation(position, rotation);
            
            particle.Play(true);
        }

        public void StopVFX()
        {
            particle.Stop(true);
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(VFXName) == false)
                gameObject.name = VFXName;
        }

    }
}