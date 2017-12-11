using UnityEngine;
using UnityEngine.EventSystems;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler {
    [HideInInspector] public Item item;
    [HideInInspector] public int amount;
    [HideInInspector] public int slot;

    private Tooltip tooltip;
    private Inventory inv;

    void Start()
    {
        inv = GameManager.Instance.inventory;
        tooltip = inv.toolTip;
        amount = 1;
    }
        
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.position = eventData.position;
            transform.SetParent(inv.canvas.transform);
            transform.SetAsLastSibling();
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.position = eventData.position; //Subtract by offset to apply offset (apply to line 25 too)
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(inv.slots[slot].transform);
        transform.position = inv.slots[slot].transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();
    }
}
