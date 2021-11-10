using System;
using System.Collections;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class Chest : MonoBehaviour
{
    // get the children object
    private GameObject chest;

    // private GameObject childChest;
    public GameObject items;
    private GameObject chestText;
    private bool collisionOccured = false;
    private ParticleSystem chestEffectParticleSystem;
    private GameObject chestEffect;
    private Animator animator;
    private int isOpeningHash;
    private GameObject player;
    private MovementController characterController;
    private InputHandler inputHandler;
    void Start()
    {
        chestText = GameObject.FindGameObjectWithTag("ChestText");
        player = GameObject.FindGameObjectWithTag("Player");
        animator = player.GetComponent<Animator>();
        inputHandler = player.GetComponent<InputHandler>();
        characterController = player.GetComponent<MovementController>();
        chest = GameObject.FindGameObjectWithTag("Chest");
        isOpeningHash = Animator.StringToHash("IsOpening");
        chestEffect = GameObject.FindGameObjectWithTag("ChestEffect");
        chestEffectParticleSystem = chestEffect.GetComponent<ParticleSystem>();
        items.SetActive(false);
        chestText.SetActive(false);
        chestEffectParticleSystem.Play();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (collisionOccured)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            chestText.SetActive(true);
            collisionOccured = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collisionOccured = false;
        chestText.SetActive(false);
    }

    private void Update()
    {
        openChest();
    }


    private void openChest()
    {
        // check if chest is opened
        if (collisionOccured && inputHandler.isOpening)
        {
            chest.transform.Rotate(-120, 0, 0);
            StartCoroutine(handleAnimation());
        }
    }

    IEnumerator handleAnimation()
    {
        animator.SetBool(isOpeningHash, true);
        characterController.enabled = false;
        yield return new WaitForSeconds(0.5f);
        //disable character controller
        // rotate chest
        // stop the particles
        Destroy(chestEffectParticleSystem, 0.5f);
        // set items visible
        items.SetActive(true);
        chestText.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool(isOpeningHash, false);
        //enable character controller
        characterController.enabled = true;
    }
}