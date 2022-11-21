using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHighlightController : MonoBehaviour
{
    public bool isLargeHighlight;
    public InventoryItemClass itemToPlace;

    Animator highlightAnimator;

    public GameObject promptManagerReference;
    PromptManager promptManager;

    void Start()
    {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
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
                roundedPosition.y += 0.3f;

                if (roundedPosition.x > RoomManager.libraryRightX - 0.5f) {
                    roundedPosition.x = RoomManager.libraryRightX - 0.5f;
                } else if (roundedPosition.x < RoomManager.libraryLeftX + 0.5f) {
                    roundedPosition.x = RoomManager.libraryLeftX + 0.5f;
                }

                if (roundedPosition.y > RoomManager.libraryTopY - 0.5f) {
                    roundedPosition.y = RoomManager.libraryTopY - 0.5f;
                } else if (roundedPosition.y < RoomManager.libraryBottomY + 0.5f) {
                    roundedPosition.y = RoomManager.libraryBottomY + 0.5f;
                }
            } else if (PlayerController.currentRoom == Room.Sunroom) {
                roundedPosition.x -= 0.44f;
                roundedPosition.y += 0.3f;

                if (roundedPosition.x > RoomManager.sunroomRightX - 0.5f) {
                    roundedPosition.x = RoomManager.sunroomRightX - 0.5f;
                } else if (roundedPosition.x < RoomManager.sunroomLeftX + 0.5f) {
                    roundedPosition.x = RoomManager.sunroomLeftX + 0.5f;
                }

                if (roundedPosition.y > RoomManager.sunroomTopY - 0.5f) {
                    roundedPosition.y = RoomManager.sunroomTopY - 0.5f;
                } else if (roundedPosition.y < RoomManager.sunroomBottomY + 0.5f) {
                    roundedPosition.y = RoomManager.sunroomBottomY + 0.5f;
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
                promptManager.OpenNotebookPrompt();
                TimeManager.ExitEditMode();
                gameObject.SetActive(false);
            }
        }

        transform.position = roundedPosition;

        if (
            Input.GetKeyDown("z")
            && (PromptManager.currentActionSet == AvailableActionSet.EditPickUpPrompts
                || PromptManager.currentActionSet == AvailableActionSet.EditPlacePrompts)
        )
        {
            promptManager.OpenNotebookPrompt();
            TimeManager.ExitEditMode();
            gameObject.SetActive(false);
        }
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
