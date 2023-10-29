using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TentacleAi : EnemyManager
{
    public bool isAttacking = false;
    // Start is called before the first frame update
    void Start()
    {
            currentHealth = enemyStats.health;
            playerManager = FindObjectOfType<PlayerManager>();
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, playerManager.transform.position) < 3 && !isAttacking)
        {

            StartCoroutine(StartAttack());
        }
    }

    IEnumerator StartAttack()
    {
        isAttacking = true;
        float cooldown = Random.Range(4, 8);
        GetComponent<Animator>().SetTrigger("Attack");
        yield return new WaitForSeconds(cooldown);
        isAttacking = false;
    }

    public void Attack()
    {
        Debug.Log("Attack");
        Collider[] hitCollider = Physics.OverlapBox(new Vector3(transform.position.x, 1.1f, transform.position.z), new Vector3(1.5f, 1.5f, 1.5f));

        foreach (Collider collider in hitCollider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerManager.TakeDamage(2, 4, Vector3.back);
            }
        }    
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x,1.1f, transform.position.z), new Vector3(1.5f, 1.5f, 1.5f));
    }


}
