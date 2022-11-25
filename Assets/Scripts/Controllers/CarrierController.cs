using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierController : MonoBehaviour
{
    Rigidbody2D carrierRigidbody;
    public GameObject ownCat;
    public GameObject alertFrame;
    public int color;
    public bool canPickUp = false;
    public bool isPickedUp = false;
    public bool isCatInside = true;
    public bool isArriving = true;
    public bool isLeaving = false;

    public GameObject bea;
    public GameObject carrierSpot;

    Sprite forwardSprite;
    Sprite backwardSprite;
    Sprite sideSprite;

    public Sprite blueForwardSprite;
    public Sprite blueBackwardSprite;
    public Sprite blueSideSprite;
    public Sprite greenForwardSprite;
    public Sprite greenBackwardSprite;
    public Sprite greenSideSprite;
    public Sprite redForwardSprite;
    public Sprite redBackwardSprite;
    public Sprite redSideSprite;

    SpriteRenderer carrierRenderer;
    PromptManager promptManager;

    void Start()
    {
        carrierRigidbody = GetComponent<Rigidbody2D>();
        carrierRenderer = GetComponent<SpriteRenderer>();

        color = Random.Range(1, 4);
        if (color == 1) {
            forwardSprite = blueForwardSprite;
            backwardSprite = blueBackwardSprite;
            sideSprite = blueSideSprite;
        } else if (color == 2) {
            forwardSprite = greenForwardSprite;
            backwardSprite = greenBackwardSprite;
            sideSprite = greenSideSprite;
        } else {
            forwardSprite = redForwardSprite;
            backwardSprite = redBackwardSprite;
            sideSprite = redSideSprite;
        }

        carrierRenderer.sprite = sideSprite;
    }

    void Update()
    {
        if (
            Input.GetKeyDown("c")
            && bea != null
            && !isPickedUp
            && canPickUp
        ) {
            PickUp();
        } else if (
            Input.GetKeyDown("x")
            && bea != null
            && isPickedUp
            && PromptManager.currentActionSet == AvailableActionSet.PlacePrompt
        ) {
            Place();
        }

        if (bea != null) {
            if (
                (isPickedUp && isCatInside && isArriving && PlayerController.currentRoom != Room.Other)
                || (isPickedUp && !isCatInside && isArriving && carrierSpot != null)
            ) {
                promptManager.PlacePrompt();
            } else {
                promptManager.NoActions();
            }
        }

        if (isPickedUp) {
            if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(0.0f, -1.0f)) {
                carrierRenderer.sprite = forwardSprite;
                carrierRenderer.flipX = false;
            } else if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(-1.0f, 0.0f)) {
                carrierRenderer.sprite = sideSprite;
                carrierRenderer.flipX = false;
            } else if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(0.0f, 1.0f)) {
                carrierRenderer.sprite = backwardSprite;
                carrierRenderer.flipX = false;
            } else if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(1.0f, 0.0f)) {
                carrierRenderer.sprite = sideSprite;
                carrierRenderer.flipX = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isPickedUp) {
            Vector2 newPos = bea.GetComponent<Rigidbody2D>().position;
            
            if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(0.0f, -1.0f)) {
                newPos.y -= 0.2f;
            } else if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(-1.0f, 0.0f)) {
                newPos.x -= 0.5f;
                newPos.y += 0.5f;
            } else if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(0.0f, 1.0f)) {
                newPos.y += 0.7f;
            } else if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(1.0f, 0.0f)) {
                newPos.x += 0.5f;
                newPos.y += 0.5f;
            }

            carrierRigidbody.MovePosition(newPos);
        }
    }

    public void PickUp() {
        isPickedUp = true;
        transform.Find("Canvas/PickUp Tooltip").gameObject.SetActive(false);
        canPickUp = false;
        bea.GetComponent<PlayerController>().PickUp();
        gameObject.layer = LayerMask.NameToLayer("Being Carried");
    }

    public void Place() {
        if (
            isCatInside && isArriving
            && PlayerController.currentRoom != Room.Other
        ) {
            bea.GetComponent<PlayerController>().Place();
            isPickedUp = false;
            Vector2 newPos = new Vector2(Mathf.Round(bea.GetComponent<Rigidbody2D>().position.x), Mathf.Round(bea.GetComponent<Rigidbody2D>().position.y));

            if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(0.0f, -1.0f)) {
                newPos.y -= 1.0f;
            } else if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(-1.0f, 0.0f)) {
                newPos.x -= 1.0f;
            } else if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(0.0f, 1.0f)) {
                newPos.y += 1.0f;
            } else if (bea.GetComponent<PlayerController>().lookDirection == new Vector2(1.0f, 0.0f)) {
                newPos.x += 1.0f;
            }

            if (PlayerController.currentRoom == Room.Library) {
                newPos.x += 0.5f;
                newPos.y += 0.8f;

                if (newPos.x > RoomManager.libraryRightX) {
                    newPos.x = RoomManager.libraryRightX - 1.0f;
                } else if (newPos.x < RoomManager.libraryLeftX) {
                    newPos.x = RoomManager.libraryLeftX + 1.0f;
                }

                if (newPos.y > RoomManager.libraryTopY) {
                    newPos.y = RoomManager.libraryTopY - 1.0f;
                } else if (newPos.y < RoomManager.libraryBottomY) {
                    newPos.y = RoomManager.libraryBottomY + 1.0f;
                }
            } else if (PlayerController.currentRoom == Room.Sunroom) {
                newPos.x -= 0.94f;
                newPos.y += 0.8f;

                if (newPos.x > RoomManager.sunroomRightX) {
                    newPos.x = RoomManager.sunroomRightX - 1.0f;
                } else if (newPos.x < RoomManager.sunroomLeftX) {
                    newPos.x = RoomManager.sunroomLeftX + 1.0f;
                }

                if (newPos.y > RoomManager.sunroomTopY) {
                    newPos.y = RoomManager.sunroomTopY - 1.0f;
                } else if (newPos.y < RoomManager.sunroomBottomY) {
                    newPos.y = RoomManager.sunroomBottomY + 1.0f;
                }
            }

            carrierRigidbody.MovePosition(newPos);
            gameObject.layer = LayerMask.NameToLayer("Default");

            CatController ownCatController = ownCat.GetComponent<CatController>();
            ownCatController.currentRoom = PlayerController.currentRoom;
            ownCat.transform.position = newPos;
            ownCat.SetActive(true);
            ownCatController.WalkOutOfCarrier(bea.GetComponent<PlayerController>().lookDirection);
            isCatInside = false;
        } else if (!isCatInside && isArriving && carrierSpot != null) {
            bea.GetComponent<PlayerController>().Place();
            isPickedUp = false;
            carrierRigidbody.MovePosition(carrierSpot.transform.position);
            gameObject.layer = LayerMask.NameToLayer("Default");
            alertFrame.SetActive(false);
            isArriving = false;
            bea = null;
            promptManager.OpenNotebookPrompt();
            TimeManager.ExitEditMode();
        }
        // else if (
        //     !isCatInside && !isArriving
        //     && PlayerController.currentRoom == ownCat.GetComponent<CatController>().currentRoom
        // ) {
        //     trigger cat enter
        // }
    }

    public void GetCat(GameObject cat, GameObject alertFrameReference) {
        ownCat = cat;
        alertFrame = alertFrameReference;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (
            collider.gameObject.tag == "Bea"
            && (isArriving || isLeaving)
            && !isPickedUp
        ) {
            bea = collider.gameObject;
            promptManager = collider.gameObject.GetComponent<PlayerController>().promptManagerReference.GetComponent<PromptManager>();
            transform.Find("Canvas/PickUp Tooltip").gameObject.SetActive(true);
            canPickUp = true;
        } else if (
            collider.gameObject.tag == "Carrier Spot"
            && !isCatInside && isArriving && isPickedUp
        ) {
            carrierSpot = collider.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea" && !isPickedUp)
        {
            bea = null;
            transform.Find("Canvas/PickUp Tooltip").gameObject.SetActive(false);
            canPickUp = false;
        } else if (
            collider.gameObject.tag == "Carrier Spot"
            && !isCatInside && isArriving && isPickedUp
        ) {
            carrierSpot = null;
        }
    }
}
