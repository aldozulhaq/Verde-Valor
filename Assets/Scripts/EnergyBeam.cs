using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnergyBeam : MonoBehaviour
{
    public GameObject beamPrefab;

    // Start is called before the first frame update
    void Start()
    {
        beamPrefab.SetActive(false);
        StartCoroutine(SummonBeam());
    }

    IEnumerator SummonBeam()
    {
        yield return new WaitForSeconds(3f);
        beamPrefab.SetActive(true);

        //physics overlay cube following the size of beam's collider
        Collider[] colliders = Physics.OverlapBox(beamPrefab.transform.position, new Vector3(1.5f,4.5f,1.5f), beamPrefab.transform.rotation);
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
    }
}
