using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // create list of items collected
    public List<GameObject> items;
    public GameObject item;

    private void Start()
    {
        items = new List<GameObject>();
    }


    // check if i have collided with an item
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.gameObject.name == item.name)
        {
            // if i have collided add the item to my list and destroy the object from the map
            
        }
    }
    // if i have collided add the item to my list and destroy the object from the map
}