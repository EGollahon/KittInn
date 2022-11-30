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
    public List<GameObject> inventorySlots = new List<GameObject>();
    public List<InventoryItemClass> inventory = new List<InventoryItemClass>();
    public Sprite oneStarSprite;
    public Sprite twoStarSprite;
    public GameObject mouseHighlight;
    
    public GameObject promptManagerReference;
    PromptManager promptManager;

    void Start()
    {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
        RefreshInventory();
    }

    void Update()
    {
        if (
            Input.GetKeyDown("x")
            && NotebookManager.currentTab == NotebookTab.Inventory
            && selectedItem != null
            && selectedItem.type != InventoryType.Food
            && PlayerController.currentRoom != Room.Other
            && PromptManager.currentActionSet == AvailableActionSet.CloseNotebookPrompt
        )
        {
            NotebookManager.CloseNotebook();
            promptManager.EditPlacePrompts();
            mouseHighlight.SetActive(true);
            mouseHighlight.GetComponent<MouseHighlightController>().SetHighlightProps(selectedItem);
        }
    }

    public void RefreshInventory() {
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
        itemDetailReference.transform.Find("Picture").gameObject.GetComponent<Image>().sprite = item.sprite;

        if (item.type == InventoryType.FoodDish || item.type == InventoryType.WaterDish || item.type == InventoryType.LitterBox) {
            itemDetailReference.transform.Find("Type").gameObject.GetComponent<TextMeshProUGUI>().text = "Basic";
        } else {
            itemDetailReference.transform.Find("Type").gameObject.GetComponent<TextMeshProUGUI>().text = item.type.ToString();
        }

        if (item.level == 1) {
            itemDetailReference.transform.Find("Level Star").gameObject.GetComponent<Image>().sprite = oneStarSprite;
        } else {
            itemDetailReference.transform.Find("Level Star").gameObject.GetComponent<Image>().sprite = twoStarSprite;
        }

        if (item.type != InventoryType.Food && PlayerController.currentRoom != Room.Other) {
            itemDetailReference.transform.Find("Place Prompt").gameObject.SetActive(true);
        } else {
            itemDetailReference.transform.Find("Place Prompt").gameObject.SetActive(false);
        }
    }

    public void RefreshInventoryDetail() {
        if (selectedItem != null && selectedItem.itemName != "" && selectedItem.quantity > 0) {
            ShowInventoryDetail(selectedItem);
        } else {
            selectedItem = null;
            itemDetailReference.SetActive(false);
        }
    }

    public void RemoveInventoryItems(string itemName, int amount) {
        int index = inventory.FindIndex(element => element.itemName == itemName);
        inventory[index].quantity -= amount;
        RefreshInventory();
        RefreshInventoryDetail();
    }

    public void AddInventoryItems(string itemName, int amount) {
        int index = inventory.FindIndex(element => element.itemName == itemName);
        inventory[index].quantity += amount;
        RefreshInventory();
        RefreshInventoryDetail();
    }
}
