using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public bool isNotebookOpen = false;
    public NotebookTab currentTab = NotebookTab.Inventory;

    public GameObject notebookReference;
    TextMeshProUGUI notebookDisplay;

    void Start()
    {
        notebookDisplay = notebookReference.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            if (isNotebookOpen)
            {
                TimeManager.PauseTime();
                notebookReference.SetActive(true);
            } else {
                TimeManager.UnpauseTime();
                notebookReference.SetActive(false);
            }

            isNotebookOpen = !isNotebookOpen;
        }
    }
}
