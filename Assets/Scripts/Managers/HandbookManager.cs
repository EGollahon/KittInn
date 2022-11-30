using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HandbookManager : MonoBehaviour
{
    public GameObject handbookReference;
    public GameObject handbookDetailReference;
    public List<GameObject> handbookSlots;
    public List<HandbookEntryClass> handbookEntries = new List<HandbookEntryClass>();

    void ShowHandbookDetail(HandbookEntryClass item) {
        handbookDetailReference.SetActive(true);
        handbookDetailReference.transform.Find("Title").gameObject.GetComponent<TextMeshProUGUI>().text = item.title;
        handbookDetailReference.transform.Find("Content").gameObject.GetComponent<TextMeshProUGUI>().text = item.content;
    }

    public void AddHandbookEntry(int index) {
        handbookSlots[index].SetActive(true);
        handbookSlots[index].GetComponent<Button>().onClick.AddListener(() => ShowHandbookDetail(handbookEntries[index]));
    }
}
