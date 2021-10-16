using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class Chest : MonoBehaviour
{
    // get the children object
    public GameObject chest;
    public GameObject player;
    public GameObject items;
    public GameObject text;
    private bool opened = false;

    void Start()
    {
        // set objects invisible
        items.SetActive(false);
        text.SetActive(false);
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.gameObject.name == player.name)
        {
            text.SetActive(true);
            opened = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        opened = false;
        text.SetActive(false);
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