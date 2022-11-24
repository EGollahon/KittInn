using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MeowMartManager : MonoBehaviour
{
    public bool isBuying = true;
    public int amount = 1;
    public int price = 0;
    public InventoryItemClass selectedItem;
    public GameObject meowMartReference;
    public GameObject meowMartDetailReference;
    public List<GameObject> meowMartSlots = new List<GameObject>();
    public Sprite oneStarSprite;
    public Sprite twoStarSprite;
    public Sprite selectedButtonSprite;
    public Sprite unselectedButtonSprite;

    public GameObject buyOption;
    public GameObject sellOption;
    public GameObject buySellPrompt;
    public GameObject amountPrompt;
    public GameObject pricePrompt;
    
    public GameObject promptManagerReference;
    PromptManager promptManager;
    public GameObject inventoryManagerReference;
    InventoryManager inventoryManager;

    void Start()
    {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
        inventoryManager = inventoryManagerReference.GetComponent<InventoryManager>();
        buyOption.GetComponent<Button>().onClick.AddListener(() => SetMode(true));
        sellOption.GetComponent<Button>().onClick.AddListener(() => SetMode(false));
        amountPrompt.transform.Find("Subtract").gameObject.GetComponent<Button>().onClick.AddListener(() => ChangeAmount(amount - 1));
        amountPrompt.transform.Find("Add").gameObject.GetComponent<Button>().onClick.AddListener(() => ChangeAmount(amount + 1));
        RefreshMart();
    }

    void Update()
    {
        if (
            Input.GetKeyDown("x")
            && PromptManager.currentActionSet == AvailableActionSet.CloseComputerPrompt
            && ComputerManager.currentTab == ComputerTab.MeowMart
        ) {
            if (isBuying && price <= MoneyManager.money) {
                BuyItems();
            } else if (!isBuying && amount <= selectedItem.quantity) {
                SellItems();
            }
        }
    }

    void RefreshMart() {
        for (int i = 0; i < meowMartSlots.Count; i++)
        {
            meowMartSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
            meowMartSlots[i].transform.Find("Item").gameObject.GetComponent<Image>().sprite = null;
            meowMartSlots[i].SetActive(false);
        }

        int usedSlots = 0;
        for (int i = 0; i < inventoryManager.inventory.Count; i++)
        {
            if ((isBuying && KittInnManager.level >= inventoryManager.inventory[i].level) || inventoryManager.inventory[i].quantity > 0) {
                int itemIndex = i;
                meowMartSlots[usedSlots].GetComponent<Button>().onClick.AddListener(() => ShowMeowMartDetail(inventoryManager.inventory[itemIndex]));
                meowMartSlots[usedSlots].transform.Find("Item").gameObject.GetComponent<Image>().sprite = inventoryManager.inventory[itemIndex].sprite;
                meowMartSlots[usedSlots].SetActive(true);
                usedSlots += 1;
            }
        }
    }

    void ShowMeowMartDetail(InventoryItemClass item) {
        selectedItem = item;
        meowMartDetailReference.SetActive(true);

        meowMartDetailReference.transform.Find("Name Quantity").gameObject.GetComponent<TextMeshProUGUI>().text = item.itemName + " x" + item.quantity.ToString();
        meowMartDetailReference.transform.Find("Picture").gameObject.GetComponent<Image>().sprite = item.sprite;

        if (item.type == InventoryType.FoodDish || item.type == InventoryType.WaterDish || item.type == InventoryType.LitterBox) {
            meowMartDetailReference.transform.Find("Type").gameObject.GetComponent<TextMeshProUGUI>().text = "Basic";
        } else {
            meowMartDetailReference.transform.Find("Type").gameObject.GetComponent<TextMeshProUGUI>().text = item.type.ToString();
        }

        if (item.level == 1) {
            meowMartDetailReference.transform.Find("Level Star").gameObject.GetComponent<Image>().sprite = oneStarSprite;
        } else {
            meowMartDetailReference.transform.Find("Level Star").gameObject.GetComponent<Image>().sprite = twoStarSprite;
        }

        ChangeAmount(1);
        if (isBuying && price > MoneyManager.money) {
            buySellPrompt.SetActive(false);
        } else {
            buySellPrompt.SetActive(true);
        }
    }

    public void RefreshMartDetail() {
        if (selectedItem != null && selectedItem.itemName != "" && selectedItem.quantity > 0) {
            ShowMeowMartDetail(selectedItem);
        } else {
            selectedItem = null;
            meowMartDetailReference.SetActive(false);
        }
    }

    void SetMode(bool setToBuy) {
        isBuying = setToBuy;
        RefreshMart();
        RefreshMartDetail();

        if (setToBuy) {
            buyOption.GetComponent<Image>().sprite = selectedButtonSprite;
            sellOption.GetComponent<Image>().sprite = unselectedButtonSprite;
            buySellPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Buy";
        } else {
            buyOption.GetComponent<Image>().sprite = unselectedButtonSprite;
            sellOption.GetComponent<Image>().sprite = selectedButtonSprite;
            buySellPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Sell";
        }
    }

    void ChangeAmount(int newAmount) {
        amount = newAmount;
        if (selectedItem != null) {
            if (isBuying && selectedItem.buyCost > 0) {
                price = amount * selectedItem.buyCost;
                pricePrompt.GetComponent<TextMeshProUGUI>().text = "-" + price.ToString();
            } else if (!isBuying && selectedItem.sellCost > 0) {
                price = amount * selectedItem.sellCost;
                pricePrompt.GetComponent<TextMeshProUGUI>().text = "+" + price.ToString();
            } else {
                price = 0;
            }
        }
        amountPrompt.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = amount.ToString();
        ConfigureAmountButtons();
    }

    void ConfigureAmountButtons() {
        if (amount == 1) {
            amountPrompt.transform.Find("Subtract").gameObject.SetActive(false);
        } else {
            amountPrompt.transform.Find("Subtract").gameObject.SetActive(true);
        }

        if (
            (!isBuying && amount == selectedItem.quantity)
            || (isBuying && (price + selectedItem.buyCost) > MoneyManager.money)
        ) {
            amountPrompt.transform.Find("Add").gameObject.SetActive(false);
        } else {
            amountPrompt.transform.Find("Add").gameObject.SetActive(true);
        }
    }

    void BuyItems() {
        selectedItem.quantity += amount;
        MoneyManager.WithdrawMoney(price);

        RefreshMart();
        RefreshMartDetail();
        inventoryManager.RefreshInventory();
        inventoryManager.RefreshInventoryDetail();
    }

    void SellItems() {
        selectedItem.quantity -= amount;
        MoneyManager.DepositMoney(price);

        RefreshMart();
        RefreshMartDetail();
        inventoryManager.RefreshInventory();
        inventoryManager.RefreshInventoryDetail();
    }
}
