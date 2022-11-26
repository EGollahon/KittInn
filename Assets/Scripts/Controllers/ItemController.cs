using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public string itemName;
    public Sprite sprite;
    public int level;
    public InventoryType type;
    public List<Vector2> locations = new List<Vector2>();
    public List<Vector2> entryPoints = new List<Vector2>();
    public bool isOccupied = false;
    public Food filledFood;
    public int availableUses = 0;
    public bool refillCleanInRange = false;

    public Sprite foodFullSprite;
    public Sprite foodEmptySprite;
    public Sprite waterFullSprite;
    public Sprite waterEmptySprite;
    public Sprite litterCleanSprite;
    public Sprite litterDirtySprite;
    public Sprite threeFoodSelect;
    public Sprite sixFoodSelect;
    public List<GameObject> foodSelectSlots = new List<GameObject>();

    GameObject refillCleanPrompt;
    GameObject foodSelectPrompt;
    InventoryManager inventoryManager;

    void Start()
    {
        refillCleanPrompt = transform.Find("Canvas/RefillClean Tooltip").gameObject;
        foodSelectPrompt = transform.Find("Canvas/FoodSelect Tooltip").gameObject;
    }

    void Update()
    {
        if (
            Input.GetKeyDown("c")
            && (PromptManager.currentActionSet == AvailableActionSet.CatRoomsPrompts || PromptManager.currentActionSet == AvailableActionSet.CatRoomsPromptsWithCats)
            && refillCleanInRange
            && availableUses == 0
        ) {
            RefillClean();
        }
    }

    public void InitializeItem(InventoryItemClass itemValues, List<Vector2> newLocations, List<Vector2> newEntryPoints, InventoryManager newInventoryManager) {
        inventoryManager = newInventoryManager;
        itemName = itemValues.itemName;
        sprite = itemValues.sprite;
        level = itemValues.level;
        type = itemValues.type;

        for (int i = 0; i < newLocations.Count; i++) {
            Vector2 locationToAdd = new Vector2(newLocations[i].x, newLocations[i].y);
            locations.Add(locationToAdd);
        }
        for (int i = 0; i < newEntryPoints.Count; i++) {
            Vector2 entryToAdd = new Vector2(newEntryPoints[i].x, newEntryPoints[i].y);
            entryPoints.Add(entryToAdd);
        }

        GetComponent<SpriteRenderer>().sprite = itemValues.sprite;

        if (itemValues.type == InventoryType.Bed) {
            GetComponent<BoxCollider2D>().size = new Vector2(1.9f, 1.9f);
        } else if (itemValues.type == InventoryType.LitterBox) {
            availableUses = 3;
        }
    }

    public void Move(List<Vector2> newLocations, List<Vector2> newEntryPoints) {
        if (type == InventoryType.Bed) {
            float newX = (newLocations[0].x + newLocations[1].x + newLocations[2].x + newLocations[3].x) / 4;
            float newY = (newLocations[0].y + newLocations[1].y + newLocations[2].y + newLocations[3].y) / 4;
            transform.position = new Vector2(newX, newY);
        } else {
            transform.position = newLocations[0];
        }

        locations.Clear();
        entryPoints.Clear();

        for (int i = 0; i < newLocations.Count; i++) {
            Vector2 locationToAdd = new Vector2(newLocations[i].x, newLocations[i].y);
            locations.Add(locationToAdd);
        }
        for (int i = 0; i < newEntryPoints.Count; i++) {
            Vector2 entryToAdd = new Vector2(newEntryPoints[i].x, newEntryPoints[i].y);
            entryPoints.Add(entryToAdd);
        }
    }

    public bool CatInteract() {
        isOccupied = true;
        if (type == InventoryType.FoodDish || type == InventoryType.WaterDish || type == InventoryType.LitterBox) {
            if (availableUses > 0) {
                return true;
            } else {
                return false;
            }
        }
        return true;
    }

    public void CatFinishInteract() {
        if (availableUses > 0) {
            availableUses--;

            if (availableUses == 0) {
                if (type == InventoryType.FoodDish) {
                    GetComponent<SpriteRenderer>().sprite = foodEmptySprite;
                } else if (type == InventoryType.WaterDish) {
                    GetComponent<SpriteRenderer>().sprite = waterEmptySprite;
                } else if (type == InventoryType.LitterBox) {
                    GetComponent<SpriteRenderer>().sprite = litterDirtySprite;
                }
            }
        }
    }

    public void CatLeave() {
        isOccupied = false;
    }

    public void RefillClean() {
        if (type == InventoryType.FoodDish) {
            ActivateFoodSelect();
        } else if (type == InventoryType.WaterDish) {
            GetComponent<SpriteRenderer>().sprite = waterFullSprite;
            availableUses = 3;
        } else if (type == InventoryType.LitterBox) {
            GetComponent<SpriteRenderer>().sprite = litterCleanSprite;
            availableUses = 3;
        }

        refillCleanPrompt.SetActive(false);
    }

    void ActivateFoodSelect() {
        foodSelectPrompt.SetActive(true);
        for (int i = 0; i < foodSelectSlots.Count; i++)
        {
            foodSelectSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
            foodSelectSlots[i].transform.Find("Item").gameObject.GetComponent<Image>().sprite = null;
            foodSelectSlots[i].SetActive(false);
        }

        int foodNum = 0;
        int usedSlots = 0;
        for (int i = 0; i < inventoryManager.inventory.Count; i++)
        {
            if (inventoryManager.inventory[i].quantity > 0 && inventoryManager.inventory[i].type == InventoryType.Food) {
                foodNum++;
            }
        }

        if (foodNum > 3) {
            foodSelectPrompt.GetComponent<Image>().sprite = sixFoodSelect;
        } else {
            foodSelectPrompt.GetComponent<Image>().sprite = threeFoodSelect;
            usedSlots = 3;
        }

        for (int i = 0; i < inventoryManager.inventory.Count; i++)
        {
            if (inventoryManager.inventory[i].quantity > 0 && inventoryManager.inventory[i].type == InventoryType.Food) {
                int itemIndex = i;
                foodSelectSlots[usedSlots].GetComponent<Button>().onClick.AddListener(() => SelectFood(inventoryManager.inventory[itemIndex]));
                foodSelectSlots[usedSlots].transform.Find("Item").gameObject.GetComponent<Image>().sprite = inventoryManager.inventory[itemIndex].sprite;
                foodSelectSlots[usedSlots].SetActive(true);
                usedSlots += 1;
            }
        }
    }

    void DeactivateFoodSelect() {
        for (int i = 0; i < foodSelectSlots.Count; i++)
        {
            foodSelectSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    void SelectFood(InventoryItemClass food) {
        switch (food.itemName) {
            case "Savory Kibble":
                filledFood = Food.SavoryKibble;
                break;
            case "Beef Pate":
                filledFood = Food.BeefPate;
                break;
            case "Chicken Kibble":
                filledFood = Food.ChickenKibble;
                break;
            case "Seafood Kibble":
                filledFood = Food.SeafoodKibble;
                break;
            case "Salmon Pate":
                filledFood = Food.SalmonPate;
                break;
            case "Turkey in Gravy":
                filledFood = Food.TurkeyInGravy;
                break;
        }
        food.quantity--;
        GetComponent<SpriteRenderer>().sprite = foodFullSprite;
        availableUses = 1;
        DeactivateFoodSelect();
        foodSelectPrompt.SetActive(false);

        inventoryManager.RefreshInventory();
        inventoryManager.RefreshInventoryDetail();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea" && availableUses == 0)
        {
            refillCleanInRange = true;
            if (type == InventoryType.FoodDish && availableUses == 0) {
                refillCleanPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Refill";
                refillCleanPrompt.SetActive(true);
            } else if (type == InventoryType.WaterDish && availableUses == 0) {
                refillCleanPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Refill";
                refillCleanPrompt.SetActive(true);
            } else if (type == InventoryType.LitterBox && availableUses == 0) {
                refillCleanPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Clean";
                refillCleanPrompt.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea")
        {
            refillCleanInRange = false;
            DeactivateFoodSelect();
            refillCleanPrompt.SetActive(false);
            foodSelectPrompt.SetActive(false);
        }
    }
}
