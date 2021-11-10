using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerInventory : MonoBehaviour
{
    // placeholders
    private Transform shieldPlaceHolder;
    private Transform weaponPlaceHolder;

    // show text when picking up item
    private GameObject itemsText;

    // animator    
    private Animator animator;
    private int isPickingUpHash;

    private GroundItem followingItem;
    private List<ItemObject> items;
    public Mesh armoredMesh;
    private MovementController characterController;

    void Awake()
    {
        characterController = GetComponent<MovementController>();
        items = new List<ItemObject>();
        animator = GetComponent<Animator>();
        isPickingUpHash = Animator.StringToHash("IsPickingUp");
        weaponPlaceHolder = GameObject.FindGameObjectWithTag("Axe").transform;
        shieldPlaceHolder = GameObject.FindGameObjectWithTag("Shield").transform;
        itemsText = GameObject.FindGameObjectWithTag("ItemText");
        itemsText.SetActive(false);
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
            itemsText.SetActive(true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        followingItem = null;
        itemsText.SetActive(false);
    }

    void addItem(ItemObject newItem)
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
    }


    void pickUpItem()
    {
        // check if the object is on the ground when i press it 
        if (followingItem != null && Input.GetKey(KeyCode.E))
        {
            StartCoroutine(handleAnimation(followingItem));
        }
    }

    IEnumerator handleAnimation(GroundItem weapon)
    {
        animator.SetBool(isPickingUpHash, true);
        characterController.enabled = false;
        yield return new WaitForSeconds(0.5f);
        // add the new item to my list and then destroy it from the map
        addItem(weapon.item);
        // equip the item
        equipItems(weapon.item);
        if (weapon != null)
        {
            Destroy(weapon.gameObject);
        }

        weapon = null;
        itemsText.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool(isPickingUpHash, false);
        characterController.enabled = true;
    }

    void equipItems(ItemObject item)
    {
        if (item.type == ItemType.Weapon)
        {
            foreach (Transform child in weaponPlaceHolder)
            {
                Destroy(child.gameObject);
            }

            // set the weapon
            Instantiate(item.characterDisplay.gameObject, weaponPlaceHolder);
        }

        if (item.type == ItemType.Shield)
        {
            foreach (Transform child in shieldPlaceHolder)
            {
                Destroy(child.gameObject);
            }

            // set the shield
            Instantiate(item.characterDisplay, shieldPlaceHolder);
        }


        if (item.type == ItemType.Armor)
        {
            // set the armor 
            var skinnedMeshedRenderer = this.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshedRenderer.sharedMesh = armoredMesh;
        }
    }
}