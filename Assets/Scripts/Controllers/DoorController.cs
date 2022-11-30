using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    SpriteRenderer doorRenderer;
    public Sprite openSprite;
    public Sprite closedSprite;

    public bool isUnlocked = true;

    void Start()
    {
        doorRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea" && isUnlocked)
        {
            doorRenderer.sprite = openSprite;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea" && isUnlocked)
        {
            doorRenderer.sprite = closedSprite;
        }
    }
}
