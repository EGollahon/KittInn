using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    public static float walkSpeed = 2.0f;
    public CatName catName;
    public CoatColor coat;
    public Personality personality;
    public int spoiledLevel;
    public Food favFood;
 
    public bool autonomous = false;
    public Status status = Status.Content;
    public Status lastStatus = Status.Content;
    public float purr = 50.0f;
    public Activity activity;
    // public GameObject interactingWith;

    public GameObject carrier;
    public bool isCheckedIn = false;
    public float arrivalTime;
    public float checkOutTime;
    public int stayLength;

    public float newStatusTime = 0.0f;
    public float satisfyTimer = -1.0f;

    public Room currentRoom;
    List<Tile> openList = new List<Tile>();
    List<Tile> closedList = new List<Tile>();
    Vector2 targetLocation = new Vector2(0.0f, 0.0f);
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
            if (TimeManager.timeOfDay == TimeOfDay.Night && activity != Activity.Sleeping) {
                status = Status.Tired;
                activity = Activity.Sleeping;
            } else if (TimeManager.time == newStatusTime) {
                GetNewStatus();
                GetPath();
            }
        }

        if (activity == Activity.Moving) {
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
                    autonomous = true;
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
                            break;
                        case Status.Tired:
                            activity = Activity.Sleeping;
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
                }
            }
        }

        if (satisfyTimer >= 0) {
            satisfyTimer -= Time.deltaTime;
            if (satisfyTimer < 0) {
                SatisfyCat();
                targetLocation = new Vector2(0.5f, 0.5f);
            }
        }
    }

    void GetPath()
    {
        if (targetLocation.x != transform.position.x || targetLocation.y != transform.position.y) {
            openList.Clear();
            closedList.Clear();
            stepInPath = 1;

            Tile currentTile = CreateNewTile(new Vector2(transform.position.x, transform.position.y), null, true);
            closedList.Add(currentTile);

            AddNewTiles();

            activity = Activity.Moving;
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
            // if a tile location is equal to one of the room manager provided items, avoid

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

    void GetNewStatus()
    {
        bool isStatusValid = false;

        while (!isStatusValid) {
            int newStatus = Random.Range(1, (int)Status.Count - 1);
            if (newStatus != (int)lastStatus) {
                status = (Status)newStatus;
                isStatusValid = true;
            }
        }
    }

    public void SatisfyCat()
    {
        status = Status.Content;

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
            GetNewStatus();
        }
    }

    public void StartPettingCat()
    {
        activity = Activity.BeingPetted;
        SatisfyCat();
    }

    public void StopPettingCat()
    {
        GetNewStatus();
    }

    public void WalkOutOfCarrier(Vector2 carrierLookDirection) {
        targetLocation = new Vector2(transform.position.x + carrierLookDirection.x, transform.position.y + carrierLookDirection.y);
        Tile currentTile = CreateNewTile(new Vector2(transform.position.x, transform.position.y), null, true);
        closedList.Add(currentTile);
        closedList.Add(CreateNewTile(targetLocation, currentTile, false));
        activity = Activity.Moving;
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
    }
}
