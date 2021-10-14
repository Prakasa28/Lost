using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementv2Controller : MonoBehaviour
{
    PlayerInputV2 playerInput;
    CharacterController characterController;
    Animator animator;

    public float characterSpeed = 15;
    public float dashTime = .6f;
    public float dashSpeed = 25;
    public float dashCooldown = 1.5f;
    public float rotationFactorPerFrame = 30.0f;

    int isMovingHash;
    int isDodgingHash;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;
    bool isDodgePressed;
    bool canDash = true;
    bool canMove = true;

    void Awake()
    {
        playerInput = new PlayerInputV2();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isMovingHash = Animator.StringToHash("IsMoving");
        isDodgingHash = Animator.StringToHash("IsDodging");

        playerInput.CharacterControlls.Move.started += onMovementInput;
        playerInput.CharacterControlls.Move.canceled += onMovementInput;
        playerInput.CharacterControlls.Move.performed += onMovementInput;
        playerInput.CharacterControlls.Dodge.started += OnDodge;
        playerInput.CharacterControlls.Dodge.canceled += OnDodge;
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

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            handleGravity();
            handleAnimation();
            handleRotation();
            characterController.Move(currentMovement * characterSpeed * Time.deltaTime);
        }
    }


    void handleAnimation()
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

        if (isDodgePressed)
        {
            //make dodge 
            if (canDash)
            {
                canDash = false;
                canMove = false;
                StartCoroutine(Dash());
                // Debug.Log("sled");
                StartCoroutine(Cooldown(dashCooldown));
            }

        }
    }

    IEnumerator Dash()
    {
        Vector3 dashDir = currentMovement;
        float startTime = Time.time;
        animator.SetBool(isDodgingHash, true);
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(dashDir * dashSpeed * Time.deltaTime);
            yield return null;
        }
        animator.SetBool(isDodgingHash, false);
        canMove = true;
    }


    IEnumerator Cooldown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canDash = true;
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
