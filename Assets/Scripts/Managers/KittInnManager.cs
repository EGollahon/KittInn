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
    public static int purrNeededForLevel2 = 1000;
    public static int purrNeededForLevel3 = 2000;

    public int lastPurr = 0;
    public GameObject innStatsReference;
    public GameObject startScreen;
    public GameObject fade;

    public GameObject catRoomsDoor;
    public GameObject sunroomDoor;
    public GameObject sunroomBlock;

    void Awake()
    {
        RecalculateStats();
    }

    void Update()
    {
        if (lastPurr != totalPurr) {
            if (level == 1 && totalPurr >= purrNeededForLevel2) {
                level = 2;
                sunroomBlock.SetActive(false);
                catRoomsDoor.GetComponent<DoorController>().isUnlocked = true;
                sunroomDoor.GetComponent<DoorController>().isUnlocked = true;
            } else if (level == 2 && totalPurr >= purrNeededForLevel3) {
                level = 3;
            }
            RecalculateStats();
            lastPurr = totalPurr;
        }
    }

    public static void AddCatStats(int purrToAdd) {
        totalPurr += purrToAdd;
        totalGuests += 1;
    }

    public void RecalculateStats() {
        averagePurr = (float)totalPurr / (float)totalGuests;

        innStatsReference.transform.Find("Level").gameObject.GetComponent<TextMeshProUGUI>().text = level.ToString();
        innStatsReference.transform.Find("Purr Label").gameObject.GetComponent<TextMeshProUGUI>().text = "Total Purr: " + totalPurr.ToString();
        int denominator = purrNeededForLevel2;
        if (level == 2) {
            denominator = purrNeededForLevel3;
        }
        float newMaskHeight = (400.0f * ((float)totalPurr / denominator)) + 1.0f;
        innStatsReference.transform.Find("Purr Mask").gameObject.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newMaskHeight);
        innStatsReference.transform.Find("Average Purr/Text").gameObject.GetComponent<TextMeshProUGUI>().text = averagePurr.ToString("0.0") + "%";
        innStatsReference.transform.Find("Total Guests/Text").gameObject.GetComponent<TextMeshProUGUI>().text = totalGuests.ToString();
    }
}
