using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CompanionController : MonoBehaviour
{
    public static bool isInTutorial = true;
    public int stepInTutorial = 1;

    Animator catAnimator;
    public static float walkSpeed = 2.0f;
    public Activity activity = Activity.Waiting;
    public Vector2 lookDirection = new Vector2(-1.0f, 0.0f);
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

    public GameObject startScreen;
    public GameObject canvas;
    public GameObject fade;
    public GameObject desk;
    public GameObject sellButton;
    public GameObject guestManagerReference;
    GuestManager guestManager;
    public GameObject inventoryManagerReference;
    InventoryManager inventoryManager;
    public GameObject roomManagerReference;
    RoomManager roomManager;
    public GameObject dialogManagerReference;
    DialogManager dialogManager;
    public GameObject handbookManagerReference;
    HandbookManager handbookManager;

    void Awake()
    {
        TimeManager.PauseTime();
        startScreen.SetActive(true);
        startScreen.transform.Find("Start Button").gameObject.GetComponent<Button>().onClick.AddListener(StartGame);

        transform.Find("LightLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)140 / 255.0f, (float)171 / 255.0f, (float)161 / 255.0f);
        transform.Find("MediumLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)87 / 255.0f, (float)72 / 255.0f, (float)82 / 255.0f);
        transform.Find("DarkLayer").gameObject.GetComponent<SpriteRenderer>().color = new Color((float)75 / 255.0f, (float)61 / 255.0f, (float)68 / 255.0f);

        catAnimator = GetComponent<Animator>();
        guestManager = guestManagerReference.GetComponent<GuestManager>();
        inventoryManager = inventoryManagerReference.GetComponent<InventoryManager>();
        roomManager = roomManagerReference.GetComponent<RoomManager>();
        dialogManager = dialogManagerReference.GetComponent<DialogManager>();
        handbookManager = handbookManagerReference.GetComponent<HandbookManager>();
        catAnimator.SetFloat("Look X", 0.0f);
        catAnimator.SetFloat("Look Y", -1.0f);
        catAnimator.SetBool("IsMoving", false);
    }

    void Update()
    {
        if (CompanionController.isInTutorial && TimeManager.time == 7.25f && stepInTutorial == 1) {
            stepInTutorial++;
            TimeManager.EnterEditMode();
            dialogManager.StartDialog(0);
        }

        if (activity == Activity.WaitingForBea) {
            switch (stepInTutorial) {
                case 4:
                    if (inventoryManager.inventory[11].quantity > 0) {
                        activity = Activity.Waiting;
                        stepInTutorial++;
                    }
                    break;
                case 7:
                    if (RoomManager.libraryItems.Count >= 5) {
                        activity = Activity.Waiting;
                        stepInTutorial++;
                    }
                    break;
                case 10:
                    if (guestManager.currentGuests.Count > 0) {
                        activity = Activity.Waiting;
                        TimeManager.ExitEditMode();
                        stepInTutorial++;
                    }
                    break;
                case 13:
                    if (guestManager.currentGuests[0].GetComponent<CatController>().autonomous == true) {
                        activity = Activity.Waiting;
                        stepInTutorial++;
                    }
                    break;
                case 15:
                    if (guestManager.currentGuests[0].GetComponent<CatController>().carrier.GetComponent<CarrierController>().isArriving == false) {
                        activity = Activity.Waiting;
                        stepInTutorial++;
                    }
                    break;
                case 17:
                    if (guestManager.currentGuests.Count == 0) {
                        dialogManager.StartDialog(12);
                        activity = Activity.Waiting;
                        stepInTutorial++;
                    }
                    break;
                default:
                    break;
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
                    if (stepInTutorial == 13 || stepInTutorial == 15 || stepInTutorial == 17) {
                        activity = Activity.WaitingForBea;
                    } else {
                        activity = Activity.Waiting;
                    }
                    catAnimator.SetBool("IsMoving", false);
                }
            }
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
            catAnimator.SetBool("IsMoving", true);
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

    void StartGame() {
        startScreen.transform.Find("Start Button").gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        fade.SetActive(true);
        startScreen.SetActive(false);
        TimeManager.UnpauseTime();
        canvas.GetComponent<Animator>().SetTrigger("FadeOut");
    }

    void NextStepInTutorial() {
        activity = Activity.Talking;
        switch (stepInTutorial) {
            case 2:
                dialogManager.StartDialog(1);
                targetLocation = new Vector2(9.0f, -1.0f);
                GetPath();
                stepInTutorial++;
                break;
            case 3:
                dialogManager.StartDialog(2);
                handbookManager.AddHandbookEntry(0);
                desk.GetComponent<DeskController>().isEnabled = true;
                activity = Activity.WaitingForBea;
                stepInTutorial++;
                break;
            case 5:
                dialogManager.StartDialog(3);
                targetLocation = new Vector2(6.0f, 0.0f);
                GetPath();
                stepInTutorial++;
                break;
            case 6:
                dialogManager.StartDialog(4);
                handbookManager.AddHandbookEntry(1);
                activity = Activity.WaitingForBea;
                stepInTutorial++;
                break;
            case 8:
                dialogManager.StartDialog(5);
                targetLocation = new Vector2(9.0f, -1.0f);
                GetPath();
                stepInTutorial++;
                break;
            case 9:
                dialogManager.StartDialog(6);
                handbookManager.AddHandbookEntry(2);
                guestManager.GenerateCat(true);
                activity = Activity.WaitingForBea;
                stepInTutorial++;
                break;
            case 11:
                dialogManager.StartDialog(7);
                targetLocation = new Vector2(2.0f, -5.0f);
                GetPath();
                stepInTutorial++;
                break;
            case 12:
                dialogManager.StartDialog(8);
                targetLocation = new Vector2(6.0f, 0.0f);
                GetPath();
                stepInTutorial++;
                break;
            case 14:
                dialogManager.StartDialog(9);
                targetLocation = new Vector2(9.0f, -7.0f);
                GetPath();
                stepInTutorial++;
                break;
            case 16:
                dialogManager.StartDialog(10);
                handbookManager.AddHandbookEntry(3);
                handbookManager.AddHandbookEntry(4);
                targetLocation = new Vector2(4.0f, 0.0f);
                GetPath();
                stepInTutorial++;
                break;
            case 18:
                dialogManager.StartDialog(13);
                handbookManager.AddHandbookEntry(5);
                isInTutorial = false;
                for (int i = 0; i < 3; i++) {
                    guestManager.GenerateCat(false);
                }
                break;
            default:
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea" && activity == Activity.Waiting)
        {
            NextStepInTutorial();
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Bea" && activity == Activity.Waiting)
        {
            NextStepInTutorial();
        }
    }

    // void OnTriggerExit2D(Collider2D collider)
    // {
    //     if (collider.gameObject.tag == "Bea")
    //     {
    //         transform.Find("Canvas/Cat Tooltip").gameObject.SetActive(false);
    //         transform.Find("Canvas/Status Thought").gameObject.SetActive(true);
    //     }
    // }
}
