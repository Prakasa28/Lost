using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [HideInInspector] public Vector2 currentMovementInput;
    [HideInInspector] public Vector3 currentMovement;
    private PlayerInputV2 playerInput;

    [HideInInspector] public bool isMovementPressed;

    [HideInInspector] public bool isDodgePressed;

    [HideInInspector] public bool isAttackedPressed;

    [HideInInspector] public bool isChargePressed;

    [HideInInspector] public bool isUltimatePressed;

    [HideInInspector] public bool isChangingEnemy;

    [HideInInspector] public bool isPickingUp;

    [HideInInspector] public bool isOpening;


    void Awake()
    {
        playerInput = new PlayerInputV2();

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
        playerInput.CharacterControlls.ChangeEnemy.started += OnEnemyChange;
        playerInput.CharacterControlls.ChangeEnemy.canceled += OnEnemyChange;
        playerInput.CharacterControlls.PickUp.performed += OnPickUp;
        playerInput.CharacterControlls.PickUp.canceled += OnPickUp;
        playerInput.CharacterControlls.Open.performed += OnOpen;
        playerInput.CharacterControlls.Open.canceled += OnOpen;
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

    void OnEnemyChange(InputAction.CallbackContext context)
    {
        isChangingEnemy = context.ReadValueAsButton();
    }

    void OnPickUp(InputAction.CallbackContext context)
    {
        isPickingUp = context.ReadValueAsButton();
    }

    void OnOpen(InputAction.CallbackContext context)
    {
        isOpening = context.ReadValueAsButton();
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