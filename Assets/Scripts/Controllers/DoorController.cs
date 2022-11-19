using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    SpriteRenderer doorRenderer;
    public Sprite openSprite;
    public Sprite closedSprite;

    void Start()
    {
        doorRenderer = GetComponent<SpriteRenderer>();
    }

    // void Update()
    // {
        
    // }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea")
        {
            doorRenderer.sprite = openSprite;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea")
        {
            doorRenderer.sprite = closedSprite;
        }
    }
}
