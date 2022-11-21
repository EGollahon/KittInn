using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public static bool isNotebookOpen = false;
    public static NotebookTab currentTab = NotebookTab.Handbook;
    public GameObject parentReference;

    public GameObject notebookReference;
    public Sprite selectedTabSprite;
    public Sprite unselectedTabSprite;

    public GameObject promptManagerReference;
    PromptManager promptManager;

    void Start()
    {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
        notebookReference.transform.Find("Handbook Tab").gameObject.GetComponent<Button>().onClick.AddListener(() => SwitchToTab(NotebookTab.Handbook));
        notebookReference.transform.Find("Inventory Tab").gameObject.GetComponent<Button>().onClick.AddListener(() => SwitchToTab(NotebookTab.Inventory));
        notebookReference.transform.Find("Quests Tab").gameObject.GetComponent<Button>().onClick.AddListener(() => SwitchToTab(NotebookTab.Quests));
    }

    void Update()
    {
        if (Input.GetKeyDown("z")
        && (PromptManager.currentActionSet == AvailableActionSet.OpenNotebookPrompt
            || PromptManager.currentActionSet == AvailableActionSet.CloseNotebookPrompt
            || PromptManager.currentActionSet == AvailableActionSet.CatRoomsPrompts
            || PromptManager.currentActionSet == AvailableActionSet.CatRoomsPromptsWithCats)
        )
        {
            if (!isNotebookOpen)
            {
                TimeManager.PauseTime();
                notebookReference.SetActive(true);
                isNotebookOpen = true;
                promptManager.CloseNotebookPrompt();
                parentReference.transform.Find("Inventory Manager").GetComponent<InventoryManager>().RefreshInventoryDetail();
            } else {
                TimeManager.UnpauseTime();
                notebookReference.SetActive(false);
                isNotebookOpen = false;
                promptManager.OpenNotebookPrompt();
            }
        }

        if ((isNotebookOpen && !notebookReference.activeInHierarchy) || (!isNotebookOpen && notebookReference.activeInHierarchy))
        {
            if (isNotebookOpen)
            {
                TimeManager.PauseTime();
                notebookReference.SetActive(true);
                isNotebookOpen = true;
                promptManager.CloseNotebookPrompt();
                parentReference.transform.Find("Inventory Manager").GetComponent<InventoryManager>().RefreshInventoryDetail();
            } else {
                TimeManager.UnpauseTime();
                notebookReference.SetActive(false);
                isNotebookOpen = false;
            }
        }
    }

    void SwitchToTab(NotebookTab newTab) {
        if (newTab != currentTab) {
            notebookReference.transform.Find(currentTab.ToString() + " Tab").gameObject.GetComponent<Image>().sprite = unselectedTabSprite;
            notebookReference.transform.Find(currentTab.ToString()).gameObject.SetActive(false);

            currentTab = newTab;
            notebookReference.transform.Find(currentTab.ToString() + " Tab").gameObject.GetComponent<Image>().sprite = selectedTabSprite;
            notebookReference.transform.Find(currentTab.ToString()).gameObject.SetActive(true);
        }
    }

    public static void OpenNotebook() {
        isNotebookOpen = true;
    }

    public static void CloseNotebook() {
        isNotebookOpen = false;
    }
}
