using UnityEngine;
using UnityEngine.UI;

public class InventoryToolTip : MonoBehaviour
{
    [SerializeField] private GameObject tooltip;

    private Item item;
    private Text text;
    private string data;

    void Start() {
        if (!tooltip) Debug.LogWarning("Tooltip has not been added into the inspector");
        tooltip.SetActive(false);
        text = tooltip.GetComponentInChildren<Text>();
    }

	void Update () {
	    if (tooltip != null && tooltip.activeSelf)
	    {
	        tooltip.transform.position = Input.mousePosition;
	    }
	}

    public void Activate(Item pItem)
    {
        item = pItem;
        ConstructDataString();
        tooltip.transform.SetAsLastSibling();
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
    }

    private void ConstructDataString()
    {
        data = "<b>"+item.ItemName+"</b>\n\n"+item.Description;
        text.text = data;
    }
}
