using System;
using Newtonsoft.Json;
using UnityEngine;
public enum ItemType { Consumable, Weapon, Misc }

[Serializable]
public class Item
{
    protected bool Equals(Item other)
    {
        return ID == other.ID && string.Equals(ItemName, other.ItemName) && MaxStack == other.MaxStack && string.Equals(Description, other.Description) && Stackable == other.Stackable && Equals(GetSprite(), other.GetSprite());
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Item) obj);
    }

    //Needed coz we override Equals
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = ID;
            hashCode = (hashCode * 397) ^ (ItemName != null ? ItemName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ MaxStack;
            hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Stackable.GetHashCode();
            hashCode = (hashCode * 397) ^ CrateDrop.GetHashCode();
            hashCode = (hashCode * 397) ^ (SpriteFilePath != null ? SpriteFilePath.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (int) type;
            return hashCode;
        }
    }


    public int ID;
    public string ItemName;
    public int MaxStack;
    public string Description;
    public bool Stackable;
    public bool CrateDrop;
    public string SpriteFilePath;
    public ItemType type;

    public Sprite GetSprite()
    {
        if (Resources.Load<Sprite>(SpriteFilePath) == null || SpriteFilePath == "")
        {
            Debug.Log("SpriteFilePath: " + SpriteFilePath);
            Debug.LogWarning("Can't load item sprite: " + SpriteFilePath);
            return null;
        }
        return Resources.Load<Sprite>(SpriteFilePath);
    }

    public Item()
    {
        ID = -1;
    }

    public override string ToString()
    {
        string output = "\n-------Item print------\nID: " + ID +
                        " Item name: " + ItemName +
                        " MaxStack: " + MaxStack +
                        " Description: " + Description +
                        " Stackable: " + Stackable +
                        " SpriteFilePath: " + SpriteFilePath;
        return output;
    }
}