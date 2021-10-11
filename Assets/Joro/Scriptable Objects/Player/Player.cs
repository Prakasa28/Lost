using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;
    public Attribute[] attributes;

    private Transform weapon;
    private Transform shield;

    public Mesh unArmoredMesh;
    public Mesh armoredMesh;
    public Transform weaponTransform;
    public Transform shieldTransform;

    private void Start()
    {
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }

        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnRemoveItem;
            equipment.GetSlots[i].OnAfterUpdate += OnAddItem;
        }
    }

    public void OnRemoveItem(InventorySlot _slot)
    {
        if (_slot.itemObject == null)
        {
            return;
        }

        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equpment:
                print(string.Concat("Removed ", _slot.itemObject, " on ", _slot.parent.inventory.type, ", Allowed Items:", string.Join(", ", _slot.AllowedItems)));

                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                        {
                            attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
                        }
                    }
                }

                switch (_slot.AllowedItems[0])
                {
                    case ItemType.Weapon:
                        if (_slot.itemObject.characterDisplay != null)
                        {
                            Destroy(weapon.gameObject);
                        }
                        break;
                    case ItemType.Shield:
                        if (_slot.itemObject.characterDisplay != null)
                        {
                            Destroy(shield.gameObject);
                        }
                        break;
                    case ItemType.Armor:
                        var skinnedMeshedRenderer = this.GetComponentInChildren<SkinnedMeshRenderer>();
                        skinnedMeshedRenderer.sharedMesh = unArmoredMesh;
                        break;
                }


                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }

    public void OnAddItem(InventorySlot _slot)
    {

        if (_slot.itemObject == null)
        {
            return;
        }

        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equpment:
                print(string.Concat("Placed ", _slot.itemObject, " on ", _slot.parent.inventory.type, ", Allowed Items:", string.Join(", ", _slot.AllowedItems)));

                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                        {
                            attributes[j].value.AddModifier(_slot.item.buffs[i]);
                        }
                    }
                }

                switch (_slot.AllowedItems[0])
                {
                    case ItemType.Weapon:
                        if (_slot.itemObject.characterDisplay != null)
                        {
                            weapon = Instantiate(_slot.itemObject.characterDisplay, weaponTransform).transform;
                        }
                        break;
                    case ItemType.Shield:

                        if (_slot.itemObject.characterDisplay != null)
                        {
                            shield = Instantiate(_slot.itemObject.characterDisplay, shieldTransform).transform;
                        }
                        break;
                    case ItemType.Armor:
                        //TODO switch whole armor
                        var skinnedMeshedRenderer = this.GetComponentInChildren<SkinnedMeshRenderer>();
                        skinnedMeshedRenderer.sharedMesh = armoredMesh;
                        break;
                }

                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }

    }


    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            Item _item = new Item(item.item);
            if (inventory.AddItem(_item, 1))
            {
                Destroy(other.gameObject);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
            equipment.Save();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            inventory.Load();
            equipment.Load();
        }

    }
    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, " was updated! Value is now: ", attribute.value.ModifiedValue));
    }

    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized]
    public Player parent;
    public Attributes type;
    public ModifiableInt value;
    public void SetParent(Player _player)
    {
        parent = _player;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}
