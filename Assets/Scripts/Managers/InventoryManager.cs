using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public InventoryItemClass selectedItem;
    public GameObject inventoryReference;
    public GameObject itemDetailReference;
    public List<GameObject> inventorySlots;
    public List<InventoryItemClass> inventory = new List<InventoryItemClass>();
    public Sprite oneStarSprite;
    public Sprite twoStarSprite;
    public GameObject mouseHighlight;

    void Start()
    {
        RefreshInventory();
    }

    void Update()
    {
        // also check if in an editable room
        if (Input.GetKeyDown("x") && NotebookManager.currentTab == NotebookTab.Inventory && selectedItem.type != InventoryType.Food)
        {
            NotebookManager.CloseNotebook();
            mouseHighlight.SetActive(true);
        }
    }

    void RefreshInventory() {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
            inventorySlots[i].transform.Find("Item").gameObject.GetComponent<Image>().sprite = null;
            inventorySlots[i].SetActive(false);
        }

        int usedSlots = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].quantity > 0) {
                int itemIndex = i;
                inventorySlots[usedSlots].GetComponent<Button>().onClick.AddListener(() => ShowInventoryDetail(inventory[itemIndex]));
                inventorySlots[usedSlots].transform.Find("Item").gameObject.GetComponent<Image>().sprite = inventory[itemIndex].sprite;
                inventorySlots[usedSlots].SetActive(true);
                usedSlots += 1;
            }
        }
    }

    void ShowInventoryDetail(InventoryItemClass item) {
        selectedItem = item;
        itemDetailReference.SetActive(true);

        itemDetailReference.transform.Find("Name Quantity").gameObject.GetComponent<TextMeshProUGUI>().text = item.itemName + " x" + item.quantity.ToString();
        itemDetailReference.transform.Find("Type").gameObject.GetComponent<TextMeshProUGUI>().text = item.type.ToString();
        itemDetailReference.transform.Find("Picture").gameObject.GetComponent<Image>().sprite = item.sprite;

        if (item.level == 1) {
            itemDetailReference.transform.Find("Level Star").gameObject.GetComponent<Image>().sprite = oneStarSprite;
        } else {
            itemDetailReference.transform.Find("Level Star").gameObject.GetComponent<Image>().sprite = twoStarSprite;
        }

        // also add condition to check if in an editable room
        if (item.type != InventoryType.Food) {
            itemDetailReference.transform.Find("Place Prompt").gameObject.SetActive(true);
        } else {
            itemDetailReference.transform.Find("Place Prompt").gameObject.SetActive(false);
        }
    }
}
