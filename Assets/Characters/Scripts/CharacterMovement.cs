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

        // store player input   
        private PlayerInput input;
        private CharacterController characterController;
        private Vector2 currentMovementInput;
        private Vector3 currentMovement;
        private Vector3 currentRunMovement;

        private bool isMovementPressed;
        private bool isRunPressed;

        private float rotationFactorPerFrame = 15.0f;


        public float walkSpeed;
        public float runSpeed;
        
        void Awake()
        {
            //initialise variables
            animator = GetComponent<Animator>();
            input = new PlayerInput();
            characterController = GetComponent<CharacterController>();

            //animations
            isWalkingHash = Animator.StringToHash("IsWalking");
            isRunningHash = Animator.StringToHash("IsRunning");

            //moving
            input.CharacterControls.Move.started += onMovementInput;
            input.CharacterControls.Move.canceled += onMovementInput;

            //running
            input.CharacterControls.Run.started += onRun;
            input.CharacterControls.Run.canceled += onRun;

            //TODO - dodge
            

            //TODO - attacking
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
        

        private void Update()
        {
            handleGravity();
            handleRotation();
            handleAnimation();

            if (isRunPressed)
            {
                characterController.Move(currentRunMovement * Time.deltaTime);
            }

            characterController.Move(currentMovement * Time.deltaTime);
        }

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

            // start walking if movement pressed is true and not already walking
            if (isMovementPressed && !isWalking)
            {
                animator.SetBool(isWalkingHash, true);
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