using UnityEngine;
using UnityEngine.EventSystems;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public Item item;
    [HideInInspector] public int amount;
    [HideInInspector] public int slot;

    [HideInInspector] public InventoryToolTip tooltip;
    [HideInInspector] public Inventory inventory;

    void Start()
    {
        amount = 1;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.position = eventData.position;
            transform.SetParent(inventory.canvas.transform);
            transform.SetAsLastSibling();
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            transform.position = eventData.position; //Subtract by offset to apply offset (apply to line 25 too)
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(inventory.slots[slot].transform);
        transform.position = inventory.slots[slot].transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.hovered.Contains(this.gameObject))
            tooltip.Activate(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventData.hovered.Contains(this.gameObject))
            tooltip.Deactivate();
    }
}
