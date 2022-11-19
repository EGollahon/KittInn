using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public static int money = 100;
    
    public GameObject moneyReference;
    TextMeshProUGUI moneyDisplay;

    void Start()
    {
        moneyDisplay = moneyReference.GetComponent<TextMeshProUGUI>();
        moneyDisplay.text = money.ToString();
    }

    void Update()
    {
        
    }

    public void DepositMoney(int amount) {
        money += amount;
        moneyDisplay.text = money.ToString();
    }

    public void WithdrawMoney(int amount) {
        if (money >= amount) {
            money -= amount;
            moneyDisplay.text = money.ToString();
        }
    }
}
