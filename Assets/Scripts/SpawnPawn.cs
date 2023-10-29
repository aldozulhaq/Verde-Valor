using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPawn : MonoBehaviour
{
    public GameObject[] pawnLists;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnPawns());
    }

    IEnumerator SpawnPawns()
    {
        int totalSpawn = Random.Range(3, 6);

        for (int i = 0; i < totalSpawn; i++)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x + Random.Range(-2f, 2f), 0.8f, transform.position.z + Random.Range(-2f, 2f));
            yield return new WaitForSeconds(0.3f);
            int randomValue = Random.Range(0, pawnLists.Length);
            Instantiate(pawnLists[randomValue], spawnPosition, Quaternion.identity);
        }
        yield return DestroyAfterTime();
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
