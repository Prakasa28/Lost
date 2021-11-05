using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class Chest : MonoBehaviour
{
    // get the children object
    // private GameObject chest;
    private GameObject childChest;
    private GameObject items;
    private GameObject chestText;
    private bool opened = false;

    void Start()
    {
        // set objects invisible
        chestText = GameObject.FindGameObjectWithTag("ChestText");
        // chest = GameObject.FindGameObjectWithTag("Chest");
        childChest = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        items = GameObject.FindGameObjectWithTag("Items");
        items.SetActive(false);
        chestText.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided with player");
            chestText.SetActive(true);
            opened = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        opened = false;
        chestText.SetActive(false);
        Debug.Log("Exited");
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
            // chest.transform.Rotate(-90, 0, 0);
            childChest.transform.Rotate(-90, 0, 0);
            // set items visible
            items.SetActive(true);
        }
    }
}