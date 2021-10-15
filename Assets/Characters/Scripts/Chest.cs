using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class Chest : MonoBehaviour
{
    // get the children object
    public GameObject chest;
    public Player player;
    public ParticleSystem particles;
    public GameObject items;
    public GameObject text;
    private bool opened = false;
    public GameObject backgroundText;

    void Start()
    {
        // set objects invisible
        items.SetActive(false);
        text.SetActive(false);
        backgroundText.SetActive(false);
        particles.Stop();
    }


    private void OnCollisionEnter(Collision collision)
    {
        // check if it has collided with the children object
        if (collision.transform.gameObject.name == player.name)
        {
            text.SetActive(true);
            backgroundText.SetActive(true);
            opened = true;
        }
    }

    private void OnCollisionExit()
    {
        opened = false;
        text.SetActive(false);
        backgroundText.SetActive(false);
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
            particles.Play();
        }
    }
}