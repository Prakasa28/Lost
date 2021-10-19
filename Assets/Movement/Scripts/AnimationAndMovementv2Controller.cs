using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AnimationAndMovementv2Controller : MonoBehaviour
{
    InputHandler inputHandler;
    CharacterController characterController;
    Animator animator;
    FocusEnemy enemy;

    public float characterSpeed = 15;
    public float dashTime = .7f;
    public float dashSpeed = 25;
    public float chargeSpeed = 50;
    public float chargeMinRadius = 20;
    public float chargeMaxRadius = 40;
    public float rotationFactorPerFrame = 30.0f;

    int isMovingHash;
    int isDodgingHash;
    int isAttackingHash1;
    int isAttackingHash2;
    int isChargingHash;
    int isEndCharging;
    int isUltimatingHash;

    bool performingAction = false;
    bool canMove = true;

    public float attackCooldown = .2f;
    public float dashCooldown = 1.5f;
    public float chargeCooldown = 6f;
    public float ultimateCooldown = 6f;

    private float attackCooldownCurrent = 0;
    private float dashCooldownCurrent = 0;
    private float chargeCooldownCurrent = 0;
    private float ultimateCooldownCurrent = 0;

    private bool switchAttackAnimations = false;


    private float nextActionTime = 0.0f;
    private float period = .1f; //Update cooldown timer every sec

    public ParticleSystem particleForCharge;
    public GameObject attackAbilityUIObject;
    public GameObject dashAblilityUIObject;
    public GameObject chargeAblilityUIObject;
    public GameObject ultimateAbilityUIObject;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        enemy = GetComponent<FocusEnemy>();
        inputHandler = GetComponent<InputHandler>();

        isMovingHash = Animator.StringToHash("IsMoving");
        isDodgingHash = Animator.StringToHash("IsDodging");
        isAttackingHash1 = Animator.StringToHash("IsAttacking1");
        isAttackingHash2 = Animator.StringToHash("IsAttacking2");
        isChargingHash = Animator.StringToHash("IsCharging");
        isEndCharging = Animator.StringToHash("IsEndCharging");
        isUltimatingHash = Animator.StringToHash("IsUltimating");

    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            handleMovement();
        }

        updateUI();
        cooldownControl();
        handleActions();
    }

    void handleRotation(float rotationSpeed = 0)
    {
        if (rotationSpeed == 0)
            rotationSpeed = rotationFactorPerFrame * Time.deltaTime;

        Vector3 positionToLookAt;

        positionToLookAt.x = inputHandler.currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = inputHandler.currentMovement.z;


        Quaternion currentRotation = transform.rotation;

        if (positionToLookAt != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed);
        }


    }
    void handleGravity()
    {
        if (characterController.isGrounded)
        {
            float groundedGravity = -.5f;
            inputHandler.currentMovement.y = groundedGravity;
        }
        else
        {
            float gravity = -10f;
            inputHandler.currentMovement.y = gravity;
        }
    }
    void handleMovement()
    {
        handleGravity();
        handleRotation();
        characterController.Move(inputHandler.currentMovement * characterSpeed * Time.deltaTime);
    }


    void handleActions()
    {
        bool isMoving = animator.GetBool(isMovingHash);

        if (inputHandler.isMovementPressed && !isMoving)
        {
            animator.SetBool(isMovingHash, true);
        }

        if (!inputHandler.isMovementPressed && isMoving)
        {
            animator.SetBool(isMovingHash, false);
        }

        if (!performingAction)
        {
            if (inputHandler.isAttackedPressed && attackCooldownCurrent <= 0)
            {
                StartCoroutine(Attack());
            }

            if (inputHandler.isDodgePressed && dashCooldownCurrent <= 0)
            {
                StartCoroutine(Dash());
            }


            if (enemy.focusedTarget != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.focusedTarget.transform.position);

                if (inputHandler.isChargePressed && chargeCooldownCurrent <= 0 && distance >= chargeMinRadius && distance <= chargeMaxRadius)
                {
                    StartCoroutine(Charge());
                }

            }

            if (inputHandler.isUltimatePressed && ultimateCooldownCurrent <= 0)
            {
                StartCoroutine(Ultimate());
            }

        }
    }

    IEnumerator Charge()
    {

        performingAction = true;
        canMove = false;

        GameObject focusedEnemy = enemy.focusedTarget;

        if (particleForCharge != null)
            particleForCharge.Play();

        animator.SetBool(isChargingHash, true);
        while (true)
        {
            FaceEnemy(focusedEnemy);
            var offset = focusedEnemy.transform.position - transform.position;
            offset.y = 0;

            if (offset.magnitude < 6f)
            {
                break;
            }

            offset = offset.normalized * chargeSpeed;
            characterController.Move(offset * Time.deltaTime);


            yield return null;
        }

        animator.SetBool(isChargingHash, false);

        if (particleForCharge != null)
            particleForCharge.Stop();

        animator.SetBool(isEndCharging, true);

        yield return new WaitForSeconds(.4f);

        animator.SetBool(isEndCharging, false);

        chargeCooldownCurrent = chargeCooldown;
        canMove = true;
        performingAction = false;
    }

    private void FaceEnemy(GameObject enemy)
    {
        //face the target
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1);

    }

    IEnumerator Attack()
    {
        performingAction = true;

        if (switchAttackAnimations)
        {
            animator.SetBool(isAttackingHash1, true);
            yield return new WaitForSeconds(.5f);
            animator.SetBool(isAttackingHash1, false);
        }
        else
        {
            animator.SetBool(isAttackingHash2, true);
            yield return new WaitForSeconds(1f);
            animator.SetBool(isAttackingHash2, false);

        }

        switchAttackAnimations = !switchAttackAnimations;
        attackCooldownCurrent = attackCooldown;

        performingAction = false;
    }
    IEnumerator Ultimate()
    {
        performingAction = true;
        canMove = false;

        animator.SetBool(isUltimatingHash, true);
        yield return new WaitForSeconds(2.3f);
        animator.SetBool(isUltimatingHash, false);
        yield return new WaitForSeconds(0.2f);

        ultimateCooldownCurrent = ultimateCooldown;

        canMove = true;
        performingAction = false;
    }
    IEnumerator Dash()
    {
        performingAction = true;
        canMove = false;

        Vector3 dashDir = inputHandler.currentMovement;
        float startTime = Time.time;
        handleRotation(1);
        animator.SetBool(isDodgingHash, true);
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(dashDir * dashSpeed * Time.deltaTime);
            yield return null;
        }
        animator.SetBool(isDodgingHash, false);

        dashCooldownCurrent = dashCooldown;

        canMove = true;
        performingAction = false;
    }


    void cooldownControl()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            if (dashCooldownCurrent > 0)
                dashCooldownCurrent -= period;

            if (chargeCooldownCurrent > 0)
                chargeCooldownCurrent -= period;

            if (ultimateCooldownCurrent > 0)
                ultimateCooldownCurrent -= period;

            if (attackCooldownCurrent > 0)
                attackCooldownCurrent -= period;

        }

    }
    void updateUIAbility(float cooldown, GameObject uiObject, bool avaliable = true)
    {
        Image image = uiObject.transform.Find("Image").gameObject.GetComponent<Image>();
        GameObject coolDownObject = uiObject.transform.Find("Cooldown").gameObject;

        if (cooldown > 0)
        {
            image.fillAmount -= 1 / cooldown * Time.deltaTime;
            coolDownObject.SetActive(true);

            float coolDownToShow = Mathf.Floor(cooldown);

            if (cooldown < 1)
            {
                coolDownToShow = cooldown;
                coolDownObject.transform.GetComponent<TextMeshProUGUI>().fontSize = 19;
                coolDownObject.transform.GetComponent<TextMeshProUGUI>().text = coolDownToShow.ToString("F1");
            }
            else
            {

                coolDownObject.transform.GetComponent<TextMeshProUGUI>().text = coolDownToShow.ToString();
            }
        }
        else
        {
            if (!avaliable)
            {
                image.fillAmount = 0;
            }
            else
            {
                image.fillAmount = 1;
            }
            coolDownObject.SetActive(false);
        }

    }
    void updateUI()
    {
        bool chargeAvaliable = false;
        if (enemy.focusedTarget != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.focusedTarget.transform.position);

            if (distance >= chargeMinRadius && distance <= chargeMaxRadius)
                chargeAvaliable = true;
        }

        updateUIAbility(dashCooldownCurrent, dashAblilityUIObject);
        updateUIAbility(ultimateCooldownCurrent, ultimateAbilityUIObject);
        updateUIAbility(attackCooldownCurrent, attackAbilityUIObject);
        updateUIAbility(chargeCooldownCurrent, chargeAblilityUIObject, chargeAvaliable);
    }

}
