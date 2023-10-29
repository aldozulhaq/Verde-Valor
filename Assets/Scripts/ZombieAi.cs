using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAi : EnemyManager
{
    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = enemyStats.health;
        playerManager = FindObjectOfType<PlayerManager>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!beingHit)
        {
            agent.destination = playerManager.transform.position;
        }
        else
        {
            agent.destination = transform.position;
        }
    }

    private void OnCollisionStay (Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player") && !beingHit)
        {
            playerManager.TakeDamage(1, 3, transform.forward);
        }
    }
}
