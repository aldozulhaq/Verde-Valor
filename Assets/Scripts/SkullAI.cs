using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkullAI : EnemyManager
{
    public NavMeshAgent agent;
    public GameObject shootProjectile;

    public bool isAttacking = false;

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
        if (!beingHit)
        {
            if (Vector3.Distance(transform.position, playerManager.transform.position) > 5f)
                agent.destination = playerManager.transform.position;
            else
                agent.destination = transform.position - playerManager.transform.position * 2;
        }
        else
            agent.destination = transform.position;

        if (Vector3.Distance(transform.position, playerManager.transform.position) < 10f && !isAttacking)
        {
            isAttacking = true;
            StartCoroutine(Attack());
        }

    }

    IEnumerator Attack()
    {
        float attackCooldown = Random.Range(8f, 12f);
        Vector3 randomPosition = Random.insideUnitSphere * .2f;

        randomPosition += playerManager.transform.position;
        
        randomPosition += new Vector3(0, 2, 0);
        
        GameObject projectile = Instantiate(shootProjectile, randomPosition, Quaternion.identity);
        yield return new WaitForSeconds(attackCooldown);        
        
        isAttacking = false;
    }
}
