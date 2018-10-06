using UnityEngine;

namespace QuickMafs
{
    public class ScoreLocator: MonoBehaviour
    {
        public BoxCollider2D Collider;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Collision");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Trigger");
        }

        private void OnParticleCollision(GameObject other)
        {
            Debug.Log("Particle collision");
        }

        private void OnParticleTrigger()
        {
            Debug.Log("Particle trigger");
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("3d collision");
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("3d trigger");
        }
    }
}
