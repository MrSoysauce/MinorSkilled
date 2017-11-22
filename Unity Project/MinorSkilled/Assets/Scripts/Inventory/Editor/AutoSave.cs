using UnityEngine;
using System.Collections;
using UnityEditor;

public class AutoSave : Editor
{
    public static bool autoSave;

    void Start()
    {
        Debug.Log("Test");
        Menu.SetChecked("Inventory manager/Enable Auto Save",true);
    }

    [MenuItem("Inventory manager/Enable Auto Save")]
    public static void Save()
    {
        autoSave = !autoSave;
        Menu.SetChecked("Inventory manager/Enable Auto Save",autoSave);
    }
}
