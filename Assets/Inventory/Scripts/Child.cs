using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Child : MonoBehaviour
{
    private GameObject player;
    public float moveSpeed = 5; //move speed
    private Transform playerPos;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // get our player
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.transform;
        // set the animator
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveToPlayer();
    }


    void moveToPlayer()
    {
        //move towards the player
        transform.position = Vector3.MoveTowards(transform.position, playerPos.position, moveSpeed * Time.deltaTime);
        //rotate to look at the player
        transform.LookAt(playerPos);
    }
}