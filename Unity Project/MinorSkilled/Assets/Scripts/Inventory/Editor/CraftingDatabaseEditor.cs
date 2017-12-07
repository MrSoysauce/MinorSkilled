using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CraftingDatabaseEditor : EditorWindow
{
    private int selectionGridInt;
    private const int ingredientAmount = 3;
    private Blueprint addBlueprint;
    private ItemDatabase itemDatabase;
    private List<Ingredient> ingredients;
    private Vector2 scrollPosition;
    private readonly List<bool> manageBlueprints = new List<bool>();

    private string[] ItemNames
    {
        get { return GetItemNames(true); }
    }

    [MenuItem("Inventory manager/Open Crafting Manager %q")]
    public static void ShowWindow()
    {
        GetWindow<CraftingDatabaseEditor>(false, "Crafting manager");
    }

    void OnEnable()
    {
        itemDatabase = new ItemDatabase();

        ingredients = new List<Ingredient>();
        addBlueprint = new Blueprint();
        for (int i = 0; i < ingredientAmount; i++)
            ingredients.Add(new Ingredient());
    }

    void OnGUI()
    {
        try
        {
            string[] selStrings = { "Add blueprints", "Edit blueprints" };
            selectionGridInt = GUILayout.SelectionGrid(selectionGridInt, selStrings, 2);
            if (selectionGridInt == 0) OnAddBlueprint();
            else OnEditBlueprints();
        }
        catch (Exception e)
        {
            GUILayout.Label(e.ToString());
        }
    }

    void OnAddBlueprint()
    {
        if (ingredients == null) OnEnable();
        addBlueprint.finalItem.ID = EditorGUILayout.Popup("Final item", addBlueprint.finalItem.ID, ItemNames);
        for (int i = 0; i < ingredients.Count; i++)
        {
            GUILayout.BeginHorizontal();
            ingredients[i].item.ID = EditorGUILayout.Popup("Ingredient tile " + (i + 1), ingredients[i].item.ID, ItemNames);
            ingredients[i].Amount = EditorGUILayout.IntField("Amount", ingredients[i].Amount);
            GUILayout.EndHorizontal();
        }

        GUI.color = Color.green;
        if (GUILayout.Button("Add blueprints"))
        {
            if (addBlueprint.finalItem.ID == -1)
            {
                EditorApplication.Beep(); 
                EditorUtility.DisplayDialog("Unselected item", "Final item has not been selected", "OK");
                return;
            }

            if (ingredients.Any(item => item.item.ID < 0))
            {
                EditorApplication.Beep();
                EditorUtility.DisplayDialog("Unselected ingredients", "Not all ingredients have been selected", "OK");
                return;
            }

            AddBlueprint();

            //Reset
            ingredients = new List<Ingredient>();
            addBlueprint = new Blueprint();
        }
        GUI.color = Color.white;
    }

    private void AddBlueprint()
    {
        addBlueprint.finalItem.ID--;
        addBlueprint.finalItem = itemDatabase.FetchItemByID(addBlueprint.finalItem.ID);
        foreach (Ingredient ingredient in ingredients)
        {
            ingredient.item.ID--; //popup starts at 0, 0 is an empty item. -1 is the id for an empty item thus decrement
            if (ingredient.item.ID < 0) //If empty don't fetch but add empty item
                ingredient.item = new Item();
            else
                ingredient.item = itemDatabase.FetchItemByID(ingredient.item.ID);
        }
        addBlueprint.ingredients = ingredients;
        Debug.Log("Adding blue print! :)");
        itemDatabase.blueprints.Add(addBlueprint);  
    }

    void OnEditBlueprints()
    {
        if (ingredients == null) OnEnable();
        if (itemDatabase == null || itemDatabase.blueprints == null || itemDatabase.blueprints.Count < 1)
        {
            GUILayout.Label("No blueprints created. " + (itemDatabase == null) + ", " + (itemDatabase.blueprints == null) + ", " + (itemDatabase.blueprints.Count < 1), GUILayout.Width(position.width));
            return;
        }

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < itemDatabase.blueprints.Count; i++)
        {
            if (itemDatabase.blueprints.Count >= manageBlueprints.Count)
                manageBlueprints.Add(false);
            GUILayout.BeginVertical("Box");
            Item finalItem = itemDatabase.blueprints[i].finalItem;
            if (finalItem != null)
                manageBlueprints[i] = EditorGUILayout.Foldout(manageBlueprints[i], finalItem.ItemName);
            if (manageBlueprints[i])
            {
                Blueprint blueprint = itemDatabase.blueprints[i];
                DisplayBlueprintProperties(blueprint);
                DisplayDeleteButton(i);
            }
            GUILayout.EndVertical();
        }
        if (GUILayout.Button("Save"))
            itemDatabase.Save();
        GUILayout.EndScrollView();
    }

    private void DisplayBlueprintProperties(Blueprint blueprint)
    {
        if (blueprint == null) return;
        blueprint.finalItem.ID = EditorGUILayout.Popup("Final item", blueprint.finalItem.ID, GetItemNames(false));
        for (int index = 0; index < blueprint.ingredients.Count; index++)
        {
            GUILayout.BeginHorizontal();
            blueprint.ingredients[index].item.ID = EditorGUILayout.Popup("Ingredient tile " + (index + 1),
                blueprint.ingredients[index].item.ID, GetItemNames(false));
            blueprint.ingredients[index].Amount = EditorGUILayout.IntField("Amount",
                blueprint.ingredients[index].Amount);
            GUILayout.EndHorizontal();
        }
    }

    private void DisplayDeleteButton(int index)
    {
        GUI.color = Color.red;
        if (GUILayout.Button("Delete Item"))
            itemDatabase.blueprints.RemoveAt(index);
        GUI.color = Color.white;
    }

    string[] GetItemNames(bool ContainsBlank)
    {
        List<string> itemNameList = new List<string>();
        if (ContainsBlank) itemNameList.Add("Empty");

        foreach (Item item in itemDatabase.items)
            itemNameList.Add(item.ItemName);

        string[] itemNames = itemNameList.ToArray();
        return itemNames;
    }

    void OnDestroy()
    {
        if (AutoSave.autoSave)
        {
            itemDatabase.Save();
            return;
        }
        bool saveChanges = EditorUtility.DisplayDialog("Save changes",
            "Would you like to save your changes?\nIf you don't want to be notified enable auto save.", "Yes", "No");
        if (saveChanges) itemDatabase.Save();
    }
}