using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ComputerManager : MonoBehaviour
{
    public static bool isComputerOpen = false;
    public static ComputerTab currentTab = ComputerTab.MeowMart;
    public GameObject parentReference;

    public GameObject computerReference;
    public Sprite selectedTabSprite;
    public Sprite unselectedTabSprite;

    public GameObject promptManagerReference;
    PromptManager promptManager;

    void Start()
    {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
        computerReference.transform.Find("MeowMart Tab").gameObject.GetComponent<Button>().onClick.AddListener(() => SwitchToTab(ComputerTab.MeowMart));
        computerReference.transform.Find("Guests Tab").gameObject.GetComponent<Button>().onClick.AddListener(() => SwitchToTab(ComputerTab.Guests));
        computerReference.transform.Find("KittInn Tab").gameObject.GetComponent<Button>().onClick.AddListener(() => SwitchToTab(ComputerTab.KittInn));
    }

    void Update()
    {
        if (Input.GetKeyDown("z")
        && (PromptManager.currentActionSet == AvailableActionSet.OpenComputerPrompt
            || PromptManager.currentActionSet == AvailableActionSet.CloseComputerPrompt)
        )
        {
            if (!isComputerOpen)
            {
                TimeManager.PauseTime();
                computerReference.SetActive(true);
                isComputerOpen = true;
                promptManager.CloseComputerPrompt();
                parentReference.transform.Find("MeowMart Manager").GetComponent<MeowMartManager>().RefreshMartDetail();
            } else {
                TimeManager.UnpauseTime();
                computerReference.SetActive(false);
                isComputerOpen = false;
                promptManager.OpenComputerPrompt();
            }
        }
    }

    void SwitchToTab(ComputerTab newTab) {
        if (newTab != currentTab) {
            computerReference.transform.Find(currentTab.ToString() + " Tab").gameObject.GetComponent<Image>().sprite = unselectedTabSprite;
            computerReference.transform.Find(currentTab.ToString()).gameObject.SetActive(false);

            currentTab = newTab;
            computerReference.transform.Find(currentTab.ToString() + " Tab").gameObject.GetComponent<Image>().sprite = selectedTabSprite;
            computerReference.transform.Find(currentTab.ToString()).gameObject.SetActive(true);
        }
    }
}
