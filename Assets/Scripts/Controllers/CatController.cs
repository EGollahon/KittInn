using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CatController : MonoBehaviour
{
    Animator catAnimator;
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
    public Activity activity = Activity.Waiting;
    public GameObject interactingWith;
    public int dailyFeedings = 2;

    public GameObject carrier;
    public bool isCheckedIn = false;
    public float arrivalTime;
    public float checkOutTime;
    public int stayLength;
    public int daysLeft;

    public Sprite contentSprite;
    public Sprite hungrySprite;
    public Sprite thirstySprite;
    public Sprite disgustedSprite;
    public Sprite tiredSprite;
    public Sprite boredSprite;
    public Sprite lonelySprite;

    public bool isLeaving = false;
    
    public float newStatusTime = 0.0f;
    public float satisfyTimer = -1.0f;

    public Room currentRoom;
    public Vector2 lookDirection = new Vector2(0.0f, -1.0f);
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

    void Awake()
    {
        catAnimator = GetComponent<Animator>();
        catAnimator.SetFloat("Look X", 0.0f);
        catAnimator.SetFloat("Look Y", -1.0f);
        catAnimator.SetBool("IsIdle", true);
    }

    void Update()
    {
        if (status == Status.Content && autonomous) {
            if (TimeManager.timeOfDay == TimeOfDay.Night && activity != Activity.Sleeping) {
                GetNewStatus(true);
                newStatusTime = 7.0f;
            } else if (TimeManager.timeOfDay == TimeOfDay.Night && newStatusTime != 7.0f) {
                newStatusTime = 7.0f;
            } else if (TimeManager.time == newStatusTime) {
                GetNewStatus(false);
            }
        }

        if (activity == Activity.Moving) {
            int xInput = (int)(closedList[stepInPath].location.x - closedList[stepInPath - 1].location.x);
            int yInput = (int)(closedList[stepInPath].location.y - closedList[stepInPath - 1].location.y);

            if (xInput > 0.5f && lookDirection.y == 0.0f) {
                    lookDirection.x = 1.0f;
                    lookDirection.y = 1.0f;
                } else if (xInput < -0.5f && lookDirection.y == 0.0f) {
                    lookDirection.x = -1.0f;
                    lookDirection.y = 1.0f;
                } else {
                    lookDirection.x = 0.0f;
                }
                
                if (yInput > 0.5f && lookDirection.x == 0.0f) {
                    lookDirection.y = 1.0f;
                } else if (yInput < -0.5f && lookDirection.x == 0.0f) {
                    lookDirection.y = -1.0f;
                } else {
                    lookDirection.y = 0.0f;
                }

                catAnimator.SetFloat("Look X", lookDirection.x);
                catAnimator.SetFloat("Look Y", lookDirection.y);

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
                    if (isLeaving && (targetLocation.x != carrier.transform.position.x ||targetLocation.y != carrier.transform.position.y)) {
                        targetLocation = carrier.transform.position;
                        GetPath();
                    } else if (isLeaving) {
                        carrier.GetComponent<CarrierController>().CatIsDone();
                        gameObject.SetActive(false);
                    } else if (status != Status.Lonely) {
                        ReachItem();
                    } else {
                        activity = Activity.WaitingForPets;
                        catAnimator.SetBool("IsSitting", true);
                    }
                }
            }
        } else if (activity == Activity.WaitingForUnoccupied && status != Status.Content) {
            GetItemLocation();
        } else if (activity == Activity.WaitingForUses && status != Status.Content && interactingWith != null) {
            ReachItem();
        }

        if (satisfyTimer >= 0) {
            satisfyTimer -= Time.deltaTime;
            if (satisfyTimer < 0) {
                SatisfyCat();
            }
        }

        if (TimeManager.time != currentTime) {
            currentTime = TimeManager.time;

            if (activity == Activity.WaitingForUnoccupied || activity == Activity.WaitingForUses || activity == Activity.WaitingForPets) {
                SubtractPurr(1);
            } else if (activity == Activity.Sleeping || activity == Activity.Playing || activity == Activity.BeingPetted) {
                if (interactingWith != null && interactingWith.GetComponent<ItemController>().level >= spoiledLevel) {
                    AddPurr(2);
                } else {
                    AddPurr(1);
                }
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
        carrier.GetComponent<CarrierController>().CatIsDone();
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
                    catAnimator.SetBool("IsCrouching", true);
                    if (interactingWith != null) {
                        lookDirection.x = interactingWith.GetComponent<ItemController>().locations[0].x - transform.position.x;
                        lookDirection.y = interactingWith.GetComponent<ItemController>().locations[0].y - transform.position.y;
                        catAnimator.SetFloat("Look X", lookDirection.x);
                        catAnimator.SetFloat("Look Y", lookDirection.y);
                    }
                    break;
                case Status.Thirsty:
                    activity = Activity.Drinking;
                    catAnimator.SetBool("IsCrouching", true);
                    if (interactingWith != null) {
                        lookDirection.x = interactingWith.GetComponent<ItemController>().locations[0].x - transform.position.x;
                        lookDirection.y = interactingWith.GetComponent<ItemController>().locations[0].y - transform.position.y;
                        catAnimator.SetFloat("Look X", lookDirection.x);
                        catAnimator.SetFloat("Look Y", lookDirection.y);
                    }
                    break;
                case Status.Disgusted:
                    activity = Activity.UsingLitterBox;
                    catAnimator.SetBool("IsSitting", true);
                    if (interactingWith != null) {
                        transform.position = interactingWith.transform.position;
                    }
                    break;
                case Status.Tired:
                    activity = Activity.Sleeping;
                    catAnimator.SetBool("IsSleeping", true);
                    if (interactingWith != null) {
                        transform.position = interactingWith.transform.position;
                    }
                    break;
                case Status.Bored:
                    activity = Activity.Playing;
                    catAnimator.SetBool("IsPlaying", true);
                    if (interactingWith != null) {
                        lookDirection.x = interactingWith.GetComponent<ItemController>().locations[0].x - transform.position.x;
                        lookDirection.y = interactingWith.GetComponent<ItemController>().locations[0].y - transform.position.y;
                        catAnimator.SetFloat("Look X", lookDirection.x);
                        catAnimator.SetFloat("Look Y", lookDirection.y);
                    }
                    break;
                case Status.Lonely:
                    activity = Activity.BeingPetted;
                    catAnimator.SetBool("IsIdle", true);
                    break;
                default:
                    activity = Activity.Waiting;
                    catAnimator.SetBool("IsIdle", true);
                    break;
            }
        } else {
            activity = Activity.WaitingForUses;
            catAnimator.SetBool("IsSitting", true);
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
            catAnimator.SetBool("IsCrouching", false);
            catAnimator.SetBool("IsIdle", false);
            catAnimator.SetBool("IsPlaying", false);
            catAnimator.SetBool("IsSitting", false);
            catAnimator.SetBool("IsSleeping", false);
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
                catAnimator.SetBool("IsSitting", true);
            }
        } else {
            if (interactingWith != null) {
                interactingWith.GetComponent<ItemController>().isOccupied = false;
            }
            if (lastStatus == Status.Tired) {
                transform.position = targetLocation;
            }
            targetLocation = RoomManager.RetrieveEmptySpace(currentRoom);
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
                    if (interactingWith != null && interactingWith.GetComponent<ItemController>().level >= spoiledLevel) {
                        if (interactingWith.GetComponent<ItemController>().filledFood == favFood) {
                            AddPurr(10);
                        } else {
                            AddPurr(5);
                        }
                    } else {
                        AddPurr(2);
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
        catAnimator.SetBool("IsCrouching", false);
        catAnimator.SetBool("IsIdle", false);
        catAnimator.SetBool("IsPlaying", false);
        catAnimator.SetBool("IsSitting", false);
        catAnimator.SetBool("IsSleeping", false);
        isCheckedIn = true;
    }

    public void WalkIntoCarrier(Vector2 carrierEntryLocation) {
        autonomous = false;
        isLeaving = true;
        if (interactingWith != null) {
                interactingWith.GetComponent<ItemController>().isOccupied = false;
            }
        if (lastStatus == Status.Tired) {
            transform.position = targetLocation;
        }
        targetLocation = carrierEntryLocation;
        GetPath();
    }

    public void InitializeCat(
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
        daysLeft = newStayLength;
        carrier = newCarrier;

        switch (coat) {
            case CoatColor.Black:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)132 / 255.0f, (float)120 / 255.0f, (float)117 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)87 / 255.0f, (float)72 / 255.0f, (float)82 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)75 / 255.0f, (float)61 / 255.0f, (float)68 / 255.0f);
                break;
            case CoatColor.Gray:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)171 / 255.0f, (float)155 / 255.0f, (float)142 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)132 / 255.0f, (float)120 / 255.0f, (float)117 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)87 / 255.0f, (float)72 / 255.0f, (float)82 / 255.0f);
                break;
            case CoatColor.Red:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)199 / 255.0f, (float)123 / 255.0f, (float)88 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)174 / 255.0f, (float)93 / 255.0f, (float)64 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)121 / 255.0f, (float)68 / 255.0f, (float)74 / 255.0f);
                break;
            case CoatColor.Tan:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)209 / 255.0f, (float)177 / 255.0f, (float)135 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)186 / 255.0f, (float)145 / 255.0f, (float)88 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)146 / 255.0f, (float)116 / 255.0f, (float)65 / 255.0f);
                break;
            case CoatColor.Gold:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)186 / 255.0f, (float)145 / 255.0f, (float)88 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)146 / 255.0f, (float)116 / 255.0f, (float)65 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)77 / 255.0f, (float)69 / 255.0f, (float)57 / 255.0f);
                break;
            case CoatColor.Blue:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)210 / 255.0f, (float)201 / 255.0f, (float)165 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)140 / 255.0f, (float)171 / 255.0f, (float)161 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)75 / 255.0f, (float)114 / 255.0f, (float)110 / 255.0f);
                break;
            case CoatColor.Orange:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)209 / 255.0f, (float)177 / 255.0f, (float)135 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)199 / 255.0f, (float)123 / 255.0f, (float)88 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)174 / 255.0f, (float)93 / 255.0f, (float)64 / 255.0f);
                break;
            case CoatColor.Teal:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)140 / 255.0f, (float)171 / 255.0f, (float)161 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)75 / 255.0f, (float)114 / 255.0f, (float)110 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)87 / 255.0f, (float)72 / 255.0f, (float)82 / 255.0f);
                break;
            case CoatColor.Cream:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)210 / 255.0f, (float)210 / 255.0f, (float)165 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)209 / 255.0f, (float)177 / 255.0f, (float)135 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)186 / 255.0f, (float)145 / 255.0f, (float)88 / 255.0f);
                break;
            case CoatColor.Burgundy:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)174 / 255.0f, (float)93 / 255.0f, (float)64 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)121 / 255.0f, (float)68 / 255.0f, (float)74 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)75 / 255.0f, (float)61 / 255.0f, (float)68 / 255.0f);
                break;
            case CoatColor.Lime:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)210 / 255.0f, (float)201 / 255.0f, (float)165 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)179 / 255.0f, (float)165 / 255.0f, (float)85 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)119 / 255.0f, (float)116 / 255.0f, (float)59 / 255.0f);
                break;
            case CoatColor.Green:
                transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)179 / 255.0f, (float)165 / 255.0f, (float)85 / 255.0f);
                transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)119 / 255.0f, (float)116 / 255.0f, (float)59 / 255.0f);
                transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)77 / 255.0f, (float)69 / 255.0f, (float)57 / 255.0f);
                break;
            default:
                break;
        }

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
        if (collider.gameObject.tag == "Bea" && activity != Activity.BeingPetted)
        {
            RefreshTooltip();
            transform.Find("Canvas/Cat Tooltip").gameObject.SetActive(true);
            transform.Find("Canvas/Status Thought").gameObject.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea")
        {
            if (activity == Activity.BeingPetted) {
                transform.Find("Canvas/Cat Tooltip").gameObject.SetActive(false);
                transform.Find("Canvas/Status Thought").gameObject.SetActive(true);
            } else {
                RefreshTooltip();
                transform.Find("Canvas/Cat Tooltip").gameObject.SetActive(true);
                transform.Find("Canvas/Status Thought").gameObject.SetActive(false);
            }
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
