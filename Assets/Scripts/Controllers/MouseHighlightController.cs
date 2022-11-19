using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHighlightController : MonoBehaviour
{
    public bool isLargeHighlight;

    Animator highlightAnimator;

    void Start()
    {
        highlightAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 roundedPosition = new Vector2(Mathf.Round(worldPosition.x), Mathf.Round(worldPosition.y));
        transform.position = roundedPosition;
    }

    public void SetHighlightProps(bool isLarge)
    {
        isLargeHighlight = isLarge;
        highlightAnimator.SetBool("IsLargeHighlight", isLarge);
    }
}
