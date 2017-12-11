using System;
using LitJson;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Item) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = ID;
            hashCode = (hashCode*397) ^ (ItemName != null ? ItemName.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ MaxStack;
            hashCode = (hashCode*397) ^ (Description != null ? Description.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ Stackable.GetHashCode();
            hashCode = (hashCode*397) ^ (GetSprite() != null ? GetSprite().GetHashCode() : 0);
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

    public Item(int id, string title,int maxStack,string description,bool stackable)
    {
        ID = id;
        ItemName = title;
        MaxStack = maxStack;
        Description = description;
        Stackable = stackable;
    }
    public Item(int id, string title, int maxStack, string description, bool stackable,int instanceID)
    {
        ID = id;
        ItemName = title;
        MaxStack = maxStack;
        Description = description;
        Stackable = stackable;
    }

    public override string ToString()
    {
        string output = "\n-------Item print------\nID: " + ID +
                        " Item name: " + ItemName +
                        " MaxStack: " + MaxStack +
                        " Description: " + Description +
                        " Stackable: " + Stackable;
        return output;
    }
}