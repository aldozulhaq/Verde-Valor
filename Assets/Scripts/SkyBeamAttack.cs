using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBeamAttack : MonoBehaviour
{
    public GameObject beamPrefab;
    private void Start()
    {
        beamPrefab.SetActive(false);
        StartCoroutine(SummonBeam());
    }
    IEnumerator SummonBeam()
    {
        yield return new WaitForSeconds(3f);
        beamPrefab.SetActive(true);
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerManager>().TakeDamage(3,0,Vector3.zero);
            }
        }
        yield return DestroyAfterTime();
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
