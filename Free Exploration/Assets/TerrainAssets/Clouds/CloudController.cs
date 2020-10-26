using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    ParticleSystem cloudSystem;
    public Color colour;
    public Color lining;
    bool painted = false;
    public int numberOfParticles;
    public float minSpeed;
    public float maxSpeed;
    public float distance;
    Vector3 startPosition;
    float speed;

    private void Start()
    {
        cloudSystem = GetComponent<ParticleSystem>();
        Spawn();
    }

    void Update()
    {
        transform.Translate(0, 0, -speed);

        if (!painted)
        {
            Paint();
        }

        if (Vector3.Distance(transform.position, startPosition) > distance)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        float xpos = Random.Range(-1.5f, 1.5f);
        float ypos = Random.Range(-1.5f, 1.5f);
        float zpos = Random.Range(-1.5f, 1.5f);
        transform.localPosition = new Vector3(xpos, ypos, zpos);
        speed = Random.Range(minSpeed, maxSpeed);
        startPosition = transform.position;
    }

    private void Paint()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[cloudSystem.particleCount];
        cloudSystem.GetParticles(particles);
        if (particles.Length > 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].startColor = Color.Lerp(lining, colour, particles[i].position.y / cloudSystem.shape.scale.y);
            }
            painted = true;
            cloudSystem.SetParticles(particles, particles.Length);
        }
    }
}
