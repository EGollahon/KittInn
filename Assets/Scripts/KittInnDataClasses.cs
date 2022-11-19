using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandbookEntryClass
{
    public string title;
    public string description;
    public bool isUnlocked;
}

[System.Serializable]
public class InventoryItemClass
{
    public string itemName;
    public Sprite sprite;
    public int quantity;
    public int level;
    public int buyCost;
    public int sellCost;
    public InventoryType type;
}

[System.Serializable]
public class QuestEntryClass
{
    public string itemName;
    public Sprite sprite;
    public int quantity;
    public int level;
    public int buyCost;
    public int sellCost;
    public InventoryType type;
}
