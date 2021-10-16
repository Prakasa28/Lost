using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AnimationAndMovementv2Controller : MonoBehaviour
{
    PlayerInputV2 playerInput;
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

    Vector2 currentMovementInput;
    Vector3 currentMovement;

    bool isMovementPressed;
    bool isDodgePressed;
    bool isAttackedPressed;
    bool isChargePressed;
    bool isUltimatePressed;

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

    public GameObject attackAbilityUIObject;
    public GameObject dashAblilityUIObject;
    public GameObject chargeAblilityUIObject;
    public GameObject ultimateAbilityUIObject;

    void Awake()
    {
        playerInput = new PlayerInputV2();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        enemy = GetComponent<FocusEnemy>();

        isMovingHash = Animator.StringToHash("IsMoving");
        isDodgingHash = Animator.StringToHash("IsDodging");
        isAttackingHash1 = Animator.StringToHash("IsAttacking1");
        isAttackingHash2 = Animator.StringToHash("IsAttacking2");
        isChargingHash = Animator.StringToHash("IsCharging");
        isEndCharging = Animator.StringToHash("IsEndCharging");
        isUltimatingHash = Animator.StringToHash("IsUltimating");

        playerInput.CharacterControlls.Move.started += onMovementInput;
        playerInput.CharacterControlls.Move.canceled += onMovementInput;
        playerInput.CharacterControlls.Move.performed += onMovementInput;
        playerInput.CharacterControlls.Dodge.started += OnDodge;
        playerInput.CharacterControlls.Dodge.canceled += OnDodge;
        playerInput.CharacterControlls.Ultimate.started += OnUltimate;
        playerInput.CharacterControlls.Ultimate.canceled += OnUltimate;
        playerInput.CharacterControlls.Attack.started += OnAttack;
        playerInput.CharacterControlls.Attack.canceled += OnAttack;
        playerInput.CharacterControlls.Charge.started += OnCharge;
        playerInput.CharacterControlls.Charge.canceled += OnCharge;



    }
    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.z = currentMovementInput.x;
        currentMovement.x = -currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }
    void OnDodge(InputAction.CallbackContext context)
    {
        isDodgePressed = context.ReadValueAsButton();
    }
    void OnUltimate(InputAction.CallbackContext context)
    {
        isUltimatePressed = context.ReadValueAsButton();
    }
    void OnAttack(InputAction.CallbackContext context)
    {
        isAttackedPressed = context.ReadValueAsButton();
    }

    void OnCharge(InputAction.CallbackContext context)
    {
        isChargePressed = context.ReadValueAsButton();
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

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;


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
            currentMovement.y = groundedGravity;
        }
        else
        {
            float gravity = -10f;
            currentMovement.y = gravity;
        }
    }
    void handleMovement()
    {
        handleGravity();
        handleRotation();
        characterController.Move(currentMovement * characterSpeed * Time.deltaTime);
    }


    void handleActions()
    {
        bool isMoving = animator.GetBool(isMovingHash);

        if (isMovementPressed && !isMoving)
        {
            animator.SetBool(isMovingHash, true);
        }

        if (!isMovementPressed && isMoving)
        {
            animator.SetBool(isMovingHash, false);
        }

        if (!performingAction)
        {
            if (isAttackedPressed && attackCooldownCurrent <= 0)
            {
                StartCoroutine(Attack());
            }

            if (isDodgePressed && dashCooldownCurrent <= 0)
            {
                StartCoroutine(Dash());
            }


            if (enemy.focusedTarget != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.focusedTarget.transform.position);

                if (isChargePressed && chargeCooldownCurrent <= 0 && distance >= chargeMinRadius && distance <= chargeMaxRadius)
                {
                    StartCoroutine(Charge());
                }

            }

            if (isUltimatePressed && ultimateCooldownCurrent <= 0)
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


        animator.SetBool(isChargingHash, true);
        while (true)
        {
            FaceEnemy(focusedEnemy);
            var offset = focusedEnemy.transform.position - transform.position;
            offset.y = 0;

            if (offset.magnitude < 4f)
            {
                break;
            }

            offset = offset.normalized * chargeSpeed;
            characterController.Move(offset * Time.deltaTime);


            yield return null;
        }
        animator.SetBool(isChargingHash, false);
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

        Vector3 dashDir = currentMovement;
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




    void OnEnable()
    {
        playerInput.CharacterControlls.Enable();
    }
    void OnDisable()
    {
        playerInput.CharacterControlls.Disable();
    }

}
