using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHighlightController : MonoBehaviour
{
    public bool isLarge = false;
    public bool hasItem = false;
    public bool isOverlapping = false;
    public bool isDeactivating = false;
    public bool canPlace = true;
    public InventoryItemClass itemToPlace;
    public GameObject itemToMove;
    public GameObject hoveredItem;
    public List<Vector2> locations = new List<Vector2>();
    public List<Vector2> entryPoints = new List<Vector2>();
    public GameObject itemPrefab;

    Animator highlightAnimator;
    Rigidbody2D highlightRigidbody;
    public GameObject promptManagerReference;
    PromptManager promptManager;
    public GameObject inventoryManagerReference;
    InventoryManager inventoryManager;

    void Awake()
    {
        StuffThatShouldWork();
    }

    void StuffThatShouldWork() {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
        inventoryManager = inventoryManagerReference.GetComponent<InventoryManager>();
        highlightAnimator = GetComponent<Animator>();
        highlightRigidbody = GetComponent<Rigidbody2D>();

        isOverlapping = false;
        isDeactivating = false;
        hoveredItem = null;
        locations.Clear();
        entryPoints.Clear();
    }

    void Update()
    {
        canPlace = true;
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 roundedPosition = new Vector2(Mathf.Round(worldPosition.x), Mathf.Round(worldPosition.y));
        highlightRigidbody.MovePosition(MoveHighlight(roundedPosition));

        if (
            Input.GetKeyDown("z")
            && (PromptManager.currentActionSet == AvailableActionSet.EditPickUpPrompts
                || PromptManager.currentActionSet == AvailableActionSet.EditPlacePrompts
                || PromptManager.currentActionSet == AvailableActionSet.EditCancelPrompt)
        )
        {
            if (itemToMove != null && itemToPlace == null) {
                itemToMove.SetActive(true);
            }

            isDeactivating = true;
            promptManager.OpenNotebookPrompt();
            TimeManager.ExitEditMode();
            gameObject.SetActive(false);
        } else if (Input.GetKeyDown("x") && PromptManager.currentActionSet == AvailableActionSet.EditPickUpPrompts)
        {
            canPlace = false;
            PickUpItem();
        } else if (Input.GetKeyDown("q") && PromptManager.currentActionSet == AvailableActionSet.EditPickUpPrompts)
        {
            RemoveItem();
        }

        if (hasItem && !isOverlapping && !isDeactivating) {
            entryPoints.Clear();
            locations.Clear();

            if (isLarge) {
                CheckEntryPoint(new Vector2(highlightRigidbody.position.x - 0.5f, highlightRigidbody.position.y + 0.5f), new Vector2(-1.0f, 0.0f));
                CheckEntryPoint(new Vector2(highlightRigidbody.position.x - 0.5f, highlightRigidbody.position.y + 0.5f), new Vector2(0.0f, 1.0f));
                CheckEntryPoint(new Vector2(highlightRigidbody.position.x + 0.5f, highlightRigidbody.position.y + 0.5f), new Vector2(0.0f, 1.0f));
                CheckEntryPoint(new Vector2(highlightRigidbody.position.x + 0.5f, highlightRigidbody.position.y + 0.5f), new Vector2(1.0f, 0.0f));
                CheckEntryPoint(new Vector2(highlightRigidbody.position.x - 0.5f, highlightRigidbody.position.y - 0.5f), new Vector2(-1.0f, 0.0f));
                CheckEntryPoint(new Vector2(highlightRigidbody.position.x - 0.5f, highlightRigidbody.position.y - 0.5f), new Vector2(0.0f, -1.0f));
                CheckEntryPoint(new Vector2(highlightRigidbody.position.x + 0.5f, highlightRigidbody.position.y - 0.5f), new Vector2(0.0f, -1.0f));
                CheckEntryPoint(new Vector2(highlightRigidbody.position.x + 0.5f, highlightRigidbody.position.y - 0.5f), new Vector2(1.0f, 0.0f));
                locations.Add(new Vector2(highlightRigidbody.position.x - 0.5f, highlightRigidbody.position.y + 0.5f));
                locations.Add(new Vector2(highlightRigidbody.position.x + 0.5f, highlightRigidbody.position.y + 0.5f));
                locations.Add(new Vector2(highlightRigidbody.position.x - 0.5f, highlightRigidbody.position.y - 0.5f));
                locations.Add(new Vector2(highlightRigidbody.position.x + 0.5f, highlightRigidbody.position.y - 0.5f));
            } else {
                locations.Add(highlightRigidbody.position);
                Vector2[] pointsToRaycast = {new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f), new Vector2(0.0f, -1.0f), new Vector2(-1.0f, 0.0f)};
                for (int i = 0; i < pointsToRaycast.Length; i++) {
                    CheckEntryPoint(highlightRigidbody.position, pointsToRaycast[i]);
                }
            }

            bool notBlockingEntries = true;
            List<GameObject> roomToCheck;
            if (PlayerController.currentRoom == Room.Sunroom) {
                roomToCheck = RoomManager.sunroomItems;
            } else {
                roomToCheck = RoomManager.libraryItems;
            }
            for (int i = 0; i < roomToCheck.Count; i++) {
                bool isBlockingItem = true;
                List<Vector2> itemToCheck = roomToCheck[i].GetComponent<ItemController>().entryPoints;
                for (int j = 0; j < itemToCheck.Count; j++) {
                    if (!locations.Exists(element => element == itemToCheck[j])) {
                        isBlockingItem = false;
                        break;
                    }
                }

                if (isBlockingItem) {
                    notBlockingEntries = false;
                    break;
                }
            }

            if (entryPoints.Count > 0 && notBlockingEntries && canPlace) {
                promptManager.EditPlacePrompts();

                if (Input.GetKeyDown("x"))
                {
                    if (itemToMove == null && itemToPlace != null) {
                        GameObject newItem = Instantiate(itemPrefab, highlightRigidbody.position, Quaternion.identity);
                        newItem.GetComponent<ItemController>().InitializeItem(itemToPlace, locations, entryPoints);
                        itemToMove = newItem;
                        if (PlayerController.currentRoom == Room.Library) {
                            RoomManager.AddItemToRoom(itemToMove, Room.Library);
                        } else if (PlayerController.currentRoom == Room.Sunroom) {
                            RoomManager.AddItemToRoom(itemToMove, Room.Sunroom);
                        }
                        inventoryManager.RemoveInventoryItems(itemToPlace.itemName, 1);
                    } else if (itemToMove != null) {
                        List<Vector2> oldLocations = itemToMove.GetComponent<ItemController>().locations;
                        for (int i = 0; i < roomToCheck.Count; i++) {
                            List<Vector2> itemToCheck = roomToCheck[i].GetComponent<ItemController>().locations;
                            for (int j = 0; j < oldLocations.Count; j++) {
                                if (
                                    itemToCheck.Exists(element => element == new Vector2(oldLocations[j].x, oldLocations[j].y + 1.0f))
                                    || itemToCheck.Exists(element => element == new Vector2(oldLocations[j].x, oldLocations[j].y - 1.0f))
                                    || itemToCheck.Exists(element => element == new Vector2(oldLocations[j].x + 1.0f, oldLocations[j].y))
                                    || itemToCheck.Exists(element => element == new Vector2(oldLocations[j].x - 1.0f, oldLocations[j].y))
                                ) {
                                    roomToCheck[i].GetComponent<ItemController>().entryPoints.Add(oldLocations[i]);
                                }
                            }
                        }

                        itemToMove.GetComponent<ItemController>().Move(locations, entryPoints);
                        itemToMove.SetActive(true);
                    }

                    for (int i = 0; i < roomToCheck.Count; i++) {
                        List<Vector2> itemToCheck = roomToCheck[i].GetComponent<ItemController>().entryPoints;
                        List<Vector2> currentLocations = itemToMove.GetComponent<ItemController>().locations;
                        for (int j = 0; j < currentLocations.Count; j++) {
                            int index = itemToCheck.FindIndex(element => element == currentLocations[j]);
                            if (index > -1) {
                                itemToCheck.Remove(itemToCheck[index]);
                            }
                        }
                    }

                    isDeactivating = true;
                    promptManager.OpenNotebookPrompt();
                    TimeManager.ExitEditMode();
                    gameObject.SetActive(false);
                }
            }
            else {
                promptManager.EditCancelPrompt();
            }
        }
        else if (!hasItem && isOverlapping && !isDeactivating) {
            promptManager.EditPickUpPrompts();
        }
        else if (!isDeactivating) {
            promptManager.EditCancelPrompt();
        }
    }

    public void CheckEntryPoint(Vector2 origin, Vector2 direction) {
        int blockEntryPointLayerIndex = LayerMask.NameToLayer("Block Entry Point");
        int layerMask = (1 << blockEntryPointLayerIndex);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 1.0f, layerMask);
        if (hit.collider == null)
        {
            entryPoints.Add(new Vector2(
                (Mathf.Round((origin.x + direction.x) * 100)) / 100.0f,
                (Mathf.Round((origin.y + direction.y) * 100)) / 100.0f
            ));
        }
    }

    void PickUpItem() {
        itemToMove = hoveredItem;
        itemToMove.SetActive(false);
        transform.Find("Item").GetComponent<SpriteRenderer>().sprite = itemToMove.GetComponent<ItemController>().sprite;
        hasItem = true;

        if (itemToMove.GetComponent<ItemController>().type == InventoryType.Bed) {
            isLarge = true;
            highlightAnimator.SetBool("IsLargeHighlight", true);
        } else {
            isLarge = false;
            highlightAnimator.SetBool("IsLargeHighlight", false);
        }
    }

    void RemoveItem() {
        List<Vector2> hoveredLocations = hoveredItem.GetComponent<ItemController>().locations;
        List<GameObject> roomToCheck;
        if (PlayerController.currentRoom == Room.Sunroom) {
            roomToCheck = RoomManager.sunroomItems;
        } else {
            roomToCheck = RoomManager.libraryItems;
        }
        for (int i = 0; i < roomToCheck.Count; i++) {
            List<Vector2> itemToCheck = roomToCheck[i].GetComponent<ItemController>().locations;
            for (int j = 0; j < hoveredLocations.Count; j++) {
                if (
                    itemToCheck.Exists(element => element == new Vector2(hoveredLocations[j].x, hoveredLocations[j].y + 1.0f))
                    || itemToCheck.Exists(element => element == new Vector2(hoveredLocations[j].x, hoveredLocations[j].y - 1.0f))
                    || itemToCheck.Exists(element => element == new Vector2(hoveredLocations[j].x + 1.0f, hoveredLocations[j].y))
                    || itemToCheck.Exists(element => element == new Vector2(hoveredLocations[j].x - 1.0f, hoveredLocations[j].y))
                ) {
                    itemToCheck.Add(hoveredLocations[i]);
                }
            }
        }

        inventoryManager.AddInventoryItems(hoveredItem.GetComponent<ItemController>().itemName, 1);
        if (PlayerController.currentRoom == Room.Library) {
            RoomManager.RemoveItemFromRoom(hoveredItem, Room.Library);
        } else if (PlayerController.currentRoom == Room.Sunroom) {
            RoomManager.RemoveItemFromRoom(hoveredItem, Room.Sunroom);
        }

        isDeactivating = true;
        promptManager.OpenNotebookPrompt();
        TimeManager.ExitEditMode();
        gameObject.SetActive(false);
    }

    public void SetHighlightProps(InventoryItemClass item)
    {
        StuffThatShouldWork();

        itemToPlace = item;
        itemToMove = null;
        transform.Find("Item").GetComponent<SpriteRenderer>().sprite = item.sprite;
        hasItem = true;

        if (item.type == InventoryType.Bed) {
            isLarge = true;
            highlightAnimator.SetBool("IsLargeHighlight", true);
        } else {
            isLarge = false;
            highlightAnimator.SetBool("IsLargeHighlight", false);
        }
    }

    public void SetHighlightProps()
    {
        StuffThatShouldWork();

        itemToPlace = null;
        itemToMove = null;
        transform.Find("Item").GetComponent<SpriteRenderer>().sprite = null;
        hasItem = false;
        isLarge = false;
        highlightAnimator.SetBool("IsLargeHighlight", false);
    }

    Vector2 MoveHighlight(Vector2 position) {
        Vector2 roundedPosition = position;

        if (isLarge) {
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
                isDeactivating = true;
                promptManager.OpenNotebookPrompt();
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
                isDeactivating = true;
                promptManager.OpenNotebookPrompt();
                TimeManager.ExitEditMode();
                gameObject.SetActive(false);
            }
        }

        return roundedPosition;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Item")
        {
            isOverlapping = true;
            hoveredItem = collider.gameObject;

            if (!hasItem && hoveredItem.GetComponent<ItemController>().type == InventoryType.Bed) {
                isLarge = true;
                highlightAnimator.SetBool("IsLargeHighlight", true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Item")
        {
            isOverlapping = false;
            hoveredItem = null;

            if (!hasItem && isLarge) {
                isLarge = false;
                highlightAnimator.SetBool("IsLargeHighlight", false);
            }
        }
    }
}