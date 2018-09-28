using UnityEngine;

public class ParticleAtraction : MonoBehaviour
{
    public Transform Attractor;
    public ParticleSystem System;
    public ParticleSystem.Particle[] particles;

    public float Gravity;
    private float _velocity;

    private void Start()
    {

    }

    void InitializeIfNeeded()
    {
        if (System == null)
            System = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < System.main.maxParticles)
            particles = new ParticleSystem.Particle[System.main.maxParticles];
    }

    private void LateUpdate()
    {
        InitializeIfNeeded();

        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        int numParticlesAlive = System.GetParticles(particles);

        // Change only the particles that are alive
        for (int i = 0; i < numParticlesAlive; i++)
        {
            _velocity += Gravity * Time.deltaTime;
            particles[i].position =
                Vector3.MoveTowards(particles[i].position, Attractor.position, Time.deltaTime * _velocity);
        }

        // Apply the particle changes to the particle system
        System.SetParticles(particles, numParticlesAlive);
    }
}
