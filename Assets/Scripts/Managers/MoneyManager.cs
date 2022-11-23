using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public static int money = 50;
    
    public GameObject moneyReference;
    TextMeshProUGUI moneyDisplay;

    void Start()
    {
        moneyDisplay = moneyReference.GetComponent<TextMeshProUGUI>();
        moneyDisplay.text = money.ToString();
    }

    void Update()
    {
        if (moneyDisplay.text != money.ToString()) {
            moneyDisplay.text = money.ToString();
        }
    }

    public static void DepositMoney(int amount) {
        money += amount;
    }

    public static void WithdrawMoney(int amount) {
        if (money >= amount) {
            money -= amount;
        }
    }
}
