using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EntityStats enemyStats;

    public int currentHealth;

    public PlayerManager playerManager;

    public bool beingHit = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = enemyStats.health;
        playerManager = FindObjectOfType<PlayerManager>();
    }

    public void TakeDamage(int damage, float knockbackPower, Vector3 knockbackDir)
    {
        currentHealth -= damage;
        beingHit = true;

        transform.Translate(knockbackDir * enemyStats.knockbackRate * knockbackPower);
        StartCoroutine(InvincibilityFrames(0.3f));

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GiveUltimatePoint(10);
            StartCoroutine(DIE());
        }
    }

    IEnumerator DIE()
    {
        Destroy(gameObject);
        yield return new WaitForSeconds(1f);
    }

    protected void GiveUltimatePoint(int amount)
    {
        playerManager.GainUltimatePoint(amount);
    }

    IEnumerator InvincibilityFrames(float invincibilityDuration)
    {
        //gameObject.layer = LayerMask.NameToLayer("Default");
        Color color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = Color.red;
        //Color color = GetComponent<Renderer>().material.color;
        //GetComponent<Renderer>().material.color = Color.red;
        GetComponent<Animator>().SetTrigger("Hit");
        
        yield return new WaitForSeconds(invincibilityDuration);
        //GetComponent<Renderer>().material.color = Color.red;
        //GetComponent<Renderer>().material.color = color;
        GetComponent<SpriteRenderer>().color = color;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        beingHit = false;
    }
}
