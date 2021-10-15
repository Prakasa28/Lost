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

    public float characterSpeed = 15;
    public float dashTime = .7f;
    public float dashSpeed = 25;
    public float rotationFactorPerFrame = 30.0f;

    int isMovingHash;
    int isDodgingHash;
    int isUltimatingHash;

    Vector2 currentMovementInput;
    Vector3 currentMovement;

    bool isMovementPressed;
    bool isDodgePressed;
    bool isUltimatePressed;

    bool performingAction = false;

    public float dashCooldown = 1.5f;
    public float ultimateCooldown = 6f;

    private float dashCooldownCurrent = 0;
    private float ultimateCooldownCurrent = 0;


    private float nextActionTime = 0.0f;
    private float period = .5f; //Update cooldown timer every sec

    public GameObject normalAblilityUIObject;
    public GameObject dashAblilityUIObject;
    public GameObject ultimateAbilityUIObject;



    void Awake()
    {
        playerInput = new PlayerInputV2();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isMovingHash = Animator.StringToHash("IsMoving");
        isDodgingHash = Animator.StringToHash("IsDodging");
        isUltimatingHash = Animator.StringToHash("IsUltimating");

        playerInput.CharacterControlls.Move.started += onMovementInput;
        playerInput.CharacterControlls.Move.canceled += onMovementInput;
        playerInput.CharacterControlls.Move.performed += onMovementInput;
        playerInput.CharacterControlls.Dodge.started += OnDodge;
        playerInput.CharacterControlls.Dodge.canceled += OnDodge;
        playerInput.CharacterControlls.Ultimate.started += OnUltimate;
        playerInput.CharacterControlls.Ultimate.canceled += OnUltimate;

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
    void handleMovement()
    {
        handleGravity();
        handleRotation();
        characterController.Move(currentMovement * characterSpeed * Time.deltaTime);
    }
    private void cooldownControl()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            if (dashCooldownCurrent > 0)
                dashCooldownCurrent -= period;

            if (ultimateCooldownCurrent > 0)
                ultimateCooldownCurrent -= period;

        }

    }


    // Update is called once per frame
    void Update()
    {
        if (!performingAction)
        {
            handleMovement();
        }

        updateUI();
        cooldownControl();
        handleActions();
    }

    private void updateUI()
    {
        updateUIAbility(dashCooldownCurrent, dashAblilityUIObject);
        updateUIAbility(ultimateCooldownCurrent, ultimateAbilityUIObject);
    }

    private void updateUIAbility(float cooldown, GameObject uiObject)
    {
        Image image = uiObject.transform.Find("Image").gameObject.GetComponent<Image>();
        GameObject coolDownObject = uiObject.transform.Find("Cooldown").gameObject;

        if (cooldown > 0)
        {
            image.fillAmount -= 1 / dashCooldownCurrent * Time.deltaTime;
            coolDownObject.SetActive(true);
            coolDownObject.transform.GetComponent<TextMeshProUGUI>().text = Mathf.Floor(dashCooldownCurrent).ToString();
        }
        else
        {
            image.fillAmount = 1;
            coolDownObject.SetActive(false);
        }

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
            if (isDodgePressed && dashCooldownCurrent <= 0)
            {
                StartCoroutine(Dash());
            }

            if (isUltimatePressed && ultimateCooldownCurrent <= 0)
            {
                StartCoroutine(Ultimate());
            }

        }
    }

    IEnumerator Ultimate()
    {
        performingAction = true;

        animator.SetBool(isUltimatingHash, true);
        yield return new WaitForSeconds(2.3f);
        animator.SetBool(isUltimatingHash, false);
        yield return new WaitForSeconds(0.2f);

        performingAction = false;
        ultimateCooldownCurrent = ultimateCooldown;
        if (dashCooldownCurrent == 0)
        {
            dashCooldownCurrent += 0.1f;
        }
    }

    IEnumerator Dash()
    {
        performingAction = true;

        Vector3 dashDir = currentMovement;
        float startTime = Time.time;
        animator.SetBool(isDodgingHash, true);
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(dashDir * dashSpeed * Time.deltaTime);
            yield return null;
        }
        animator.SetBool(isDodgingHash, false);

        performingAction = false;
        dashCooldownCurrent = dashCooldown;
    }

    IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
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
    void handleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;


        Quaternion currentRotation = transform.rotation;

        if (positionToLookAt != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }

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
