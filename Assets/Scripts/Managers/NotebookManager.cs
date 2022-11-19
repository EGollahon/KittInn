using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public static bool isNotebookOpen = false;
    public static NotebookTab currentTab = NotebookTab.Handbook;

    public GameObject notebookReference;
    public GameObject notebookPromptReference;
    TextMeshProUGUI notebookPromptDisplay;
    public Sprite selectedTabSprite;
    public Sprite unselectedTabSprite;

    void Start()
    {
        notebookPromptDisplay = notebookPromptReference.GetComponent<TextMeshProUGUI>();
        notebookReference.transform.Find("Handbook Tab").gameObject.GetComponent<Button>().onClick.AddListener(() => SwitchToTab(NotebookTab.Handbook));
        notebookReference.transform.Find("Inventory Tab").gameObject.GetComponent<Button>().onClick.AddListener(() => SwitchToTab(NotebookTab.Inventory));
        notebookReference.transform.Find("Quests Tab").gameObject.GetComponent<Button>().onClick.AddListener(() => SwitchToTab(NotebookTab.Quests));
    }

    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            if (!isNotebookOpen)
            {
                TimeManager.PauseTime();
                notebookReference.SetActive(true);
                isNotebookOpen = true;
                notebookPromptDisplay.text = "Close Notebook";
            } else {
                TimeManager.UnpauseTime();
                notebookReference.SetActive(false);
                isNotebookOpen = false;
                notebookPromptDisplay.text = "Open Notebook";
            }
        }

        if ((isNotebookOpen && !gameObject.activeInHierarchy) || (!isNotebookOpen && gameObject.activeInHierarchy))
        {
            if (isNotebookOpen)
            {
                TimeManager.PauseTime();
                notebookReference.SetActive(true);
                isNotebookOpen = true;
                notebookPromptDisplay.text = "Close Notebook";
            } else {
                TimeManager.UnpauseTime();
                notebookReference.SetActive(false);
                isNotebookOpen = false;
                notebookPromptDisplay.text = "Open Notebook";
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
