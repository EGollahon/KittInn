using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CatController : MonoBehaviour
{
    float currentTime;
    public static float walkSpeed = 2.0f;
    public CatName catName;
    public CoatColor coat;
    public Personality personality;
    public int spoiledLevel;
    public Food favFood;
 
    public bool autonomous = false;
    public Status status = Status.Content;
    public Status lastStatus = Status.Content;
    public int purr = 50;
    public Activity activity;
    public GameObject interactingWith;
    public int dailyFeedings = 2;

    public GameObject carrier;
    public bool isCheckedIn = false;
    public float arrivalTime;
    public float checkOutTime;
    public int stayLength;

    public Sprite contentSprite;
    public Sprite hungrySprite;
    public Sprite thirstySprite;
    public Sprite disgustedSprite;
    public Sprite tiredSprite;
    public Sprite boredSprite;
    public Sprite lonelySprite;

    public float newStatusTime = 0.0f;
    public float satisfyTimer = -1.0f;

    public Room currentRoom;
    List<Tile> openList = new List<Tile>();
    List<Tile> closedList = new List<Tile>();
    public Vector2 targetLocation = new Vector2(0.0f, 0.0f);
    public int stepInPath = 1;
    class Tile
    {
        public Vector2 location;
        public Tile parentTile;
        public int gScore;
        public int hScore;
        public int fScore;
    }

    void Start()
    {

    }

    void Update()
    {
        if (status == Status.Content && autonomous) {
            Debug.Log("if youre content");
            if (TimeManager.timeOfDay == TimeOfDay.Night && activity != Activity.Sleeping) {
                Debug.Log("1");
                GetNewStatus(true);
                newStatusTime = 7.0f;
            } else if (TimeManager.timeOfDay == TimeOfDay.Night && newStatusTime != 7.0f) {
                Debug.Log("2");
                newStatusTime = 7.0f;
            } else if (TimeManager.time == newStatusTime) {
                Debug.Log("3");
                GetNewStatus(false);
            }
            Debug.Log("end content");
        }

        if (activity == Activity.Moving) {
            Debug.Log("moving");
            int xInput = (int)(closedList[stepInPath].location.x - closedList[stepInPath - 1].location.x);
            int yInput = (int)(closedList[stepInPath].location.y - closedList[stepInPath - 1].location.y);

            float newXPosition = transform.position.x + (walkSpeed * xInput * Time.deltaTime);
            float newYPosition = transform.position.y + (walkSpeed * yInput * Time.deltaTime);
            transform.position = new Vector2(newXPosition, newYPosition);

            if (
                Mathf.Abs(closedList[stepInPath].location.x - transform.position.x) <= 0.05
                && Mathf.Abs(closedList[stepInPath].location.y - transform.position.y) <= 0.05
            ) {
                transform.position = closedList[stepInPath].location;
                if (stepInPath < (closedList.Count - 1)) {
                    stepInPath++;
                } else {
                    if (status != Status.Lonely) {
                        ReachItem();
                    } else {
                        activity = Activity.WaitingForPets;
                    }
                }
            }
        } else if (activity == Activity.WaitingForUnoccupied && status != Status.Content) {
            GetItemLocation();
        } else if (activity == Activity.WaitingForUses && status != Status.Content && interactingWith != null) {
            ReachItem();
        }

        if (satisfyTimer >= 0) {
            Debug.Log("satisfy");
            satisfyTimer -= Time.deltaTime;
            if (satisfyTimer < 0) {
                SatisfyCat();
            }
        }

        if (TimeManager.time != currentTime) {
            Debug.Log("cat time change");
            currentTime = TimeManager.time;

            if (activity == Activity.WaitingForUnoccupied || activity == Activity.WaitingForUses || activity == Activity.WaitingForPets) {
                SubtractPurr(2);
            } else if (activity == Activity.Sleeping || activity == Activity.Playing || activity == Activity.BeingPetted) {
                AddPurr(2);
            }
        }
    }

    void AddPurr(int amount) {
        if (purr + amount <= 100) {
            purr += amount;
        } else {
            purr = 100;
        }
        RefreshTooltip();
    }

    void SubtractPurr(int amount) {
        if (purr - amount >= 0) {
            purr -= amount;
        } else {
            purr = 0;
        }
        RefreshTooltip();
    }

    void ReachItem() {
        autonomous = true;
        bool isSatisfied = true;

        if (interactingWith != null && (status == Status.Hungry || status == Status.Thirsty || status == Status.Disgusted)) {
            isSatisfied = interactingWith.GetComponent<ItemController>().CatInteract();
        }

        if (isSatisfied) {
            satisfyTimer = 1.0f;
            switch (status) {
                case Status.Hungry:
                    activity = Activity.Eating;
                    break;
                case Status.Thirsty:
                    activity = Activity.Drinking;
                    break;
                case Status.Disgusted:
                    activity = Activity.UsingLitterBox;
                    if (interactingWith != null) {
                        transform.position = interactingWith.transform.position;
                    }
                    break;
                case Status.Tired:
                    activity = Activity.Sleeping;
                    if (interactingWith != null) {
                        transform.position = interactingWith.transform.position;
                    }
                    break;
                case Status.Bored:
                    activity = Activity.Playing;
                    break;
                case Status.Lonely:
                    activity = Activity.BeingPetted;
                    break;
                default:
                    activity = Activity.Waiting;
                    break;
            }
        } else {
            activity = Activity.WaitingForUses;
        }
    }

    void GetPath()
    {
        if (targetLocation.x != transform.position.x || targetLocation.y != transform.position.y) {
            openList.Clear();
            closedList.Clear();
            stepInPath = 1;
            Vector2 currentTileLocation = new Vector2(transform.position.x, transform.position.y);
            Tile currentTile = CreateNewTile(currentTileLocation, null, true);
            closedList.Add(currentTile);

            AddNewTiles();

            activity = Activity.Moving;
        } else {
            ReachItem();
        }
    }

    void CheckOpenTiles() {
        Tile bestTile = openList[0];
        
        for (int i = 1; i < openList.Count; i++) {
            if (openList[i].fScore <= bestTile.fScore) {
                bestTile = openList[i];
            }
        }

        closedList.Add(bestTile);
        openList.RemoveAt(openList.IndexOf(bestTile));

        if (bestTile.hScore != 0) {
            AddNewTiles();
        }
    }

    void AddNewTiles() {
        Tile parentTile = closedList[closedList.Count - 1];
        List<Vector2> locationsToAdd = new List<Vector2>();

        locationsToAdd.Add(new Vector2(parentTile.location.x + 1.0f, parentTile.location.y));
        locationsToAdd.Add(new Vector2(parentTile.location.x - 1.0f, parentTile.location.y));
        locationsToAdd.Add(new Vector2(parentTile.location.x, parentTile.location.y + 1.0f));
        locationsToAdd.Add(new Vector2(parentTile.location.x, parentTile.location.y - 1.0f));

        for (int i = 0; i < locationsToAdd.Count; i++) {
            if (
                !closedList.Exists(e => e.location == locationsToAdd[i])
                && !openList.Exists(e => e.location == locationsToAdd[i])
            ) {
                openList.Add(CreateNewTile(locationsToAdd[i], parentTile, false));
            }
        }

        CheckOpenTiles();
    }

    Tile CreateNewTile(Vector2 loc, Tile parent, bool isCurrentTile) {
        Tile tile = new Tile();
        tile.location = loc;

        if (isCurrentTile) {
            tile.parentTile = parent;
            tile.gScore = 0;
        } else {
            tile.parentTile = parent;
            tile.gScore = parent.gScore + 1;
        }

        float xDifference = Mathf.Abs(tile.location.x - targetLocation.x);
        float yDifference = Mathf.Abs(tile.location.y - targetLocation.y);
        tile.hScore = (int)(xDifference + yDifference);

        tile.fScore = tile.gScore + tile.hScore;

        return tile;
    }

    void GetNewStatus(bool isSleep)
    {
        Debug.Log("get new status " + TimeManager.time.ToString());
        bool isStatusValid = false;

        if (!isSleep) {
            while (!isStatusValid) {
                int startIndex = 1;
                if (dailyFeedings < 1) {
                    startIndex = 2;
                }

                int newStatus = Random.Range(startIndex, (int)Status.Count);
                if (newStatus != (int)lastStatus) {
                    status = (Status)newStatus;
                    isStatusValid = true;
                }
                
            }
        } else {
            status = Status.Tired;
            isStatusValid = true;
        }

        transform.Find("Canvas/Status Thought/Image").gameObject.GetComponent<Image>().sprite = GetStatusSprite();
        RefreshTooltip();
        GetItemLocation();
    }

    void GetItemLocation() {
        activity = Activity.Checking;
        InventoryType neededType = InventoryType.None;
        switch (status) {
            case Status.Hungry:
                neededType = InventoryType.FoodDish;
                break;
            case Status.Thirsty:
                neededType = InventoryType.WaterDish;
                break;
            case Status.Disgusted:
                neededType = InventoryType.LitterBox;
                break;
            case Status.Tired:
                neededType = InventoryType.Bed;
                break;
            case Status.Bored:
                neededType = InventoryType.Toy;
                break;
            case Status.Lonely:
                neededType = InventoryType.None;
                break;
            default:
                neededType = InventoryType.None;
                break;
        }
        if (neededType != InventoryType.None) {
            GameObject newInteractingWith = RoomManager.RetrieveItemForCat(currentRoom, neededType, transform.position);

            if (newInteractingWith != null) {
                Vector2 newTarget = new Vector2(0.0f, 0.0f);

                for (int i = 0; i < newInteractingWith.GetComponent<ItemController>().entryPoints.Count; i++) {
                    if (newTarget.x == 0.0f && newTarget.y == 0.0f) {
                        newTarget = newInteractingWith.GetComponent<ItemController>().entryPoints[i];
                    } else {
                        int distanceToOldTile =
                            (int)(Mathf.Abs(transform.position.x - newTarget.x) + Mathf.Abs(transform.position.y - newTarget.y));
                        int distanceToNewTile =
                            (int)(Mathf.Abs(transform.position.x - newInteractingWith.GetComponent<ItemController>().entryPoints[i].x)
                                + Mathf.Abs(transform.position.y - newInteractingWith.GetComponent<ItemController>().entryPoints[i].y));
                        if (distanceToNewTile < distanceToOldTile) {
                            newTarget = newInteractingWith.GetComponent<ItemController>().entryPoints[i];
                        }
                    }
                }

                if (interactingWith != null) {
                    interactingWith.GetComponent<ItemController>().isOccupied = false;
                }
                newInteractingWith.GetComponent<ItemController>().isOccupied = true;
                interactingWith = newInteractingWith;
                if (lastStatus == Status.Tired) {
                    transform.position = targetLocation;
                }
                targetLocation = newTarget;
                GetPath();
            } else {
                activity = Activity.WaitingForUnoccupied;
            }
        } else {
            if (interactingWith != null) {
                interactingWith.GetComponent<ItemController>().isOccupied = false;
            }
            if (lastStatus == Status.Tired) {
                transform.position = targetLocation;
            }
            targetLocation = RoomManager.RetrieveEmptySpace(currentRoom);
            if (interactingWith != null) {
                interactingWith.GetComponent<ItemController>().isOccupied = false;
            }
            GetPath();
        }
    }

    public void SatisfyCat()
    {
        if (interactingWith != null) {
            interactingWith.GetComponent<ItemController>().CatFinishInteract();
        }
        lastStatus = status;
        status = Status.Content;
        transform.Find("Canvas/Status Thought/Image").gameObject.GetComponent<Image>().sprite = GetStatusSprite();
        RefreshTooltip();

        if (
            activity == Activity.Sleeping
            || activity == Activity.Playing
            || activity == Activity.BeingPetted
        ) {
            int nextStatusSwitch = Random.Range(2, 13);
            newStatusTime = TimeManager.time + (0.25f * nextStatusSwitch);
        } else if (TimeManager.timeOfDay == TimeOfDay.Night && activity == Activity.Sleeping) {
            newStatusTime = 24.0f;
        } else {
            if (activity == Activity.Eating) {
                dailyFeedings--;
                if (interactingWith != null) {
                    if (interactingWith.GetComponent<ItemController>().filledFood == favFood) {
                        AddPurr(10);
                    } else {
                        AddPurr(5);
                    }
                }
            } else if (activity == Activity.Drinking || activity == Activity.UsingLitterBox) {
                AddPurr(10);
            }
            GetNewStatus(false);
        }
    }

    public void StartPettingCat()
    {
        ReachItem();
    }

    public void StopPettingCat()
    {
        GetNewStatus(false);
    }

    public void WalkOutOfCarrier(Vector2 carrierLookDirection) {
        targetLocation = new Vector2(transform.position.x + carrierLookDirection.x, transform.position.y + carrierLookDirection.y);
        Tile currentTile = CreateNewTile(new Vector2(transform.position.x, transform.position.y), null, true);
        closedList.Add(currentTile);
        closedList.Add(CreateNewTile(targetLocation, currentTile, false));
        activity = Activity.Moving;
        isCheckedIn = true;
    }

    public void IntializeCat(
        CatName newCatName,
        CoatColor newCoat,
        Personality newPersonality,
        int newSpoiledLevel,
        Food newFavFood,
        int newStayLength,
        GameObject newCarrier
    )
    {
        catName = newCatName;
        coat = newCoat;
        personality = newPersonality;
        spoiledLevel = newSpoiledLevel;
        favFood = newFavFood;
        stayLength = newStayLength;
        carrier = newCarrier;

        transform.Find("Canvas/Cat Tooltip/Name").gameObject.GetComponent<TextMeshProUGUI>().text = newCatName.ToString();
        transform.Find("Canvas/Status Thought/Image").gameObject.GetComponent<Image>().sprite = GetStatusSprite();
        RefreshTooltip();
    }

    public Sprite GetStatusSprite() {
        switch (status) {
            case Status.Content:
                return contentSprite;
            case Status.Hungry:
                return hungrySprite;
            case Status.Thirsty:
                return thirstySprite;
            case Status.Disgusted:
                return disgustedSprite;
            case Status.Tired:
                return tiredSprite;
            case Status.Bored:
                return boredSprite;
            case Status.Lonely:
                return lonelySprite;
            default:
                return contentSprite;
        }
    }

    void RefreshTooltip() {
        transform.Find("Canvas/Cat Tooltip/Status Bubble/Image").gameObject.GetComponent<Image>().sprite = GetStatusSprite();
        transform.Find("Canvas/Cat Tooltip/Status").gameObject.GetComponent<TextMeshProUGUI>().text = status.ToString();
        float newMaskWidth = (1.5625f * ((float)purr / 100.0f)) + 0.03125f;
        transform.Find("Canvas/Cat Tooltip/Purr Meter Mask").gameObject.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newMaskWidth);
        transform.Find("Canvas/Cat Tooltip/Purr Percent").gameObject.GetComponent<TextMeshProUGUI>().text = purr.ToString() + "%";
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea")
        {
            RefreshTooltip();
            transform.Find("Canvas/Cat Tooltip").gameObject.SetActive(true);
            transform.Find("Canvas/Status Thought").gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea")
        {
            transform.Find("Canvas/Cat Tooltip").gameObject.SetActive(false);
            transform.Find("Canvas/Status Thought").gameObject.SetActive(true);
        }
    }
}
