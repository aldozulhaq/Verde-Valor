using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShoot : MonoBehaviour
{
    public ParticleSystem particle;

    ParticleCollisionEvent collisionEvent;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        transform.LookAt(GameObject.FindAnyObjectByType<PlayerManager>().transform);
    }

    private void Update()
    {
        transform.LookAt(GameObject.FindAnyObjectByType<PlayerManager>().transform);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        other.GetComponent<PlayerManager>().TakeDamage(1, 1, collisionEvent.normal);
        Destroy(gameObject);
    }
}
