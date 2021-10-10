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
        private int isMovingHash;
        private int isDodgingHash;
        private int isAttackingHash;

        // store player input   
        private PlayerInput input;
        private CharacterController characterController;
        private Vector2 currentMovementInput;
        private Vector3 currentMovement;

        private bool isMovementPressed;
        private float rotationFactorPerFrame = 15.0f;

        public float movementSpeed;
        public float dashDistance = 15;
        private bool isAttacking = false;
        private bool isDodging = false;
  
        [SerializeField] private float animationFinishedTime = 0.6f;
        
        void Awake()
        {
            animator = GetComponent<Animator>();
            input = new PlayerInput();
            characterController = GetComponent<CharacterController>();


            //animations
            isMovingHash = Animator.StringToHash("IsMoving");
            isDodgingHash = Animator.StringToHash("IsDodging");
            isAttackingHash = Animator.StringToHash("IsAttacking");

            //moving
            input.CharacterControls.Move.performed += onMovementInput;

            //dodging
            input.CharacterControls.Dodge.performed += onDodge;

            //attacking
            input.CharacterControls.Attack.performed += onAttack;
        }

        void onMovementInput(InputAction.CallbackContext context)
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x * movementSpeed;
            currentMovement.z = currentMovementInput.y * movementSpeed;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        }


        void onDodge(InputAction.CallbackContext context)
        {
            if (!isDodging)
            {
                animator.SetTrigger(isDodgingHash);
                StartCoroutine(InitialiseDodge());
            }
        }

        private void onAttack(InputAction.CallbackContext context)
        {
            if (!isAttacking)
            {
                animator.SetTrigger(isAttackingHash);
                StartCoroutine(InitialiseAttack());
            }
        }

        IEnumerator InitialiseAttack()
        {
            yield return new WaitForSeconds(0.1f);
            isAttacking = true;
        }

        IEnumerator InitialiseDodge()
        {
            yield return new WaitForSeconds(0.1f);
            isDodging = true;
        }
        
        private void Update()
        {
            handleGravity();
            handleRotation();
            handleAnimation();


            if (isAttacking)
            {
                if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinishedTime)
                {
                    isAttacking = false; 
                }
            }
            
            if (isDodging)
            {
                if (animator.GetCurrentAnimatorStateInfo(2).normalizedTime >= animationFinishedTime)
                {
                    isDodging = false; 
                }
                characterController.Move(currentMovement * dashDistance * Time.deltaTime);    
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
            }

            float gravity = -9.8f;
            currentMovement.y += gravity;
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
            // bool isRunning = animator.GetBool(isRunningHash);
            bool isMoving = animator.GetBool(isMovingHash);

            // start moving if movement pressed is true and not already walking
            if (isMovementPressed && !isMoving)
            {
                animator.SetBool(isMovingHash, true);
            }

            // stop moving if button is released and already walking 
            if (!isMovementPressed && isMoving)
            {
                animator.SetBool(isMovingHash, false);
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