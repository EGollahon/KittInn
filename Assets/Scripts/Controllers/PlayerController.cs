using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRigidbody;
    Animator playerAnimator;
    float walkSpeed = 3.0f;
    Vector2 movement = new Vector2(0.0f, 0.0f);
    public Vector2 lookDirection = new Vector2(0.0f, -1.0f);
    public static Room currentRoom;
    public bool isCarrying = false;
    public bool isPetting = false;
    public bool isSleeping = false;

    public GameObject catInRange;
    public GameObject carrierCarrying;

    public GameObject promptManagerReference;
    PromptManager promptManager;
    public GameObject mouseHighlight;

    void Start()
    {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerAnimator.SetFloat("LookX", 0.0f);
        playerAnimator.SetFloat("LookY", -1.0f);
    }

    void Update()
    {
        if (!isPetting && !isSleeping) {
            movement = Vector2.zero;
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");

            if (movement.x > 0.5f || movement.y > 0.5f || movement.x < -0.5f || movement.y < -0.5f) {
                playerAnimator.SetBool("IsWalking", true);

                if (movement.x > 0.5f && lookDirection.y == 0.0f) {
                    lookDirection.x = 1.0f;
                    lookDirection.y = 1.0f;
                } else if (movement.x < -0.5f && lookDirection.y == 0.0f) {
                    lookDirection.x = -1.0f;
                    lookDirection.y = 1.0f;
                } else {
                    lookDirection.x = 0.0f;
                }
                
                if (movement.y > 0.5f && lookDirection.x == 0.0f) {
                    lookDirection.y = 1.0f;
                } else if (movement.y < -0.5f && lookDirection.x == 0.0f) {
                    lookDirection.y = -1.0f;
                } else {
                    lookDirection.y = 0.0f;
                }

                playerAnimator.SetFloat("LookX", lookDirection.x);
                playerAnimator.SetFloat("LookY", lookDirection.y);
            } else {
                playerAnimator.SetBool("IsWalking", false);
            }
        } else if (isPetting) {
            if (catInRange == null || (catInRange.GetComponent<CatController>().activity != Activity.WaitingForPets && catInRange.GetComponent<CatController>().activity != Activity.BeingPetted)) {
                isPetting = false;
                playerAnimator.SetBool("IsPetting", false);
                promptManager.CatRoomsPrompts();
            }
        }

        if (
            transform.position.x > RoomManager.libraryLeftX - 0.5f && transform.position.x < RoomManager.libraryRightX + 0.5f
            && transform.position.y > RoomManager.libraryBottomY - 0.5f && transform.position.y < RoomManager.libraryTopY + 0.5f
        ) {
            currentRoom = Room.Library;
            if (PromptManager.currentActionSet == AvailableActionSet.OpenNotebookPrompt) {
                promptManager.CatRoomsPrompts();
            }
        } else if (
            transform.position.x > RoomManager.sunroomLeftX - 0.5f && transform.position.x < RoomManager.sunroomRightX + 0.5f
            && transform.position.y > RoomManager.sunroomBottomY - 0.5f && transform.position.y < RoomManager.sunroomTopY + 0.5f
        ) {
            currentRoom = Room.Sunroom;
            if (PromptManager.currentActionSet == AvailableActionSet.OpenNotebookPrompt) {
                promptManager.CatRoomsPrompts();
            }
        } else {
            currentRoom = Room.Other;
            if (
                PromptManager.currentActionSet == AvailableActionSet.CatRoomsPrompts
                || PromptManager.currentActionSet == AvailableActionSet.CatRoomsPromptsWithCats
            ) {
                promptManager.OpenNotebookPrompt();
            }
        }

        if (Input.GetKeyDown("q")
            && (PromptManager.currentActionSet == AvailableActionSet.CatRoomsPrompts
            || PromptManager.currentActionSet == AvailableActionSet.CatRoomsPromptsWithCats)
        ) {
            promptManager.EditCancelPrompt();
            mouseHighlight.SetActive(true);
            mouseHighlight.GetComponent<MouseHighlightController>().SetHighlightProps();
        } else if (Input.GetKeyDown("e") && catInRange != null) {
            if (
                PromptManager.currentActionSet == AvailableActionSet.StopPetting
                && catInRange.GetComponent<CatController>().activity == Activity.BeingPetted
            ) {
                isPetting = false;
                playerAnimator.SetBool("IsPetting", false);
                catInRange.GetComponent<CatController>().StopPettingCat();
            } else if (
                PromptManager.currentActionSet == AvailableActionSet.CatRoomsPromptsWithCats
                && catInRange.GetComponent<CatController>().activity == Activity.WaitingForPets
            ) {
                isPetting = true;
                playerAnimator.SetBool("IsPetting", true);
                catInRange.GetComponent<CatController>().StartPettingCat();
                promptManager.StopPetting();
            }
        } else if (
            Input.GetKeyDown("x")
            && carrierCarrying != null
            && isCarrying
            && PromptManager.currentActionSet == AvailableActionSet.PlacePrompt
        ) {
            carrierCarrying.GetComponent<CarrierController>().Place();
        }
    }

    void FixedUpdate()
    {
        Vector2 position = playerRigidbody.position;
        Vector2 newPos = position;

        newPos.x += walkSpeed * movement.x * Time.deltaTime;
        newPos.y += walkSpeed * movement.y * Time.deltaTime;

        playerRigidbody.MovePosition(newPos);
    }
    
    public void PickUp(GameObject newCarrier) {
        isCarrying = true;
        playerAnimator.SetBool("IsCarrying", true);
        carrierCarrying = newCarrier;
    }

    public void Place() {
        isCarrying = false;
        playerAnimator.SetBool("IsCarrying", false);
        carrierCarrying = null;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Cat") {
            if (
                (catInRange == null)
                || (collider.gameObject.GetComponent<CatController>().activity == Activity.WaitingForPets)
                || (collider.gameObject.GetComponent<CatController>().status != Status.Content && catInRange.GetComponent<CatController>().activity != Activity.WaitingForPets)
                || (collider.gameObject.GetComponent<CatController>().status == Status.Content && catInRange.GetComponent<CatController>().status == Status.Content)
            ) {
                catInRange = collider.gameObject;
            }

            if (collider.gameObject.GetComponent<CatController>().activity == Activity.WaitingForPets) {
                promptManager.CatRoomsPromptsWithCats();
            }
        } else if (
            collider.gameObject.tag == "Carrier Spot"
            && carrierCarrying != null
            && !carrierCarrying.GetComponent<CarrierController>().isCatInside
            && carrierCarrying.GetComponent<CarrierController>().isArriving
            && carrierCarrying.GetComponent<CarrierController>().isPickedUp
            && !collider.gameObject.GetComponent<SpotController>().isOccupied
        ) {
            carrierCarrying.GetComponent<CarrierController>().carrierSpot = collider.gameObject;
        } else if (
            collider.gameObject.tag == "Leave Zone"
            && carrierCarrying != null
            && carrierCarrying.GetComponent<CarrierController>().isCatInside
            && carrierCarrying.GetComponent<CarrierController>().isLeaving
            && carrierCarrying.GetComponent<CarrierController>().isPickedUp
        ) {
            carrierCarrying.GetComponent<CarrierController>().leaveZone = collider.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Cat")
        {
            promptManager.CatRoomsPrompts();
            catInRange = null;
        } else if (
            collider.gameObject.tag == "Carrier Spot"
            && carrierCarrying != null
            && !carrierCarrying.GetComponent<CarrierController>().isCatInside
            && carrierCarrying.GetComponent<CarrierController>().isArriving
            && carrierCarrying.GetComponent<CarrierController>().isPickedUp
        ) {
            carrierCarrying.GetComponent<CarrierController>().carrierSpot = null;
        } else if (
            collider.gameObject.tag == "Leave Zone"
            && carrierCarrying != null
            && carrierCarrying.GetComponent<CarrierController>().isCatInside
            && carrierCarrying.GetComponent<CarrierController>().isLeaving
            && carrierCarrying.GetComponent<CarrierController>().isPickedUp
        ) {
            carrierCarrying.GetComponent<CarrierController>().leaveZone = null;
        }
    }
}