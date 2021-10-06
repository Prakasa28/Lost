using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Scripts
{
    public class CharacterMovement : MonoBehaviour
    {
        // character animator component
        private Animator animator;

        // getter/setter parameter IDs
        private int isWalkingHash;
        private int isRunningHash;
        private int isDodgingHash;
        private int isAttackingHash;

        // store player input   
        private PlayerInput input;
        private CharacterController characterController;
        private Vector2 currentMovementInput;
        private Vector3 currentMovement;
        private Vector3 currentRunMovement;

        private bool isMovementPressed;
        private bool isRunPressed;
        private bool isDodgePressed;
        private bool isAttackPressed;


        private float rotationFactorPerFrame = 15.0f;

        public float dashSpeed;
        public float dashTime;
        public float walkSpeed;
        public float runSpeed;

        void Awake()
        {
            animator = GetComponent<Animator>();
            input = new PlayerInput();
            characterController = GetComponent<CharacterController>();

            //animations
            isWalkingHash = Animator.StringToHash("IsWalking");
            isRunningHash = Animator.StringToHash("IsRunning");
            isDodgingHash = Animator.StringToHash("IsDodging");
            isAttackingHash = Animator.StringToHash("IsFighting");

            //moving
            input.CharacterControls.Move.started += onMovementInput;
            input.CharacterControls.Move.canceled += onMovementInput;

            //running
            input.CharacterControls.Run.started += onRun;
            input.CharacterControls.Run.canceled += onRun;

            //dodge
            input.CharacterControls.Dodge.started += onDodge;
            input.CharacterControls.Dodge.canceled += onDodge;


            //attacking
            input.CharacterControls.Attack.started += onAttack;
            input.CharacterControls.Attack.canceled += onAttack;
        }

        void onMovementInput(InputAction.CallbackContext context)
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x * walkSpeed;
            currentMovement.z = currentMovementInput.y * walkSpeed;
            currentRunMovement.x = currentMovementInput.x * runSpeed;
            currentRunMovement.z = currentMovementInput.y * runSpeed;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        }

        void onRun(InputAction.CallbackContext context)
        {
            isRunPressed = context.ReadValueAsButton();
        }

        void onDodge(InputAction.CallbackContext context)
        {
            isDodgePressed = context.ReadValueAsButton();
        }

        void onAttack(InputAction.CallbackContext context)
        {
            isAttackPressed = context.ReadValueAsButton();
        }


        private void Update()
        {
            handleGravity();
            handleRotation();
            handleAnimation();

            if (isRunPressed)
            {
                characterController.Move(currentRunMovement * Time.deltaTime);
            }

            // if (isDodgePressed)
            // {
            //     StartCoroutine(Dash());
            // }

            characterController.Move(currentMovement * Time.deltaTime);
        }


        // IEnumerator Dash()
        // {
        //     float startTime = Time.time;
        //     while (Time.time < startTime + dashTime)
        //     {
        //         characterController.Move(currentMovement * dashSpeed * Time.deltaTime);
        //         yield return null;
        //     }
        // }

        void handleGravity()
        {
            // apply proper gravity depending if the character is grounded or not
            if (characterController.isGrounded)
            {
                float groundedGravity = -.05f;
                currentMovement.y = groundedGravity;
                currentRunMovement.y = groundedGravity;
            }

            float gravity = -9.8f;
            currentMovement.y += gravity;
            currentRunMovement.y += gravity;
        }

        void handleRotation()
        {
            Vector3 positionToLookAt;

            // change in position our character should point to
            positionToLookAt.x = currentMovement.x;
            positionToLookAt.y = 0.0f;
            positionToLookAt.z = currentMovement.z;

            // current rotation of character
            Quaternion currentRotation = transform.rotation;


            if (isMovementPressed)
            {
                // create a new rotation based on where the player is currently pressing
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                transform.rotation =
                    Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
            }
        }


        void handleAnimation()
        {
            bool isRunning = animator.GetBool(isRunningHash);
            bool isWalking = animator.GetBool(isWalkingHash);
            bool isDodging = animator.GetBool(isDodgingHash);
            bool isAttacking = animator.GetBool(isAttackingHash);

            // start walking if movement pressed is true and not already walking
            if (isMovementPressed && !isWalking)
            {
                animator.SetBool(isWalkingHash, true);
            }

            // start attacking if not already attacking
            if (isAttackPressed && !isAttacking)
            {
                animator.SetBool(isAttackingHash, true);
            }

            // stop attacking if already attacking
            if (!isAttackPressed && isAttacking)
            {
                animator.SetBool(isAttackingHash, false);
            }

            // start dodging if not already dodging
            if (isDodgePressed && !isDodging)
            {
                animator.SetBool(isDodgingHash, true);
                
            }

            // stop dodging if already dodging
            if (!isDodgePressed && isDodging)
            {
                animator.SetBool(isDodgingHash, false);
            }


            // stop walking if button is released and already walking 
            if (!isMovementPressed && isWalking)
            {
                animator.SetBool(isWalkingHash, false);
            }


            // start running if movement pressed and run is pressed is true and not already running
            if ((isMovementPressed && isRunPressed) && !isRunning)
            {
                animator.SetBool(isRunningHash, true);
            }

            // stop running if movement or run pressed are false and currently running
            if ((!isMovementPressed || !isRunPressed) && isRunning)
            {
                animator.SetBool(isRunningHash, false);
            }
        }

        // enable player actions map
        private void OnEnable()
        {
            input.CharacterControls.Enable();
        }

        // disable player actions map
        private void OnDisable()
        {
            input.CharacterControls.Disable();
        }
    }
}