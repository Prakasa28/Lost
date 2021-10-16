using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerInventory : MonoBehaviour
{
    // placeholders
    public GameObject shieldPlaceHolder;
    public GameObject weaponPlaceHolder;

    // animator
    private Animator animator;
    // private int isPickingUpHash;

    private GroundItem followingItem;
    private List<ItemObject> items;

    void Awake()
    {
        items = new List<ItemObject>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        pickUpItem();
    }


    private void OnTriggerEnter(Collider other)
    {
        // get the item attached with ground item script
        var groundItem = other.GetComponent<GroundItem>();
        // check if i have collided with item on the ground
        if (groundItem)
        {
            followingItem = groundItem;
            Debug.Log("Entered collision: " + followingItem);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        followingItem = null;
    }

    void AddItem(ItemObject newItem)
    {
        
        // check if there's already an item with this type if there is remove it replace it with the new one
        ItemObject itemToRemove = null;
        
        foreach (ItemObject oldItem in items)
        {
            if (oldItem.type == newItem.type)
            {
                itemToRemove = oldItem;
            }
        }
        
        if (itemToRemove != null)
            items.Remove(itemToRemove);

        // add the object to my list
        items.Add(newItem);
        // equip and display item
    }


    void pickUpItem()
    {
        // check if the object is on the ground when i press it 
        if (followingItem != null && Input.GetKey(KeyCode.T))
        {
            //TODO set animation to true  
            // add the new item to my list and destroy the object from the scene
            AddItem(followingItem.item);
            Destroy(followingItem.gameObject);
            followingItem = null;
            Debug.Log("I have picked up an item!");
        }
    }

    void equipItems()
    {
    }


    void displayUI()
    {
    }
}