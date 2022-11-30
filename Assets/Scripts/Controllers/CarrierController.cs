using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierController : MonoBehaviour
{
    public GameObject ownCat;
    public GameObject alertFrame;
    public int color;
    public bool canPickUp = false;
    public bool isPickedUp = false;
    public bool isCatInside = true;
    public bool isArriving = true;
    public bool isLeaving = false;
    public bool isWaitingForCat = false;

    public float leaveTimer = -1.0f;

    public GameObject bea;
    public GameObject carrierSpot;
    public GameObject leaveZone;

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
    GuestManager guestManager;

    void Start()
    {
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
        if (leaveTimer >= 0) {
            leaveTimer -= Time.deltaTime;
            if (leaveTimer < 0) {
                Leave();
            }
        }

        if (
            Input.GetKeyDown("c")
            && bea != null
            && !isPickedUp
            && canPickUp
        ) {
            PickUp();
        }

        if (bea != null) {
            if (
                (isPickedUp && isCatInside && isArriving && PlayerController.currentRoom != Room.Other)
                || (isPickedUp && !isCatInside && isArriving && carrierSpot != null)
                || (isPickedUp && !isCatInside && isLeaving && PlayerController.currentRoom == ownCat.GetComponent<CatController>().currentRoom)
                || (isPickedUp && isCatInside && isLeaving && leaveZone != null)
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

            transform.position = newPos;
        }
    }

    public void PassGuestManager(GuestManager reference) {
        guestManager = reference;
    }

    public void PickUp() {
        isPickedUp = true;
        transform.Find("Canvas/PickUp Tooltip").gameObject.SetActive(false);
        canPickUp = false;
        bea.GetComponent<PlayerController>().PickUp(gameObject);
        gameObject.layer = LayerMask.NameToLayer("Being Carried");
    }

    public void Place() {
        if (
            (isCatInside && isArriving && PlayerController.currentRoom != Room.Other)
            || (!isCatInside && isLeaving && PlayerController.currentRoom == ownCat.GetComponent<CatController>().currentRoom)
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

            transform.position = newPos;
            gameObject.layer = LayerMask.NameToLayer("Default");

            isWaitingForCat = true;
            transform.Find("Canvas/PickUp Tooltip").gameObject.SetActive(false);
            canPickUp = false;
            CatController ownCatController = ownCat.GetComponent<CatController>();

            if (isArriving) {
                ownCatController.currentRoom = PlayerController.currentRoom;
                ownCat.transform.position = newPos;
                ownCat.SetActive(true);
                ownCatController.WalkOutOfCarrier(bea.GetComponent<PlayerController>().lookDirection);
                isCatInside = false;
            } else if (isLeaving) {
                ownCatController.WalkIntoCarrier(new Vector2(
                    transform.position.x + bea.GetComponent<PlayerController>().lookDirection.x,
                    transform.position.y + bea.GetComponent<PlayerController>().lookDirection.y
                ));
                isCatInside = true;
            }
        } else if (!isCatInside && isArriving && carrierSpot != null) {
            bea.GetComponent<PlayerController>().Place();
            isPickedUp = false;
            transform.position = carrierSpot.transform.position;
            carrierSpot.GetComponent<SpotController>().isOccupied = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            alertFrame.SetActive(false);
            isArriving = false;
            bea = null;
            promptManager.OpenNotebookPrompt();
            TimeManager.ExitEditMode();
        } else if (isCatInside && isLeaving && leaveZone != null) {
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

            if (newPos.y < -10.52f) {
                newPos.y = -10.52f;
            }

            transform.position = newPos;
            gameObject.layer = LayerMask.NameToLayer("Default");
            transform.Find("Canvas/PickUp Tooltip").gameObject.SetActive(false);
            canPickUp = false;

            leaveTimer = 1.0f;
        }
    }

    public void CatIsDone() {
        isWaitingForCat = false;
        if (bea != null) {
            transform.Find("Canvas/PickUp Tooltip").gameObject.SetActive(true);
            canPickUp = true;
        }
    }

    public void GetCat(GameObject cat, GameObject alertFrameReference) {
        ownCat = cat;
        alertFrame = alertFrameReference;
    }

    public void Leave() {
        CatController catController = ownCat.GetComponent<CatController>();
        int moneyToAdd = (Mathf.RoundToInt((float)catController.purr/10) * catController.spoiledLevel * catController.stayLength) + (10 * catController.spoiledLevel);
        MoneyManager.DepositMoney(moneyToAdd);
        KittInnManager.AddCatStats(catController.purr);
        guestManager.DespawnCat(true, ownCat);

        alertFrame.SetActive(false);
        bea = null;
        promptManager.OpenNotebookPrompt();
        TimeManager.ExitEditMode();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (
            collider.gameObject.tag == "Bea"
            && (isArriving || isLeaving)
            && !isPickedUp && !isWaitingForCat
        ) {
            bea = collider.gameObject;
            promptManager = collider.gameObject.GetComponent<PlayerController>().promptManagerReference.GetComponent<PromptManager>();
            transform.Find("Canvas/PickUp Tooltip").gameObject.SetActive(true);
            canPickUp = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea" && !isPickedUp)
        {
            bea = null;
            transform.Find("Canvas/PickUp Tooltip").gameObject.SetActive(false);
            canPickUp = false;
        }
    }
}
