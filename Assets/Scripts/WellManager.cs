using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellManager : MonoBehaviour
{
    public int currentHealth = 4;
    public Renderer healthBar;

    public GameObject LICH;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;        

        StartCoroutine(healthBarAnim());

        healthBar.material.SetFloat("_RemovedSegments", healthBar.material.GetFloat("_RemovedSegments") + 0.08f);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Destroy(gameObject);
            Instantiate(LICH, new Vector3(transform.position.x + 2, transform.position.y, transform.position.z - 2), Quaternion.identity);
        }
    }

    IEnumerator healthBarAnim()
    {
        float timeIn = 0f;
        float timeOut = 0f;
        float targetIntensity = 1.5f;
        float timeRate = 0.01f;

        for (int i = 0; i < 100; i++)
        {
            timeIn += timeRate;
            float emissiveIntensity = Mathf.Lerp(0, targetIntensity, timeIn);
            healthBar.material.SetFloat("_EmissiveIntensity", emissiveIntensity);
            yield return new WaitForSeconds(timeRate);
        }

        yield return new WaitForSeconds(0.5f);
        //emissiveIntensity = 0f;

        for (int i = 0; i < 100; i++)
        {
            timeOut += timeRate;
            float emissiveIntensity = Mathf.Lerp(targetIntensity, 0, timeOut);
            healthBar.material.SetFloat("_EmissiveIntensity", emissiveIntensity);
            yield return new WaitForSeconds(timeRate);
        }
    }
}
