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
        notebookPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Open Notebook";
        SetPromptsVisibility(true, false, true, false, false, false);
        currentActionSet = AvailableActionSet.CatRoomsPrompts;
    }

    public void CatRoomsPromptsWithCats() {
        notebookPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Open Notebook";
        placePrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Pick Up";
        placePrompt.GetComponent<RectTransform>().anchoredPosition = new Vector2(489.0f, 48.0f);
        SetPromptsVisibility(true, false, true, false, true, true);
        currentActionSet = AvailableActionSet.CatRoomsPromptsWithCats;
    }
    
    public void OpenNotebookPrompt() {
        notebookPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Open Notebook";
        SetPromptsVisibility(true, false, false, false, false, false);
        currentActionSet = AvailableActionSet.OpenNotebookPrompt;
    }

    public void CloseNotebookPrompt() {
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
        currentActionSet = AvailableActionSet.PlacePrompt;
    }

    // public void OpenComputerPrompt() {
        
    // }

    // public void CloseComputerPrompt() {

    // }
}
