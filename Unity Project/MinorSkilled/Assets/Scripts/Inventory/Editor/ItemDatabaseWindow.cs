using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

public class ItemDatabaseWindow : EditorWindow
{
    private delegate void OnGUIImplementation();

    private Dictionary<int, OnGUIImplementation> menus;
    private ItemDatabase itemDatabase;
    private readonly List<bool> manageItem = new List<bool>();
    private Item addItem;
    private int selectionGridInt;
    private Vector2 scrollPosition;

    [MenuItem("Inventory manager/Open Item Manager %e")] //Opens when CTRL + E is pressed
    public static void ShowWindow()
    {
        GetWindow<ItemDatabaseWindow>(false, "Item manager");
    }

    void Awake()
    {
        addItem = new Item();
        menus = new Dictionary<int, OnGUIImplementation>();
        itemDatabase = new ItemDatabase();
        menus[0] = OnAddItem;
        menus[1] = OnItemDatabase;
    }

    void OnAddItem()
    {
        addItem.ItemName = EditorGUILayout.TextField("Item name", addItem.ItemName);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Item Description");
        GUILayout.Space(10);
        addItem.Description = EditorGUILayout.TextArea(
            addItem.Description,
            GUILayout.Width(position.width - 157),
            GUILayout.Height(70),
            GUILayout.ExpandWidth(false)
            );
        GUILayout.EndHorizontal();
        addItem.type = (ItemType) EditorGUILayout.EnumPopup("Item type", addItem.type);
        addItem.Stackable = EditorGUILayout.Toggle("Stackable", addItem.Stackable);
        addItem.CrateDrop = EditorGUILayout.Toggle("Crate drop?", addItem.CrateDrop);
        if (addItem.Stackable)
        {
            addItem.MaxStack = EditorGUILayout.IntSlider("Max stack", addItem.MaxStack, 1, 100,
                GUILayout.Width(position.width/2));
        }
        Object spriteObj = EditorGUILayout.ObjectField("Item Icon", addItem.GetSprite(),
            typeof(Sprite), false, GUILayout.Width(position.width - 45));
        if (spriteObj != null)
        {
            addItem.SpriteFilePath = GetResourcesFilePath(spriteObj);
        }
        else addItem.SpriteFilePath = "";
        CreateAddButton();
    }

    private void CreateAddButton()
    {
        GUI.color = Color.green;
        if (GUILayout.Button("Add item"))
        {
            if (itemDatabase.items == null || itemDatabase.items.Count < 1)
            {
                itemDatabase.ConstructItemDatabase();
                addItem.ID = 0;
            }
            else addItem.ID = itemDatabase.items.Count;

            if (itemDatabase == null) Debug.Log("What");
            if (itemDatabase.items == null) Debug.Log("Wott");
            itemDatabase.items.Add(addItem);
            addItem = new Item();
        }
        GUI.color = Color.white;
    }

    void OnItemDatabase()
    {
        if (itemDatabase == null || itemDatabase.items == null || itemDatabase.items.Count < 1)
        {
            GUILayout.Label("No items created", GUILayout.Width(position.width));
            return;
        }
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        LoadItems();
        if (GUILayout.Button("Save"))
            itemDatabase.Save();
        GUILayout.EndScrollView();
    }

    private void LoadItems()
    {
        for (int i = 0; i < itemDatabase.items.Count; i++)
        {
            if (itemDatabase.items.Count >= manageItem.Count)
                manageItem.Add(false);
            GUILayout.BeginVertical("Box");
            manageItem[i] = EditorGUILayout.Foldout(manageItem[i], "" + itemDatabase.items[i].ItemName);
            if (manageItem[i])
            {
                Item item = itemDatabase.items[i];
                GUI.color = Color.red;
                if (GUILayout.Button("Delete Item"))
                {
                    itemDatabase.items.RemoveAt(i);
                }
                GUI.color = Color.white;

                EditorGUILayout.LabelField("Item ID", item.ID.ToString());
                item.ItemName = EditorGUILayout.TextField("Item name", item.ItemName);
                item.ID = i;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Item Description");
                GUILayout.Space(30);
                item.Description = EditorGUILayout.TextArea(
                    item.Description,
                    GUILayout.Width(position.width - 165),
                    GUILayout.Height(70)
                    );
                GUILayout.EndHorizontal();
                addItem.type = (ItemType) EditorGUILayout.EnumPopup("Item type", addItem.type);
                item.Stackable = EditorGUILayout.Toggle("Stackable", item.Stackable);
                item.CrateDrop = EditorGUILayout.Toggle("Crate drop?", item.CrateDrop);
                if (item.Stackable)
                    item.MaxStack = EditorGUILayout.IntSlider("Max stacks", item.MaxStack, 1, 100,
                        GUILayout.Width(position.width/2));
                Object spriteObj = EditorGUILayout.ObjectField("Item Icon", item.GetSprite(),
                    typeof(Sprite), false, GUILayout.Width(position.width - 45));
                if (spriteObj != null)
                {
                    addItem.SpriteFilePath = GetResourcesFilePath(spriteObj);
                    Debug.Log("Sprite path: " + addItem.SpriteFilePath);
                }
                else addItem.SpriteFilePath = "";
            }
            GUILayout.EndVertical();
        }
    }

    private static string GetResourcesFilePath(Object spriteObj)
    {
        string path = AssetDatabase.GetAssetPath(spriteObj);
        path = path.Replace("Assets/Resources/", "");
        string[] fullPath = path.Split('/');
        fullPath[fullPath.Length - 1] = "";
        path = "";
        for (int index = 0; index < fullPath.Length; index++)
        {
            string s = fullPath[index];
            path = path + s + ((fullPath.Length - 1 == index) ? "" : "/");
        }
        path += spriteObj.name;
        return path;
    }

    void OnGUI()
    {
        if (menus == null) Awake();
        string[] selStrings = {"Add item", "Edit Items"};
        selectionGridInt = GUILayout.SelectionGrid(selectionGridInt, selStrings, 2);
        if (menus != null)
        {
            OnGUIImplementation onGuiImplementation = menus[selectionGridInt];
            onGuiImplementation();
        }
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

