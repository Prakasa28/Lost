using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{

    InputHandler inputHandler;
    Vector3 moveDirection;
    CharacterController characterController;

    [HideInInspector]
    public Transform myTransform;
    [HideInInspector]
    public AnimatorHandler animatorHandler;



    [Header("Stats")]
    [SerializeField]
    float movementSpeed = 5;
    [SerializeField]
    float rotationSpeed = 10;

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        characterController = GetComponent<CharacterController>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        myTransform = transform;
        animatorHandler.Initialize();
    }

    public void Update()
    {
        float delta = Time.deltaTime;

        inputHandler.TickInput(delta);

        characterController.Move(inputHandler.currentMovement * movementSpeed * Time.deltaTime);

        animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

        if (animatorHandler.canRotate)
        {
            HandleRotation(delta);
        }

    }



    private void HandleRotation(float delta)
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = inputHandler.horizontal;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = inputHandler.vertical;


        Quaternion currentRotation = transform.rotation;

        if (positionToLookAt != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


}
