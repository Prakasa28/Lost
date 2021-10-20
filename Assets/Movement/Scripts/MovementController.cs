using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MovementController : MonoBehaviour
{
    InputHandler inputHandler;
    CharacterController characterController;
    Animator animator;
    AbilitiesController abilities;

    public float characterSpeed = 15;
    public float rotationFactorPerFrame = 30.0f;

    int isMovingHash;
    [HideInInspector]
    public bool canMove = true;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        inputHandler = GetComponent<InputHandler>();
        abilities = GetComponent<AbilitiesController>();
        isMovingHash = Animator.StringToHash("IsMoving");
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            handleMovement();
        }

        handleActions();
    }

    void handleMovement()
    {
        handleGravity();
        handleRotation();
        characterController.Move(inputHandler.currentMovement * characterSpeed * Time.deltaTime);
    }

    public void handleRotation(float rotationSpeed = 0)
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





}
