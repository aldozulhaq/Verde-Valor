using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player settings
    public float maxSpeed = 5f;
    public float acceleration = 10f;
    public float dashSpeed = 10f;
    public float dashDistance = 3f;
    public float dashCooldown = 1.5f;
    public float invincibilityDuration = 0.2f;

    // Internal variables
    private Vector3 movementInput;
    private Vector3 dashDirection;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isAttacking = false;
    private float dashTimer = 0f;

    // Store the starting position of the dash for distance calculation
    private Vector3 dashStartPos;

    public Transform attackTarget;
    public GameObject thrustTarget;
    public LayerMask enemyLayers;
    public float attackRange = 1.5f;

    public bool inCombo = false;
    private int comboCount = 3;
    public int comboIndex = 0;
    private float comboTimer = 0f;
    private float comboTimeLimit = 0.9f;
    
    public GameObject swordParent;
    public Animator swordAnimator;

    public SpriteRenderer playerSR;
    public Animator playerAnimator;
    
    private Vector3 currentVelocity;

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
            playerAnimator.SetTrigger("Run");
            movementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
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

            // Check if the dash distance has been reached
            if (Vector3.Distance(transform.position, dashStartPos) >= dashDistance) 
            {
                isDashing = false;
            }
        }
    }

    void Dash()
    {
        dashDirection = movementInput.normalized;
        isDashing = true;
        canDash = false;
        dashTimer = 0f;
        dashStartPos = transform.position; // Store the starting position for distance calculation
        // Add any visual effects or animations for dashing here

        // Invincibility frames during dash
        StartCoroutine(InvincibilityFrames());
    }

    IEnumerator InvincibilityFrames()
    {
        yield return new WaitForSeconds(invincibilityDuration);
        isDashing = false;
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
    }

    void ThrustAttack()
    {
        isAttacking = true;

        Vector3 attackDir = Vector3.zero;

        if(Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.z))
        {
            thrustTarget.transform.localEulerAngles = new Vector3(0, 0, 0);
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

        swordAnimator.SetTrigger("Thrust");
        transform.Translate(attackDir * 1.5f);

        Collider[] hitEnemies = Physics.OverlapBox(thrustTarget.transform.position, new Vector3(3.5f, 0.375f, 1f), thrustTarget.transform.rotation, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
        }

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
                swordAnimator.SetTrigger("ComboAnim1");
                inCombo = true;
                comboTimer = 0f;
                StartCoroutine(comboAttackDelay());
                comboIndex++;
                break;
            case 1:
                swordAnimator.SetTrigger("ComboAnim2");
                inCombo = true;
                comboTimer = 0f;
                StartCoroutine(comboAttackDelay());
                comboIndex++;
                break;
            case 2:
                swordAnimator.SetTrigger("ComboAnim3");
                inCombo = false;
                StartCoroutine(meleeRecoveryTime(0.6f));
                comboIndex = 0;
                break;
        }


        yield return new WaitForSeconds(0.15f); // anim time wing up

        transform.Translate(attackDir * 1f);
        //attackTarget.gameObject.GetComponent<Renderer>().enabled = true;
        Collider[] hitEnemies = Physics.OverlapSphere(attackTarget.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
        }

        swordAnimator.SetTrigger("Idle");
        hitEnemies = null;
    }

    IEnumerator comboAttackDelay()
    {
        yield return new WaitForSeconds(0.3f); // anim time
        isAttacking = false;
        //attackTarget.gameObject.GetComponent<Renderer>().enabled = false;
    }

    IEnumerator meleeRecoveryTime(float time)
    {
        yield return new WaitForSeconds(time);
        isAttacking = false;
        //attackTarget.gameObject.GetComponent<Renderer>().enabled = false;
        //thrustTarget.SetActive(false);
    }

    void HandleTimers()
    {
        if (!canDash)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashCooldown)
            {
                canDash = true;
            }
        }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackTarget.position, 1.5f);
    }

}
