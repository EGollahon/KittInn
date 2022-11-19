using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    SpriteRenderer wallRenderer;
    public Sprite daySprite;
    public Sprite nightSprite;
    Color translucent = new Color(255.0f, 255.0f, 255.0f, 0.5f);
    Color opaque = new Color(255.0f, 255.0f, 255.0f, 1.0f);

    void Start()
    {
        wallRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (TimeManager.timeOfDay == TimeOfDay.Morning || TimeManager.timeOfDay == TimeOfDay.Day) {
            wallRenderer.sprite = daySprite;
        } else if (TimeManager.timeOfDay == TimeOfDay.Evening || TimeManager.timeOfDay == TimeOfDay.Night) {
            wallRenderer.sprite = nightSprite;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea")
        {
            wallRenderer.color = translucent;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea")
        {
            wallRenderer.color = opaque;
        }
    }
}
