using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskController : MonoBehaviour
{
    public GameObject promptManagerReference;
    PromptManager promptManager;
    public bool isEnabled = false;

    void Start()
    {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea" && isEnabled)
        {
            promptManager.OpenComputerPrompt();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea" && isEnabled)
        {
            promptManager.OpenNotebookPrompt();
        }
    }
}
