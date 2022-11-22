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
}
