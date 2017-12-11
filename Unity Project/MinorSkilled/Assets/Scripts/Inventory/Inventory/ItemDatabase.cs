using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;

public class ItemDatabase
{
    public List<Blueprint> blueprints = new List<Blueprint>();
    public List<Item> items = new List<Item>();

    public ItemDatabase()
    {
        ConstructItemDatabase();
        ConstructBlueprintDatabase();
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

    /// <summary>
    /// Returns item by item name, returns null if item name doesn't exist
    /// </summary>
    /// <param name="name">Name of the item</param>
    /// <returns></returns>
    public Item FetchItemByName(string name)
    {
        foreach (Item item in items)
            if (item.ItemName == name) return item;
        Debug.LogWarning("Item couldn't be found");
        return null;
    }

    public void ConstructItemDatabase()
    {
        ConstructDatabase("Items.json");
    }

    public void ConstructBlueprintDatabase()
    {
        ConstructDatabase("Blueprints.json");
    }

    private void ConstructDatabase(string filename)
    {
        string jsonText = "";

        if (!File.Exists(Application.dataPath + "/StreamingAssets/" + filename))
            File.CreateText(Application.dataPath + "/StreamingAssets/" + filename);

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
            Debug.Log("Exception: "+exception);
        }

        if (filename == "Items.json")
            items = JsonMapper.ToObject<List<Item>>(jsonText);
        else
            blueprints = JsonMapper.ToObject<List<Blueprint>>(jsonText);
    }

    public void Save()
    {
        //Save item database
        string itemJson = JsonMapper.ToJson(items);
        if (!File.Exists(Application.dataPath + "/StreamingAssets/Items.json"))
            File.CreateText(Application.dataPath + "/StreamingAssets/Items.json");
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Items.json", itemJson);

        //Save blueprints database
        string blueprintJson = JsonMapper.ToJson(blueprints);
        if (!File.Exists(Application.dataPath + "/StreamingAssets/Blueprints.json"))
            File.CreateText(Application.dataPath + "/StreamingAssets/Blueprints.json");
        File.WriteAllText(Application.dataPath + "/StreamingAssets/Blueprints.json", blueprintJson);
        //Repetition can be prevented by having database states but for only two databases there are no real benefits
    }

    public override string ToString()
    {
        string output =
            "---------- Item database ---------\n"+
            items.Aggregate("", (current, item) => current + item.ToString()) +
            "\n\n---------- Crafting database ---------\n"+

            blueprints.Aggregate("", (current, blueprint) => current + blueprint.ToString());
        return output;
    }
}
