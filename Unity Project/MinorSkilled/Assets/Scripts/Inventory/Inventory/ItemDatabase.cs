using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using LitJson;

public class ItemDatabase
{
    public List<Item> items = new List<Item>();

    public ItemDatabase()
    {
        ConstructItemDatabase();
    }

    /// <summary>
    /// Returns item by id, returns null if item id doesn't exist
    /// </summary>
    /// <param name="id">Item id</param>
    /// <returns></returns>
    public Item FetchItemByID(int id)
    {
        foreach (Item item in items)
            if (item.ID == id) return item;
        Debug.LogWarning("Item couldn't be found");
        return null;
    }

    public void ConstructItemDatabase()
    {
        string filename = "Items.json";
        string jsonText = "";

        if (!File.Exists(Application.dataPath + "/StreamingAssets/" + filename))
            File.CreateText(Application.dataPath + "/StreamingAssets/" + filename);
        else
        {
            try
            {
                string[] jsonLines = File.ReadAllLines(Application.dataPath + "/StreamingAssets/" + filename);
                foreach (string line in jsonLines) jsonText += line;

                if (jsonText == "")
                {
                    Debug.LogWarning("Json file is empty");
                    return;
                }

            }
            catch (Exception exception)
            {
                Debug.Log("Exception: " + exception);
            }
        }

        items = JsonMapper.ToObject<List<Item>>(jsonText) ?? new List<Item>();
    }

    public void Save()
    {
        //Save item database
        if (items == null)
            items = new List<Item>();
        string itemJson = JsonMapper.ToJson(items);
        if (!File.Exists(Application.dataPath + "/StreamingAssets/Items.json"))
            File.CreateText(Application.dataPath + "/StreamingAssets/Items.json");
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Items.json", itemJson);
    }
}
