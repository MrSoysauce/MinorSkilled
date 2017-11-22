using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

public class OutputSlot : MonoBehaviour
{
    [SerializeField] private int cellSize = 60;
    [HideInInspector] public bool containsItem;
    private Inventory inv;

    void Start()
    {
        containsItem = false;
    }

    public void ClearSlot()
    {
        if (transform.childCount > 0)
        {
            containsItem = false;
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    public void OutputItem(List<GameObject> slots,Item output)
    {
        if (containsItem) return;
        print("Creating output!");
        containsItem = true;
        GameObject obj = inv.GetItemObject(output);
        obj.transform.SetParent(transform);
        obj.transform.position = transform.position;

        RectTransform rectTransform = obj.GetComponent<RectTransform>();

        //Reset anchor presets
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);

        rectTransform.SetSizeWithCurrentAnchors(0,cellSize);
        containsItem = true;
    }
}
