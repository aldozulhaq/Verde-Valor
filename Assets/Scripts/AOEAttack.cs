using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEAttack : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(attack());
    }

    IEnumerator attack()
    {
        yield return new WaitForSeconds(4.5f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, 4.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerManager>().TakeDamage(5,0,Vector3.zero);
            }
        }
        yield return DestroyAfterTime();
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 4.5f);
    }
}
