using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationController : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
       animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool playerAttacking = animator.GetBool("playerAttacking");
        bool attack = Input.GetKey("f");

        if (!playerAttacking && attack)
        {
           animator.SetBool("playerAttacking", true);
        }
        if (playerAttacking && !attack)
        {
            animator.SetBool("playerAttacking", false);
        }
    }
}
