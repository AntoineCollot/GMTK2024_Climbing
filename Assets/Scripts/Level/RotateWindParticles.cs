using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWindParticles : MonoBehaviour
{
    public float minRevolutionTime = 4;
    public float maxRevolutionTime = 6;
    int particleCount;
    ParticleSystem.Particle[] particles;

    ParticleSystem particleSyst;

    // Start is called before the first frame update
    void Start()
    {
        particleSyst = GetComponent<ParticleSystem>();
        particleCount = particleSyst.main.maxParticles;
        particles = new ParticleSystem.Particle[particleCount];
    }

    // Update is called once per frame
    void Update()
    {
        RotateParticles();
    }

    void RotateParticles()
    {
        particleSyst.GetParticles(particles, particleSyst.particleCount);

        for (int i = 0; i < particleSyst.particleCount; i++)
        {
            particles[i].position = Quaternion.Euler(0, Time.deltaTime * 360 / Mathf.Lerp(minRevolutionTime, maxRevolutionTime, (float)i/(particleCount-1)), 0) * particles[i].position;

        }
        particleSyst.SetParticles(particles, particleSyst.particleCount);
    }
}
