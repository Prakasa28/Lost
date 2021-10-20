using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class Chest : MonoBehaviour
{
    // get the children object
    private GameObject chest;
    private GameObject items;
    private GameObject chestText;
    private bool opened = false;

    void Start()
    {
        // set objects invisible
        chestText = GameObject.FindGameObjectWithTag("ChestText");
        chest = GameObject.FindGameObjectWithTag("Chest");
        items = GameObject.FindGameObjectWithTag("Items");
        items.SetActive(false); 
        chestText.SetActive(false);
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.gameObject.CompareTag("Player"))
        {
            chestText.SetActive(true);
            opened = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        opened = false;
        chestText.SetActive(false);
    }

    private void Update()
    {
        openChest();
    }


    private void openChest()
    {
        // check if chest is opened
        if (opened && Input.GetKeyDown(KeyCode.F))
        {
            // rotate chest
            chest.transform.Rotate(-90, 0, 0);
            // set items visible
            items.SetActive(true);
        }
    }
}