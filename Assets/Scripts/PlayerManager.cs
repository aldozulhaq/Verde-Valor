using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public EntityStats playerStats;

    public float ultimatePoint = 0f;
    private float maxUltimatePoint = 100f;
    public float currentHealth;

    public Renderer swordMat;

    public SpriteRenderer playerSR;
    public Animator playerAnimator;

    float playerInitialSpeed;

    public Renderer healthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = playerStats.health;
        playerInitialSpeed = GetComponent<PlayerController>().maxSpeed;
    }

    public void TakeDamage(int damage, float knockbackPower, Vector3 knockbackDir)
    {
        StartCoroutine(ShowHealthBar());
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            FindObjectOfType<GameManager>().GameOver();
        }
        else
        {
            GetComponent<PlayerController>().maxSpeed = 0;
            playerAnimator.SetTrigger("Hit");
            transform.Translate(knockbackDir * playerStats.knockbackRate * knockbackPower);
            StartCoroutine(InvincibilityFrames(0.2f));
        }
    }

    IEnumerator ShowHealthBar()
    {
        //slowly fade in using _EmissiveIntensity
        float timeAnime = 0f;
        if (healthBar.material.GetFloat("_EmissiveIntensity") <= 0.2f)
        {
            while (timeAnime < 1f)
            {
                timeAnime += Time.deltaTime;
                healthBar.material.SetFloat("_EmissiveIntensity", Mathf.Lerp(0, 1, timeAnime));
                yield return null;
            }
        }

        //animate losing health bar using _RemovedSegments
        float timeAnime2 = 0f;
        float HPDiff = (playerStats.health - currentHealth);
        float currentHP = currentHealth + 1f;
        while (timeAnime2 < 2f)
        {
            timeAnime2 += Time.deltaTime;
            healthBar.material.SetFloat("_RemovedSegments", Mathf.SmoothStep(currentHP * (1f / 9f), HPDiff * (1f / 9f), timeAnime2));
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        //slowly fade out using _EmissiveIntensity
        float timeAnime3 = 0f;
        while (timeAnime3 < 1f)
        {
            timeAnime3 += Time.deltaTime;
            healthBar.material.SetFloat("_EmissiveIntensity", Mathf.Lerp(1, 0, timeAnime3));
            yield return null;
        }
    }

    IEnumerator DIE()
    {
        yield return new WaitForSeconds(2f);
    }

    IEnumerator InvincibilityFrames(float invincibilityDuration)
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        playerSR.color = Color.red;
        playerAnimator.SetTrigger("Hit");
        yield return new WaitForSeconds(invincibilityDuration);
        playerSR.color = Color.white;
        gameObject.layer = LayerMask.NameToLayer("Player");
        GetComponent<PlayerController>().maxSpeed = playerInitialSpeed;
    }

    public void GainUltimatePoint(float amount)
    {
        ultimatePoint += amount;

        swordMat.sharedMaterial.SetColor("_EmissionColor", Color.green * Mathf.Lerp(0, 3.5f, ultimatePoint / maxUltimatePoint));

        if (ultimatePoint >= maxUltimatePoint)
        {
            ultimatePoint = maxUltimatePoint;
        }
    }
}
