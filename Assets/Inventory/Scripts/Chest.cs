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
    private bool opened = false;
    private bool collisonOccured = false;

    void Start()
    {
        chestText = GameObject.FindGameObjectWithTag("ChestText");
        chest = GameObject.FindGameObjectWithTag("Chest");
        items.SetActive(false);
        chestText.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (collisonOccured)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            chestText.SetActive(true);
            opened = true;
            collisonOccured = true;
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
            // chest.transform.Rotate(-90, 0, 0);
            chest.transform.Rotate(-90, 0, 0);
            // set items visible
            items.SetActive(true);
            chestText.SetActive(false);
        }
    }
}