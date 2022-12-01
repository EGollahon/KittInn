using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogBox;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogText;
    public GameObject promptsUI;
    public GameObject promptManagerReference;
    public PromptManager promptManager;
    
    public List<DialogSetClass> dialogList = new List<DialogSetClass>();
    public float letterTimer = -1.0f;
    public int currentDialogSet = 0;
    public int currentDialogIndex = 0;
    public DialogClass currentDialog;
    public int currentDialogLength = 1;
    public float letterLength = 0.005f;
    public bool canContinue = false;

    void Start()
    {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
        speakerText = dialogBox.transform.Find("Speaker").gameObject.GetComponent<TextMeshProUGUI>();
        dialogText = dialogBox.transform.Find("Dialog").gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (letterTimer >= 0) {
            letterTimer -= Time.unscaledDeltaTime;
            if (letterTimer < 0) {
                AddLetter();
            }
        }

        if (Input.GetKeyDown("x") && canContinue) {
            if (dialogList[currentDialogSet].dialogList.Count > currentDialogIndex+1) {
                PlayDialogSnippet(currentDialogIndex + 1);
            } else {
                EndDialog();
            }
        }
    }

    public void StartDialog(int dialogSetIndex) {
        TimeManager.PauseTime();
        promptManager.NoActions();
        promptsUI.SetActive(false);
        dialogBox.SetActive(true);
        currentDialogSet = dialogSetIndex;
        PlayDialogSnippet(0);
    }

    public void EndDialog() {
        canContinue = false;
        TimeManager.UnpauseTime();
        promptManager.OpenNotebookPrompt();
        promptsUI.SetActive(true);
        dialogBox.SetActive(false);
    }

    public void PlayDialogSnippet(int dialogIndex) {
        currentDialogIndex = dialogIndex;
        currentDialog = dialogList[currentDialogSet].dialogList[currentDialogIndex];
        speakerText.text = currentDialog.speaker;
        currentDialogLength = 1;
        letterTimer = letterLength;
        dialogBox.transform.Find("Continue Prompt").gameObject.SetActive(false);
        canContinue = false;
    }

    public void AddLetter() {
        if (currentDialogLength <= currentDialog.dialog.Length) {
            dialogText.text = currentDialog.dialog.Substring(0, currentDialogLength);
            currentDialogLength++;
            letterTimer = letterLength;
        } else {
            dialogBox.transform.Find("Continue Prompt").gameObject.SetActive(true);
            canContinue = true;
        }
    }
}
