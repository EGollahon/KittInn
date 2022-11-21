using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public string itemName;
    public Sprite sprite;
    public int level;
    public InventoryType type;
    public List<Vector2> locations = new List<Vector2>();
    public List<Vector2> entryPoints = new List<Vector2>();
    public bool isOccupied = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void InitializeItem(InventoryItemClass itemValues, List<Vector2> newLocations, List<Vector2> newEntryPoints) {
        itemName = itemValues.itemName;
        sprite = itemValues.sprite;
        level = itemValues.level;
        type = itemValues.type;
        locations = newLocations;
        entryPoints = newEntryPoints;

        GetComponent<SpriteRenderer>().sprite = itemValues.sprite;

        if (itemValues.type == InventoryType.Bed) {
            GetComponent<BoxCollider2D>().size = new Vector2(1.9f, 1.9f);
        }
    }
}
