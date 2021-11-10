using System;
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

    void Start()
    {
        chestText = GameObject.FindGameObjectWithTag("ChestText");
        chest = GameObject.FindGameObjectWithTag("Chest");
        animator = GetComponent<Animator>();
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
        if (collisionOccured && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetBool(isOpeningHash, true);
            // rotate chest
            chest.transform.Rotate(-90, 0, 0);
            // stop the particles
            Destroy(chestEffectParticleSystem, 0.5f);
            // set items visible
            items.SetActive(true);
            chestText.SetActive(false);
        }
    }
}