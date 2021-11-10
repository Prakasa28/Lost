using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AI : MonoBehaviour
{
    // range for the UI
    public float range = 5f;
    private float distance;

    // get parameter ids
    private int isWavingHash;

    // get player position
    private Transform playerPos;
    private GameObject player;
    private GameObject aiText;
    private TextMeshProUGUI aiTextPro;
    private Animator animator;
    private float textTime = 9;

    // get player position
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aiText = GameObject.FindGameObjectWithTag("AIText");
        aiTextPro = GetComponentInChildren<TextMeshProUGUI>();
        // get the player object
        playerPos = player.transform;
        // set the animator
        animator = GetComponent<Animator>();
        // get the animation 
        isWavingHash = Animator.StringToHash("IsWaving");
        aiText.SetActive(false);
        // aiText2.SetActive(false);
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
            handleText();
        }
    }

    void handleText()
    {
        aiText.SetActive(true);
        textTime -= Time.deltaTime;

        if (textTime <= 6)
        {
            aiTextPro.text = "My wife disappeared aswell..";
        }

        if (textTime <= 3)
        {
            aiTextPro.text = "You have to save us use the chest!";
        }

        if (textTime <= 1)
        {
            aiText.SetActive(false);
        }
        
    }

    // draw the range circle
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}