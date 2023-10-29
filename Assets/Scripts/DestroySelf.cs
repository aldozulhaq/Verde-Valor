using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(destroyME());
    }

    IEnumerator destroyME()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}
