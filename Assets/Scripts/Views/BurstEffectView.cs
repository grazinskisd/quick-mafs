using UnityEngine;

namespace QuickMafs
{
    public delegate void BurstEffectEventHandler();
    public class BurstEffectView: MonoBehaviour
    {
        public ParticleSystem ParticleSystem;
        public event BurstEffectEventHandler ParticleTrigger;

        private void OnParticleTrigger()
        {
            if(ParticleTrigger != null)
            {
                ParticleTrigger();
            }
        }
    }
}
