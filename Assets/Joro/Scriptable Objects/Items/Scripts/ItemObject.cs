using System.Data;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Food,
    Weapon,
    Shield,
    Armor,
    Default
}

public enum Attributes
{
    Stamina,
    Strength,
    Defense,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Item")]
public class ItemObject : ScriptableObject
{
    public Sprite uiDisplay;
    public ItemType type;
    public bool stackable; 
    [TextArea(15, 20)]
    public string description;
    public Item data = new Item();
    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }

}


[System.Serializable]
public class Item
{
    public string Name;
    public int Id = -1;
    public ItemBuff[] buffs;

    public Item(ItemObject item)
    {
        Name = item.name;
        Id = item.data.Id;
        buffs = new ItemBuff[item.data.buffs.Length];

        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = item.data.buffs[i];
        }
    }

    public Item()
    {
        Name = "";
        Id = -1;
    }
}


[System.Serializable]
public class ItemBuff : IModifier
{
    public Attributes attribute;
    public int value;

    public ItemBuff(int _value)
    {
        value = _value;
    }

    public void AddValue(ref int baseValue)
    {
        baseValue += value;
    }
}