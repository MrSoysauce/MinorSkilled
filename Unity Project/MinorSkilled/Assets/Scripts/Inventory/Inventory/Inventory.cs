
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 649

public class Inventory : MonoBehaviour
{
    #region tooltip string
    private const string tooltip1 = "Enables debug tool to print out inventory data";
    private const string tooltip2 = "Prefab of the slot gameobject";
    private const string tooltip3 = "Prefab of the item gameobject";
    private const string tooltip4 = "The panel in the scene where the slots are added to";
    #endregion

    [HideInInspector] public ItemDatabase database;              //The item database containing the properties of the items
    [Tooltip(tooltip1)][SerializeField] private bool debug;                         //Enables debug prints
    [Tooltip(tooltip2)][SerializeField] private GameObject slotPrefab;              //The prefab of the slot
    [Tooltip(tooltip3)][SerializeField] private GameObject itemPrefab;              //The prefab of the item
    [Tooltip(tooltip4)][SerializeField] private Transform inventoryPanel;           //UI panel in the scene

    [SerializeField] private int slotAmount;
    [SerializeField] private float slotRadius;
    private int oldSlotAmount;

    public Canvas canvas;
    [HideInInspector] public List<Item> items;
    [HideInInspector] public List<GameObject> slots;

    public Tooltip toolTip;

    private List<Slot> slotObjects;

    void Awake()
    {
        ConstructInventory();
        inventoryPanel.gameObject.SetActive(false);
    }

    private void ConstructInventory()
    {
        oldSlotAmount = slotAmount;
        database = new ItemDatabase();
        items = new List<Item>();
        slots = new List<GameObject>();
        slotObjects = new List<Slot>();

        float interpolate = 360.0f / slotAmount;
        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(slotPrefab));
            slots[i].transform.SetParent(inventoryPanel);
            slotObjects.Add(slots[i].GetComponent<Slot>());
            slotObjects[i].id = i;
            slotObjects[i].inv = this;
            slotObjects[i].transform.RotateAround(inventoryPanel.transform.position, inventoryPanel.transform.forward, interpolate * i + 20);
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
                itemObj.GetComponent<ItemData>().item = item;
                itemObj.GetComponent<ItemData>().slot = i;
                itemObj.transform.SetParent(slots[i].transform);

                RectTransform rectTransform = itemObj.GetComponent<RectTransform>();

                //Reset anchor presets
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.offsetMax = new Vector2(0, 0);

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
        print("Item destroyed");
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
        if (debug)
            DebugPrint();

        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.gameObject.SetActive(!inventoryPanel.gameObject.activeSelf);
            toolTip.Deactivate();
        }

        if (slotAmount != oldSlotAmount)
        {
            //Attempt to change slot amount while debugging
            List<Item> tempItems = items;
            List<GameObject> tempSlot = slots;
            List<Slot> tempSlotObj = slotObjects;

            foreach (GameObject slot in slots)
                Destroy(slot);
            foreach (Slot slotObject in slotObjects)
                Destroy(slotObject);
            ConstructInventory();
            items = tempItems;
            slots = tempSlot;
            slotObjects = tempSlotObj;
        }
    }

    private void DebugPrint()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            int itemNumber = Random.Range(0, database.items.Count);
            AddItem(itemNumber);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            print(items.Aggregate("", (current, item) => current + item.ToString()));
        }

        if (Input.GetKeyDown(KeyCode.P))
            print(database);
    }
}
