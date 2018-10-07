using Zenject;
using UnityEngine;

namespace QuickMafs
{
    public delegate void ParticleTriggerEnterEventHandler(int particleCount);
    public class ScoreEffectController
    {
        [Inject] private BurstEffectView _effect;
        [Inject] private ScoreLocator _scoreLocator;
        [Inject] private LateTickController _lateTick;
        [Inject] private Settings _settings;

        public event ParticleTriggerEnterEventHandler ParticlesKilled;

        private ScoreEffectParameters _params;
        private ParticleSystem.Particle[] _particles;

        private float _velocity;

        [Inject]
        private void Initialize(ScoreEffectParameters parameters)
        {
            _lateTick.OnLateTick += LateTick;

            _params = parameters;
            _effect = GameObject.Instantiate(_effect);
            _effect.transform.position = new Vector2(_params.Row, _params.Col);
            _effect.transform.SetParent(_params.Parent, false);
            _effect.ParticleTrigger += OnParticleTrigger;
            _effect.ParticleSystem.trigger.SetCollider(0, _scoreLocator.Collider);

            _particles = new ParticleSystem.Particle[_effect.ParticleSystem.main.maxParticles];
        }

        private void OnParticleTrigger()
        {
            int numEnter = _effect.ParticleSystem.GetSafeTriggerParticlesSize(ParticleSystemTriggerEventType.Enter);
            if(ParticlesKilled != null)
            {
                ParticlesKilled(numEnter);
            }
        }

        private void LateTick()
        {
            // GetParticles is allocation free because we reuse the m_Particles buffer between updates
            int numParticlesAlive = _effect.ParticleSystem.GetParticles(_particles);

            if(numParticlesAlive == 0 && _velocity != 0)
            {
                _velocity = 0;
                _effect.ParticleSystem.Stop();
            }

            if(numParticlesAlive > 0)
            {
                _velocity += _settings.Gravity * Time.deltaTime;
            }

            // Change only the particles that are alive
            for (int i = 0; i < numParticlesAlive; i++)
            {
                _particles[i].position =
                    Vector3.MoveTowards(_particles[i].position, _scoreLocator.transform.position, Time.deltaTime * _velocity);
            }

            // Apply the particle changes to the particle system
            _effect.ParticleSystem.SetParticles(_particles, numParticlesAlive);
        }

        public void Emit(int count)
        {
            _effect.ParticleSystem.Emit(count);
        }

        public class Factory : PlaceholderFactory<ScoreEffectParameters, ScoreEffectController> { }

        [System.Serializable]
        public class Settings
        {
            public float Gravity;
        }
    }

    public class ScoreEffectParameters
    {
        public int Row, Col;
        public Transform Parent;
    }
}
