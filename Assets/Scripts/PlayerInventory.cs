using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private GameObject shield;
    private GameObject armor;
    private GameObject weapon;


    public void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<GroundItem>();
        // if i have collided with the item assign it to my object and destroy it from the map
        Debug.Log("colided with: " + item);
    }
}