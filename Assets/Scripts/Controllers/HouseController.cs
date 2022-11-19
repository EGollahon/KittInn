using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour
{
    SpriteRenderer houseRenderer;
    public Sprite daySprite;
    public Sprite nightSprite;

    void Start()
    {
        houseRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (TimeManager.timeOfDay == TimeOfDay.Morning || TimeManager.timeOfDay == TimeOfDay.Day) {
            houseRenderer.sprite = daySprite;
        } else if (TimeManager.timeOfDay == TimeOfDay.Evening || TimeManager.timeOfDay == TimeOfDay.Night) {
            houseRenderer.sprite = nightSprite;
        }
    }
}
