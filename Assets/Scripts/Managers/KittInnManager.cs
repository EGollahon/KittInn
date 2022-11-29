using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KittInnManager : MonoBehaviour
{
    public static int totalPurr = 0;
    public static float averagePurr = 0;
    public static int totalGuests = 0;
    public static int level = 1;
    public static int purrNeededForLevel2 = 100;
    public static int purrNeededForLevel3 = 200;

    public GameObject innStatsReference;

    void Start()
    {
        innStatsReference.transform.Find("Level").gameObject.GetComponent<TextMeshProUGUI>().text = level.ToString();
        innStatsReference.transform.Find("Purr Label").gameObject.GetComponent<TextMeshProUGUI>().text = "Total Purr: " + totalPurr.ToString();
        int denominator = purrNeededForLevel2;
        if (level == 2) {
            denominator = purrNeededForLevel3;
        }
        float newMaskHeight = (400.0f * ((float)totalPurr / denominator)) + 1.0f;
        innStatsReference.transform.Find("Purr Mask").gameObject.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newMaskHeight);
        innStatsReference.transform.Find("Average Purr/Text").gameObject.GetComponent<TextMeshProUGUI>().text = averagePurr.ToString() + "%";
        innStatsReference.transform.Find("Total Guests/Text").gameObject.GetComponent<TextMeshProUGUI>().text = totalGuests.ToString();
    }

    void Update()
    {
        
    }
}
