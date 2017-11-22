using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public int id;
    [HideInInspector] public Inventory inv;

    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        if (inv.items[id].ID == -1)
        {
            inv.items[droppedItem.slot] = new Item();
            inv.items[id] = droppedItem.item;
            droppedItem.slot = id;
        }
        else if (droppedItem.slot != id)
        {
            SwitchItems(droppedItem);
        }
    }

    private void SwitchItems(ItemData droppedItem)
    {
        Transform item = transform.GetChild(0);
        item.GetComponent<ItemData>().slot = droppedItem.slot;
        item.transform.SetParent(inv.slots[droppedItem.slot].transform);
        item.transform.position = inv.slots[droppedItem.slot].transform.position;

        droppedItem.slot = id;

        inv.items[item.GetComponent<ItemData>().slot] = item.GetComponent<ItemData>().item;
        inv.items[id] = droppedItem.item;
    }
}
