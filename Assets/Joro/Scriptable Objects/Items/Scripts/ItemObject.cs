using System.Data;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Food,
    Weapon,
    Shield,
    Armor,
    Default
}

public enum Attributes {
    Agility,
    Intellect,
    Stamina,
    Strength
}

public abstract class ItemObject : ScriptableObject
{

    public int Id;
   public Sprite uiDisplay;
   public ItemType type;
   [TextArea(15,20)]
   public string description;

   public ItemBuff[] buffs;

   public Item CreateItem() {
       Item newItem = new Item(this);
       return newItem;
   }

}


[System.Serializable]
public class Item
{
    public string Name;
    public int Id;
    public ItemBuff[] buffs;

    public Item(ItemObject item) {
        Name = item.name;
        Id = item.Id;
        buffs = new ItemBuff[item.buffs.Length];

        for (int i = 0; i < buffs.Length; i++)
        {
           buffs[i]  = item.buffs[i];
        }
    }

    public Item() {
        Name = "";
        Id = -1;
    }
}


[System.Serializable]
public class ItemBuff
{
    public Attributes attribute;
    public int value;

    public ItemBuff(int _value)
    {
       value = _value; 
    }
}