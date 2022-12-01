using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandbookEntryClass
{
    public string title;
    public string content;
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
    public string title;
    public string description;
    public bool isLevelUpQuest;
    public bool isRewardMoney;
    public int rewardMoney;
    public string rewardItem;
    public int rewardItemQuantity;

    public bool isShown;
    public bool isCompleted;
    public string questDependentOn;
}

[System.Serializable]
public class CoatClass
{
    public CoatColor coatColor;
    public Sprite displaySprite;
}

[System.Serializable]
public class FoodClass
{
    public Food foodName;
    public Sprite displaySprite;
}

[System.Serializable]
public class DialogSetClass
{
    public string name;
    public List<DialogClass> dialogList;
}

[System.Serializable]
public class DialogClass
{
    public string speaker;
    public string dialog;
}