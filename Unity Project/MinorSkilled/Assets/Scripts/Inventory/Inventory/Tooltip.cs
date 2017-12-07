using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
#pragma warning disable 649

public class Tooltip : MonoBehaviour
{
    private Item item;
    private Text text;
    [SerializeField] private GameObject tooltip;
    private string data;

    void Start() {
        if (!tooltip) Debug.LogWarning("Tooltip has not been added into the inspector");

        #if UNITY_EDITOR
        if (PrefabUtility.GetPrefabParent(tooltip) == null && PrefabUtility.GetPrefabObject(tooltip) != null)
            Debug.LogWarning("Prefab tooltip has been selected instead of a scene object, change the tooltip to a gameobject from the scene");
        #endif

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
