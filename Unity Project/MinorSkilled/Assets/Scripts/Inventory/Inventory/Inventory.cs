using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Inventory : MonoBehaviour
{
    #region tooltip string
    private const string tooltip2 = "Prefab of the slot gameobject";
    private const string tooltip3 = "Prefab of the item gameobject";
    private const string tooltip4 = "The panel in the scene where the slots are added to";
    #endregion

    [HideInInspector] public ItemDatabase database;              //The item database containing the properties of the items
    [Tooltip(tooltip2)][SerializeField] private GameObject slotPrefab;              //The prefab of the slot
    [Tooltip(tooltip3)][SerializeField] private GameObject itemPrefab;              //The prefab of the item
    [Tooltip(tooltip4)][SerializeField] private Transform inventoryPanel;           //UI panel in the scene

    [SerializeField] private int slotAmount;
    [SerializeField] private float slotRadius = 50;

    [SerializeField] private InventoryToolTip tooltip;

    public Canvas canvas;
    [HideInInspector] public List<Item> items;
    [HideInInspector] public List<GameObject> slots;

    private List<Slot> slotObjects;

    void Awake()
    {
        ConstructInventory();
        inventoryPanel.gameObject.SetActive(false);
    }

    private void ConstructInventory()
    {
        database = new ItemDatabase();
        items = new List<Item>();
        slots = new List<GameObject>();
        slotObjects = new List<Slot>();

        float interpolate = 360.0f / slotAmount;
        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(slotPrefab));
            slotObjects.Add(slots[i].GetComponent<Slot>());
            slotObjects[i].id = i;
            slotObjects[i].inv = this;
            slots[i].transform.SetParent(inventoryPanel);
            slotObjects[i].transform.RotateAround(inventoryPanel.transform.position, inventoryPanel.transform.forward, interpolate * i + 15);
            slotObjects[i].transform.position = inventoryPanel.transform.position +
                                                (slotObjects[i].transform.position - inventoryPanel.transform.position)
                                                .normalized * slotRadius;
        }
    }

    /// <summary>
    /// Returns the item data of the specified item with the lowest amount
    /// </summary>
    /// <param name="item">The item type</param>
    /// <returns></returns>
    public ItemData LowestItemAmount(Item item)
    {
        int itemAmount = 0;
        ItemData output = null;
        if (items.Contains(item))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == item.ID)
                {
                    ItemData data = slotObjects[i].GetComponentInChildren<ItemData>();
                    if (itemAmount == 0 || data.amount < itemAmount)
                    {
                        itemAmount = data.amount;
                        output = data;
                    }
                }
            }
        }
        return output;
    }

    /// <summary>
    /// Fetches item id from database and adds item to inventory
    /// </summary>
    /// <param name="id"></param>
    public void AddItem(int id)
    {
        Item itemToAdd = database.FetchItemByID(id);
        if (itemToAdd == null)
        {
            Debug.LogWarning("Item with ID " + id + " wasn't found. Not adding anything!");
            return;
        }

        //Get all items with the same id
        List<Item> itemInstances = ItemInstancesInInventory(itemToAdd.ID);
        ItemData lowestItemAmount = LowestItemAmount(itemToAdd);
        if (itemInstances.Count > 0 && itemToAdd.Stackable && lowestItemAmount.amount < itemToAdd.MaxStack)
            IncrementItemAmount(lowestItemAmount);
        else
            AddNewItem(itemToAdd);
    }

    /// <summary>
    /// Finds ID of given obj and adds the item by id to inventory
    /// </summary>
    /// <param name="obj">Must contain ItemData component</param>
    public void AddItem(GameObject obj)
    {
        AddItem(obj.GetComponent<ItemData>().item.ID);
    }

    /// <summary>
    /// Returns the amount of item instances in the inventory with a certain ID
    /// </summary>
    public List<Item> ItemInstancesInInventory(int id)
    {
        return items.Where(item => item.ID == id).ToList();
    }

    private void AddNewItem(Item item)
    {
        for (int i = 0; i < slots.Count; i++)   //Go through inventory slots
        {
            if (items[i].ID == -1)              //If empty slot found, add item
            {
                items[i] = item;
                GameObject itemObj = Instantiate(itemPrefab);
                itemObj.GetComponent<ItemData>().tooltip = tooltip;
                itemObj.GetComponent<ItemData>().inventory = this;
                itemObj.GetComponent<ItemData>().item = item;
                itemObj.GetComponent<ItemData>().slot = i;
                itemObj.transform.SetParent(slots[i].transform);
                itemObj.transform.localPosition = Vector3.zero;
                itemObj.GetComponent<Image>().sprite = item.GetSprite();
                break;
            }
        }
    }

    /// <summary>
    /// Increases given itemdata by 1 and updates text string
    /// </summary>
    /// <param name="data"></param>
    private void IncrementItemAmount(ItemData data)
    {
        if (data.amount == 0) data.amount++;
        data.amount++;
        if (data.transform.childCount != 0)
            data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
    }

    /// <summary>
    /// Destroy the item that
    /// </summary>
    /// <param name="index"></param>
    public void DestroyItemAt(int index)
    {
        if (slots[index].transform.childCount != 0) Destroy(slots[index].transform.GetChild(0).gameObject);
        else return;
        items[index] = new Item();
    }

    public void DestroyItem(int id)
    {
        foreach (GameObject slotObj in slots)
        {
            if (slotObj.transform.childCount != 0 && slotObj.GetComponentInChildren<Item>().ID == id)
            {
                Destroy(slotObj.transform.GetChild(0).gameObject);
                return;
            }
        }
    }

    public GameObject GetItemObject(Item item)
    {
        GameObject itemObj = Instantiate(itemPrefab);
        itemObj.GetComponent<ItemData>().item = item;
        itemObj.GetComponent<Image>().sprite = item.GetSprite();
        return itemObj;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.gameObject.SetActive(!inventoryPanel.gameObject.activeSelf);
            tooltip.Deactivate();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            AddItem(0);
        }
    }
}
