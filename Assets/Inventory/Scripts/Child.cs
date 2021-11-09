using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

public class Child : MonoBehaviour
{
    private GameObject player;
    public float moveSpeed = 5; //move speed
    public float followRange = 2; //follow range
    private float distance; //distance between players
    private Transform playerPos;
    private Animator animator;
    private int isFollowingHash;
    private int isDuyingHash;
    public float timeRemaining = 10;
    public GameObject portalEffect;
    private bool isSpawned = false;
    private bool canMove = true;
    private GameObject newPortal;
    private GameObject childCanvas;

    // Start is called before the first frame update
    void Start()
    {
        // get our player
        player = GameObject.FindGameObjectWithTag("Player");
        childCanvas = GameObject.FindGameObjectWithTag("ChildText");
        playerPos = player.transform;
        // set the animator
        animator = GetComponent<Animator>();
        isFollowingHash = Animator.StringToHash("IsFollowing");
        isDuyingHash = Animator.StringToHash("IsSucked");
        childCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
            moveToPlayer();

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            if (!isSpawned)
                StartCoroutine(spawnPortal());
        }
    }


    void moveToPlayer()
    {
        var playerPositionToCheck = playerPos;
        var ourPosition = transform;

        distance = Vector3.Distance(playerPositionToCheck.position, ourPosition.position);

        if (distance > followRange - 1)
        {
            //move towards the player
            transform.position =
                Vector3.MoveTowards(transform.position, playerPos.position, moveSpeed * Time.deltaTime);
            //rotate to look at the player
            transform.LookAt(playerPos);
            // set animation on
            animator.SetBool(isFollowingHash, true);
        }
        else
        {
            animator.SetBool(isFollowingHash, false);
        }
    }


    IEnumerator spawnPortal()
    {
        isSpawned = true;
        canMove = false;
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += .4f;
        newPortal = Instantiate(portalEffect, spawnPosition, Quaternion.Euler(new Vector3(90, 0, 0)));
        yield return new WaitForSeconds(1f);
        StartCoroutine(SuckPlayer());
    }

    IEnumerator SuckPlayer()
    {
        float suckingTime = 10;
        animator.SetBool(isFollowingHash, false);
        animator.SetBool(isDuyingHash, true);
        while (true)
        {
            if (suckingTime <= 0)
                break;
            suckingTime -= Time.deltaTime;
            transform.position += new Vector3(0, -0.2f, 0) * Time.deltaTime;
            childCanvas.SetActive(true);
            yield return null;
        }
        childCanvas.SetActive(false);
        Destroy(gameObject);
        Destroy(newPortal);
        animator.SetBool(isDuyingHash, false);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}