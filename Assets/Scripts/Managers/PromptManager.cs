using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PromptManager : MonoBehaviour
{
    public GameObject notebookPrompt;
    public GameObject cancelPrompt;
    public GameObject movePrompt;
    public GameObject removePrompt;
    public GameObject petPrompt;
    public GameObject placePrompt;
    public static AvailableActionSet currentActionSet;

    void Start()
    {
        OpenNotebookPrompt();
    }

    void Update()
    {
        
    }

    void SetPromptsVisibility(bool bool1, bool bool2, bool bool3, bool bool4, bool bool5, bool bool6) {
        notebookPrompt.SetActive(bool1);
        cancelPrompt.SetActive(bool2);
        movePrompt.SetActive(bool3);
        removePrompt.SetActive(bool4);
        petPrompt.SetActive(bool5);
        placePrompt.SetActive(bool6);
    }

    public void CatRoomsPrompts() {
        notebookPrompt.transform.Find("Letter").gameObject.GetComponent<TextMeshProUGUI>().text = "Z";
        notebookPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Open Notebook";
        SetPromptsVisibility(true, false, true, false, false, false);
        currentActionSet = AvailableActionSet.CatRoomsPrompts;
    }

    public void CatRoomsPromptsWithCats() {
        notebookPrompt.transform.Find("Letter").gameObject.GetComponent<TextMeshProUGUI>().text = "Z";
        notebookPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Open Notebook";
        SetPromptsVisibility(true, false, true, false, true, false);
        currentActionSet = AvailableActionSet.CatRoomsPromptsWithCats;
    }
    
    public void OpenNotebookPrompt() {
        notebookPrompt.transform.Find("Letter").gameObject.GetComponent<TextMeshProUGUI>().text = "Z";
        notebookPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Open Notebook";
        SetPromptsVisibility(true, false, false, false, false, false);
        currentActionSet = AvailableActionSet.OpenNotebookPrompt;
    }

    public void CloseNotebookPrompt() {
        notebookPrompt.transform.Find("Letter").gameObject.GetComponent<TextMeshProUGUI>().text = "Z";
        notebookPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Close Notebook";
        SetPromptsVisibility(true, false, false, false, false, false);
        currentActionSet = AvailableActionSet.CloseNotebookPrompt;
    }

    public void EditPlacePrompts() {
        placePrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Place";
        placePrompt.GetComponent<RectTransform>().anchoredPosition = new Vector2(243.0f, 48.0f);
        SetPromptsVisibility(false, true, false, false, false, true);
        currentActionSet = AvailableActionSet.EditPlacePrompts;
    }

    public void EditPickUpPrompts() {
        placePrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Pick Up";
        placePrompt.GetComponent<RectTransform>().anchoredPosition = new Vector2(243.0f, 48.0f);
        SetPromptsVisibility(false, true, false, true, false, true);
        currentActionSet = AvailableActionSet.EditPickUpPrompts;
    }

    public void PlacePrompt() {
        placePrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Place";
        placePrompt.GetComponent<RectTransform>().anchoredPosition = new Vector2(89.0f, 48.0f);
        SetPromptsVisibility(false, false, false, false, false, true);
        currentActionSet = AvailableActionSet.PlacePrompt;
    }

    public void EditCancelPrompt() {
        SetPromptsVisibility(false, true, false, false, false, false);
        currentActionSet = AvailableActionSet.EditCancelPrompt;
    }

    public void OpenComputerPrompt() {
        notebookPrompt.transform.Find("Letter").gameObject.GetComponent<TextMeshProUGUI>().text = "Z";
        notebookPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Open Computer";
        SetPromptsVisibility(true, false, false, false, false, false);
        currentActionSet = AvailableActionSet.OpenComputerPrompt;
    }

    public void CloseComputerPrompt() {
        notebookPrompt.transform.Find("Letter").gameObject.GetComponent<TextMeshProUGUI>().text = "Z";
        notebookPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Close Computer";
        SetPromptsVisibility(true, false, false, false, false, false);
        currentActionSet = AvailableActionSet.CloseComputerPrompt;
    }

    public void NoActions() {
        SetPromptsVisibility(false, false, false, false, false, false);
        currentActionSet = AvailableActionSet.NoActions;
    }

    public void StopPetting() {
        notebookPrompt.transform.Find("Letter").gameObject.GetComponent<TextMeshProUGUI>().text = "E";
        notebookPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Stop Petting";
        SetPromptsVisibility(true, false, false, false, false, false);
        currentActionSet = AvailableActionSet.StopPetting;
    }
}
