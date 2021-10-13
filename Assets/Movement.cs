using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController controller;
    // Start is called before the first frame update
    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocty;


    // Update is called once per frame
    void Update()
    {

        handleGravity();
        float vertiacal = Input.GetAxisRaw("Horizontal");
        float horizontal = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(-horizontal, 0f, vertiacal).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocty, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            controller.Move(direction * speed * Time.deltaTime);
        }
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

}
