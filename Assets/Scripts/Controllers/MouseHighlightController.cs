using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHighlightController : MonoBehaviour
{
    public bool isLargeHighlight;
    public InventoryItemClass itemToPlace;

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

        if (isLargeHighlight) {
            if (roundedPosition.x % 2 == 0) {
                roundedPosition.x -= 1.0f;
            }
            if (roundedPosition.y % 2 != 0) {
                roundedPosition.y += 1.0f;
            }
            
            if (PlayerController.currentRoom == Room.Library) {
                roundedPosition.x += 0.5f;
                roundedPosition.y += 0.8f;

                if (roundedPosition.x > RoomManager.libraryRightX) {
                    roundedPosition.x = RoomManager.libraryRightX;
                } else if (roundedPosition.x < RoomManager.libraryLeftX) {
                    roundedPosition.x = RoomManager.libraryLeftX;
                }

                if (roundedPosition.y > RoomManager.libraryTopY) {
                    roundedPosition.y = RoomManager.libraryTopY;
                } else if (roundedPosition.y < RoomManager.libraryBottomY) {
                    roundedPosition.y = RoomManager.libraryBottomY;
                }
            } else if (PlayerController.currentRoom == Room.Sunroom) {
                // redo values
                roundedPosition.x -= 0.41f;
                roundedPosition.y += 0.325f;

                if (roundedPosition.x > RoomManager.sunroomRightX - 0.53f) {
                    roundedPosition.x = RoomManager.sunroomRightX - 0.53f;
                } else if (roundedPosition.x < RoomManager.sunroomLeftX + 0.53f) {
                    roundedPosition.x = RoomManager.sunroomLeftX + 0.53f;
                }

                if (roundedPosition.y > RoomManager.sunroomTopY - 0.525f) {
                    roundedPosition.y = RoomManager.sunroomTopY - 0.525f;
                } else if (roundedPosition.y < RoomManager.sunroomBottomY + 0.525f) {
                    roundedPosition.y = RoomManager.sunroomBottomY + 0.525f;
                }
            } else {
                TimeManager.ExitEditMode();
                gameObject.SetActive(false);
            }
        } else {
            if (PlayerController.currentRoom == Room.Library) {
                roundedPosition.x += 0.5f;
                roundedPosition.y += 0.8f;

                if (roundedPosition.x > RoomManager.libraryRightX) {
                    roundedPosition.x = RoomManager.libraryRightX;
                } else if (roundedPosition.x < RoomManager.libraryLeftX) {
                    roundedPosition.x = RoomManager.libraryLeftX;
                }

                if (roundedPosition.y > RoomManager.libraryTopY) {
                    roundedPosition.y = RoomManager.libraryTopY;
                } else if (roundedPosition.y < RoomManager.libraryBottomY) {
                    roundedPosition.y = RoomManager.libraryBottomY;
                }
            } else if (PlayerController.currentRoom == Room.Sunroom) {
                roundedPosition.x -= 0.94f;
                roundedPosition.y += 0.8f;

                if (roundedPosition.x > RoomManager.sunroomRightX) {
                    roundedPosition.x = RoomManager.sunroomRightX;
                } else if (roundedPosition.x < RoomManager.sunroomLeftX) {
                    roundedPosition.x = RoomManager.sunroomLeftX;
                }

                if (roundedPosition.y > RoomManager.sunroomTopY) {
                    roundedPosition.y = RoomManager.sunroomTopY;
                } else if (roundedPosition.y < RoomManager.sunroomBottomY) {
                    roundedPosition.y = RoomManager.sunroomBottomY;
                }
            } else {
                TimeManager.ExitEditMode();
                gameObject.SetActive(false);
            }
        }

        transform.position = roundedPosition;
    }

    public void SetHighlightProps(bool isLarge, InventoryItemClass item)
    {
        highlightAnimator = GetComponent<Animator>();
        isLargeHighlight = isLarge;
        highlightAnimator.SetBool("IsLargeHighlight", isLargeHighlight);
        itemToPlace = item;
        transform.Find("Item").GetComponent<SpriteRenderer>().sprite = item.sprite;
    }
}
