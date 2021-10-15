using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAI : MonoBehaviour
{
    // range for the UI
    public float range = 5f;
    private float distance;

    // get parameter ids
    private int isWavingHash;

    // get player position
    private Transform playerPos;
    public Player player;
    private Animator animator;

    // get player position
    void Start()
    {
        // get the player object
        playerPos = player.transform;
        // set the animator
        animator = GetComponent<Animator>();
        // get the animation 
        isWavingHash = Animator.StringToHash("IsWaving");
    }

    void Update()
    {
        handleAnimation();
    }

    void handleAnimation()
    {
        distance = Vector3.Distance(playerPos.position, transform.position);
        // check if player has entered ui range
        if (distance < range)
        {
            animator.SetBool(isWavingHash, true);
        }
    }

    // draw the range circle
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}