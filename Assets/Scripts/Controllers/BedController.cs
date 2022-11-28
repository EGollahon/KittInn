using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BedController : MonoBehaviour
{
    public GameObject fastForwardPrompt;
    public GameObject bea;
    public bool isInRange = false;
    public bool isDisabled = false;
    public bool isFastForwarding = false;
    public bool isSleeping = false;

    public GameObject promptManagerReference;
    PromptManager promptManager;

    void Start()
    {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
    }

    void Update()
    {
        if (TimeManager.isEditMode || Time.timeScale == 0.0f) {
            isDisabled = true;
            fastForwardPrompt.SetActive(false);
        } else if (!isSleeping) {
            isDisabled = false;
            fastForwardPrompt.SetActive(true);
        }

        if (!isDisabled && Input.GetKeyDown("f")) {
            if (isInRange) {
                Sleep();
            } else {
                FastForward();
            }
        }

        if (isSleeping && TimeManager.time == 7.0f) {
            isSleeping = false;
            isDisabled = false;
            TimeManager.lengthOfQuarterHour = 2.0f;
            fastForwardPrompt.SetActive(true);
            fastForwardPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Fast Forward";

            bea.GetComponent<PlayerController>().isSleeping = false;
            bea.transform.position = new Vector2(-10.19f, -4.0f);
            bea.GetComponent<Animator>().SetBool("IsSleeping", false);
            gameObject.layer = LayerMask.NameToLayer("Default");
            promptManager.OpenNotebookPrompt();
        }
    }

    void FastForward() {
        if (isFastForwarding) {
            isFastForwarding = false;
            TimeManager.lengthOfQuarterHour = 2.0f;
            fastForwardPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Fast Forward";
        } else {
            isFastForwarding = true;
            TimeManager.lengthOfQuarterHour = 0.5f;
            fastForwardPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Normal Time";
        }
    }

    void Sleep() {
        isSleeping = true;
        isDisabled = true;
        fastForwardPrompt.SetActive(false);
        promptManager.NoActions();
        gameObject.layer = LayerMask.NameToLayer("Being Carried");
        bea.transform.position = transform.position;
        bea.GetComponent<PlayerController>().isSleeping = true;
        bea.GetComponent<Animator>().SetBool("IsSleeping", true);
        TimeManager.lengthOfQuarterHour = 0.25f;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea" && TimeManager.timeOfDay == TimeOfDay.Night && !isFastForwarding)
        {
            isInRange = true;
            bea = collider.gameObject;
            fastForwardPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Fall Asleep";
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea" && !isSleeping && !isFastForwarding)
        {
            isInRange = false;
            bea = null;
            fastForwardPrompt.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "Fast Forward";
        }
    }
}
