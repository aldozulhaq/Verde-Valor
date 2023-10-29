using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerManager playerManager;

    // Player settings
    public float maxSpeed = 5f;
    private float acceleration = 10f;
    private float dashSpeed = 10f;
    private float dashDistance = 3f;
    public float dashCooldown = 1.5f;
    private float invincibilityDuration = 0.2f;

    // Internal variables
    private Vector3 movementInput;
    private Vector3 dashDirection;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isAttacking = false;

    // Store the starting position of the dash for distance calculation
    private Vector3 dashStartPos;

    public Transform attackTarget;
    public GameObject thrustTarget;
    public LayerMask enemyLayers;
    private float attackRange = 1.5f;

    private bool inCombo = false;
    private int comboCount = 3;
    private int comboIndex = 0;
    private float comboTimer = 0f;
    private float comboTimeLimit = 0.9f;
    
    public GameObject swordParent;
    public Animator swordAnimator;

    public SpriteRenderer playerSR;
    public Animator playerAnimator;

    public Renderer dashCooldownMat;

    private Vector3 currentVelocity;

    public GameObject swordImpact;
    public GameObject thrustImpact;

    void Update()
    {
        HandleMovementInput();
        HandleDashInput();
        HandleMeleeAttackInput();
        HandleTimers();
        HandleRot();

        // Apply acceleration and deceleration
        currentVelocity = Vector3.Lerp(currentVelocity, movementInput * maxSpeed, acceleration * Time.deltaTime);
        transform.Translate(currentVelocity * Time.deltaTime);
    }

    void HandleMovementInput()
    {
        if (isDashing || isAttacking)
        {
            playerAnimator.SetTrigger("Static");
            movementInput = Vector3.zero;
        }
        else
        {
            movementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
            
            if(movementInput != Vector3.zero)
                playerAnimator.SetTrigger("Run");
            else
                playerAnimator.SetTrigger("Static");
        }
    }

    void HandleRot()
    {
        if(movementInput.x < 0)
        {
            if(!isAttacking)
                swordParent.transform.localEulerAngles = new Vector3(0, 180, 0);
            playerSR.flipX = true;
        }
        else if(movementInput.x > 0)
        {
            if (!isAttacking)
                swordParent.transform.localEulerAngles = new Vector3(0, 0, 0);
            playerSR.flipX = false;
        }
    }

    void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isAttacking)
        {
            Dash();
        }

        if (isDashing)
        {
            float distance = dashSpeed * Time.deltaTime;
            transform.Translate(dashDirection * distance);
        }
    }

    void Dash()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        dashDirection = movementInput.normalized;
        isDashing = true;
        canDash = false;
        dashStartPos = transform.position; // Store the starting position for distance calculation
                                           // Add any visual effects or animations for dashing here


        // Invincibility frames during dash
        StartCoroutine(InvincibilityFrames());
        StartCoroutine(DashCooldown());
    }

    IEnumerator InvincibilityFrames()
    {
        yield return new WaitForSeconds(invincibilityDuration);
        isDashing = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    void HandleMeleeAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && !isDashing)
        {
            MeleeAttack();
        }

        if (Input.GetMouseButtonDown(1) && !isAttacking && !isDashing)
        {
            //thrustTarget.SetActive(true);
            ThrustAttack();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking && !isDashing)
        {
            UltimateAttack();
        }
    }

    void UltimateAttack()
    {
        int damage = Mathf.Abs((int)GetComponent<PlayerManager>().ultimatePoint / 10);
        int heal = Mathf.Abs((int)GetComponent<PlayerManager>().ultimatePoint / 40);
        if(heal > 9)
        {
            heal = 9;
        }
        isAttacking = true;
        swordAnimator.SetTrigger("Ultimate");
        Collider[] collider = Physics.OverlapSphere(transform.position, 9, enemyLayers);

        GiveDamage(collider, damage, 4, Vector3.right);

        if(GetComponent<PlayerManager>().currentHealth + heal > 9)
        {
            GetComponent<PlayerManager>().currentHealth = 9;
        }
        else
        {
            GetComponent<PlayerManager>().currentHealth += heal;
        }

        GetComponent<PlayerManager>().ultimatePoint = 0;

        StartCoroutine(meleeRecoveryTime(1f));
    }

    void ThrustAttack()
    {
        isAttacking = true;

        Vector3 attackDir = Vector3.zero;
        Vector3 thrustImpactEuler = Vector3.zero;

        if(Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.z))
        {
            thrustTarget.transform.localEulerAngles = new Vector3(0, 0, 0);
            thrustImpactEuler = new Vector3(90, 90, 0);
            if (movementInput.x < 0)
            {
                thrustTarget.transform.localPosition = new Vector3(-3.078f, -0.125f, 0);
                swordParent.transform.localEulerAngles = new Vector3(0, 180, 0);
                attackDir = Vector3.left;
            }
            else if (movementInput.x > 0)
            {
                thrustTarget.transform.localPosition = new Vector3(3.078f, -0.125f, 0);
                swordParent.transform.localEulerAngles = new Vector3(0, 0, 0);
                attackDir = Vector3.right;
            }
        } else {
            thrustTarget.transform.localEulerAngles = new Vector3(0, 90, 0);
            thrustImpactEuler = new Vector3(90, 0, 0);
            if (movementInput.z < 0)
            {
                thrustTarget.transform.localPosition = new Vector3(0, -0.125f, -3.078f);
                swordParent.transform.localEulerAngles = new Vector3(90, 90, 0);
                attackDir = Vector3.back;
            }
            else if (movementInput.z > 0)
            {
                thrustTarget.transform.localPosition = new Vector3(0, -0.125f, 3.078f);
                swordParent.transform.localEulerAngles = new Vector3(90, -90, 0);
                attackDir = Vector3.forward;
            }
        }

        Instantiate(thrustImpact, new Vector3(transform.position.x, 1.2f, transform.position.z), Quaternion.Euler(thrustImpactEuler), transform);

        swordAnimator.SetTrigger("Thrust");
        transform.Translate(attackDir * 1.5f);

        Collider[] hitEnemies = Physics.OverlapBox(thrustTarget.transform.position, new Vector3(3.5f, 0.375f, 1f), thrustTarget.transform.rotation, enemyLayers);

        GiveDamage(hitEnemies, 1, 2, attackDir);

        StartCoroutine(meleeRecoveryTime(0.6f));
    }

    void MeleeAttack()
    {
        isAttacking = true;

        StartCoroutine(comboAttackAnim());        
    }

    IEnumerator comboAttackAnim()
    {
        Vector3 attackDir = Vector3.zero;
        int damage = 1;

        if (movementInput.x < 0)
        {
            attackTarget.localPosition = new Vector3(-0.86f, 0.84f, 0);
            attackDir = Vector3.left;
        }
        else if (movementInput.x > 0)
        {
            attackTarget.localPosition = new Vector3(0.86f, 0.84f, 0);
            attackDir = Vector3.right;
        }

        switch (comboIndex)
        {
            case 0:
                damage = 1;
                swordAnimator.SetTrigger("ComboAnim1");
                inCombo = true;
                comboTimer = 0f;
                StartCoroutine(meleeRecoveryTime(0.4f));
                comboIndex++;
                break;
            case 1:
                damage = 1;
                swordAnimator.SetTrigger("ComboAnim2");
                inCombo = true;
                comboTimer = 0f;
                StartCoroutine(meleeRecoveryTime(0.4f));
                comboIndex++;
                break;
            case 2:
                damage = 2;
                swordAnimator.SetTrigger("ComboAnim3");
                inCombo = false;
                StartCoroutine(meleeRecoveryTime(0.6f));
                comboIndex = 0;
                break;
        }


        yield return new WaitForSeconds(0.15f); // anim time wing up

        transform.Translate(attackDir * 1f);
        Collider[] hitEnemies = Physics.OverlapSphere(attackTarget.position, attackRange, enemyLayers);

        GiveDamage(hitEnemies, damage, 1f, attackDir);

        swordAnimator.SetTrigger("Idle");
    }

    void GiveDamage(Collider[] enemies, int damage, float knockbackPower, Vector3 attackDir)
    {
        foreach (Collider enemy in enemies)
        {
            if(enemy.GetComponent<EnemyManager>() != null)
                enemy.GetComponent<EnemyManager>().TakeDamage(damage, knockbackPower, attackDir);
            else
                enemy.GetComponent<WellManager>().TakeDamage(1);

            var SI = Instantiate(swordImpact, new Vector3(enemy.transform.position.x, enemy.transform.position.y + (enemy.transform.position.y / 2), enemy.transform.position.z), Quaternion.identity, transform);
            SI.GetComponent<ParticleSystemRenderer>().flip = attackDir;

        }
    }

    IEnumerator meleeRecoveryTime(float time)
    {
        yield return new WaitForSeconds(time);
        isAttacking = false;
    }

    void HandleTimers()
    {
        if (inCombo)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer >= comboTimeLimit)
            {
                inCombo = false;
                comboIndex = 0;
            }
        }
    }

    IEnumerator DashCooldown()
    {
        StartCoroutine(DashCDAnim());
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator DashCDAnim()
    {
        float timeDC = 0f;
        float targetIntensity = 1.5f;
        float timeRate = 0.01f;

        dashCooldownMat.material.SetFloat("_RemovedSegments", 0);

        for (int i = 0; i < 45; i++)
        {
            timeDC += timeRate;
            float emissiveIntensity = Mathf.Lerp(0, targetIntensity, timeDC);
            dashCooldownMat.material.SetFloat("_EmissiveIntensity", emissiveIntensity);
            yield return new WaitForSeconds(timeRate);
        }

        timeDC = 0f;

        for (int i = 0; i < 45; i++)
        {
            timeDC += timeRate;
            float dashCDMatAlpha = Mathf.SmoothStep(0, 1.5f, timeDC * 2);
            dashCooldownMat.material.SetFloat("_RemovedSegments", dashCDMatAlpha);
            yield return new WaitForSeconds(timeRate);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackTarget.position, 1.5f);
    }

}
