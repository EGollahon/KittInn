using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GuestManager : MonoBehaviour
{
    float currentTime;
    public bool takingRequests = false;
    public CatController selectedCat;
    public List<GameObject> currentGuests = new List<GameObject>();
    public List<GameObject> requestedGuests = new List<GameObject>();

    public GameObject guestsReference;
    public GameObject guestsDetailReference;
    public List<GameObject> guestSlots = new List<GameObject>();
    public Sprite oneStarSprite;
    public Sprite twoStarSprite;
    public Sprite selectedButtonSprite;
    public Sprite unselectedButtonSprite;
    public List<CoatClass> coats = new List<CoatClass>();
    public List<FoodClass> foods = new List<FoodClass>();

    public GameObject currentOption;
    public GameObject requestsOption;
    public GameObject acceptPrompt;
    
    public GameObject alertFrameReference;
    public GameObject promptManagerReference;
    PromptManager promptManager;
    public GameObject catPrefab;
    public GameObject carrierPrefab;

    void Start()
    {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
        currentOption.GetComponent<Button>().onClick.AddListener(() => SetMode(false));
        requestsOption.GetComponent<Button>().onClick.AddListener(() => SetMode(true));
        RefreshGuests();
    }

    void Update()
    {
        if (
            Input.GetKeyDown("x")
            && PromptManager.currentActionSet == AvailableActionSet.CloseComputerPrompt
            && ComputerManager.currentTab == ComputerTab.Guests
            && takingRequests && currentGuests.Count < 13
            && selectedCat != null
        ) {
            if (CompanionController.isInTutorial) {
                AcceptCat(true);
            } else {
                AcceptCat(false);
            }
        }

        if (TimeManager.time != currentTime) {
            currentTime = TimeManager.time;

            for (int i = 0; i < currentGuests.Count; i++) {
                if (
                    !currentGuests[i].GetComponent<CatController>().isCheckedIn
                    && currentGuests[i].GetComponent<CatController>().arrivalTime == currentTime
                ) {
                    TimeManager.EnterEditMode();
                    currentGuests[i].GetComponent<CatController>().carrier.SetActive(true);
                    alertFrameReference.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text =
                        currentGuests[i].GetComponent<CatController>().catName.ToString() + " has arrived!";
                    alertFrameReference.SetActive(true);
                } else if (
                    currentGuests[i].GetComponent<CatController>().checkOutTime == currentTime
                    && currentGuests[i].GetComponent<CatController>().daysLeft == 0
                ) {
                    TimeManager.EnterEditMode();
                    currentGuests[i].GetComponent<CatController>().carrier.GetComponent<CarrierController>().isLeaving = true;
                    currentGuests[i].GetComponent<CatController>().carrier.GetComponent<CarrierController>().PassGuestManager(gameObject.GetComponent<GuestManager>());
                    alertFrameReference.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text =
                        currentGuests[i].GetComponent<CatController>().catName.ToString() + " is ready to leave!";
                    alertFrameReference.SetActive(true);
                } else if (currentTime == 0.0f) {
                    currentGuests[i].GetComponent<CatController>().daysLeft--;
                    currentGuests[i].GetComponent<CatController>().dailyFeedings = 2;
                }
            }

            if (currentTime == 7.0f && !CompanionController.isInTutorial) {
                Debug.Log(requestedGuests.Count);
                for (int i = 0; i < requestedGuests.Count; i++) {
                    DespawnCat(false, requestedGuests[i]);
                }
                if (KittInnManager.level == 1) {
                    for (int i = 0; i < 5; i++) {
                        GenerateCat(false);
                    }
                } else {
                    for (int i = 0; i < 13; i++) {
                        GenerateCat(false);
                    }
                }
            }
        }
    }

    void RefreshGuests() {
        for (int i = 0; i < guestSlots.Count; i++)
        {
            guestSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
            guestSlots[i].transform.Find("Item").gameObject.GetComponent<Image>().sprite = null;
            guestSlots[i].SetActive(false);
        }

        int usedSlots = 0;
        if (takingRequests) {
            for (int i = 0; i < requestedGuests.Count; i++) {
                int itemIndex = i;
                guestSlots[usedSlots].GetComponent<Button>().onClick.AddListener(() => ShowGuestsDetail(requestedGuests[itemIndex].GetComponent<CatController>()));
                int index = coats.FindIndex(element => element.coatColor == requestedGuests[itemIndex].GetComponent<CatController>().coat);
                if (index > -1) {
                    guestSlots[usedSlots].transform.Find("Item").gameObject.GetComponent<Image>().sprite = coats[index].displaySprite;
                }
                guestSlots[usedSlots].SetActive(true);
                usedSlots += 1;
            }
        } else {
            for (int i = 0; i < currentGuests.Count; i++) {
                int itemIndex = i;
                guestSlots[usedSlots].GetComponent<Button>().onClick.AddListener(() => ShowGuestsDetail(currentGuests[itemIndex].GetComponent<CatController>()));
                int index = coats.FindIndex(element => element.coatColor == currentGuests[itemIndex].GetComponent<CatController>().coat);
                if (index > -1) {
                    guestSlots[usedSlots].transform.Find("Item").gameObject.GetComponent<Image>().sprite = coats[index].displaySprite;
                }
                guestSlots[usedSlots].SetActive(true);
                usedSlots += 1;
            }
        }
    }

    void ShowGuestsDetail(CatController cat) {
        selectedCat = cat;
        guestsDetailReference.SetActive(true);

        guestsDetailReference.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = cat.catName.ToString();
        guestsDetailReference.transform.Find("Personality").gameObject.GetComponent<TextMeshProUGUI>().text = cat.personality.ToString();
        if (cat.spoiledLevel == 1) {
            guestsDetailReference.transform.Find("Level Star").gameObject.GetComponent<Image>().sprite = oneStarSprite;
        } else {
            guestsDetailReference.transform.Find("Level Star").gameObject.GetComponent<Image>().sprite = twoStarSprite;
        }

        string foodString = "";
        switch (cat.favFood) {
            case Food.SavoryKibble:
                foodString = "Savory Kibble";
                break;
            case Food.BeefPate:
                foodString = "Beef Pate";
                break;
            case Food.ChickenKibble:
                foodString = "Chicken Kibble";
                break;
            case Food.SeafoodKibble:
                foodString = "Seafood Kibble";
                break;
            case Food.SalmonPate:
                foodString = "Salmon Pate";
                break;
            case Food.TurkeyInGravy:
                foodString = "Turkey in Gravy";
                break;
            default:
                break;
        }
        guestsDetailReference.transform.Find("Food/Food Name").gameObject.GetComponent<TextMeshProUGUI>().text = foodString;
        int index = foods.FindIndex(element => element.foodName == cat.favFood);
        if (index > -1) {
            guestsDetailReference.transform.Find("Food/Food Sprite").gameObject.GetComponent<Image>().sprite = foods[index].displaySprite;
        }

        if (takingRequests) {
            guestsDetailReference.transform.Find("Stay").gameObject.GetComponent<TextMeshProUGUI>().text = "Stays for " + cat.stayLength.ToString() + " Days";
        } else {
            if (!cat.isCheckedIn && cat.arrivalTime >= TimeManager.time) {
                guestsDetailReference.transform.Find("Stay").gameObject.GetComponent<TextMeshProUGUI>().text = "Arrives Today";
            } else if (!cat.isCheckedIn && cat.arrivalTime < TimeManager.time) {
                guestsDetailReference.transform.Find("Stay").gameObject.GetComponent<TextMeshProUGUI>().text = "Arrives Tomorrow";
            } else if (cat.daysLeft == 0) {
                guestsDetailReference.transform.Find("Stay").gameObject.GetComponent<TextMeshProUGUI>().text = "Leaves Today";
            } else if (cat.daysLeft == 1) {
                guestsDetailReference.transform.Find("Stay").gameObject.GetComponent<TextMeshProUGUI>().text = "Leaves Tomorrow";
            } else {
                guestsDetailReference.transform.Find("Stay").gameObject.GetComponent<TextMeshProUGUI>().text = "Leaves in " + cat.daysLeft.ToString() + " Days";
            }
        }

        if (takingRequests && currentGuests.Count < 13) {
            acceptPrompt.SetActive(true);
            guestsDetailReference.transform.Find("Status Bubble").gameObject.SetActive(false);
            guestsDetailReference.transform.Find("Status").gameObject.SetActive(false);
            guestsDetailReference.transform.Find("Purr Background").gameObject.SetActive(false);
            guestsDetailReference.transform.Find("Purr Mask").gameObject.SetActive(false);
            guestsDetailReference.transform.Find("Purr Percent").gameObject.SetActive(false);
        } else {
            acceptPrompt.SetActive(false);

            guestsDetailReference.transform.Find("Status Bubble/Status Icon").gameObject.GetComponent<Image>().sprite = selectedCat.GetStatusSprite();
            guestsDetailReference.transform.Find("Status").gameObject.GetComponent<TextMeshProUGUI>().text = selectedCat.status.ToString();
            float newMaskHeight = (200.0f * ((float)selectedCat.purr / 100.0f)) + 1.0f;
            guestsDetailReference.transform.Find("Purr Mask").gameObject.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newMaskHeight);
            guestsDetailReference.transform.Find("Purr Percent").gameObject.GetComponent<TextMeshProUGUI>().text = selectedCat.purr.ToString() + "%";

            guestsDetailReference.transform.Find("Status Bubble").gameObject.SetActive(true);
            guestsDetailReference.transform.Find("Status").gameObject.SetActive(true);
            guestsDetailReference.transform.Find("Purr Background").gameObject.SetActive(true);
            guestsDetailReference.transform.Find("Purr Mask").gameObject.SetActive(true);
            guestsDetailReference.transform.Find("Purr Percent").gameObject.SetActive(true);
        }
    }

    public void RefreshGuestsDetail() {
        if (selectedCat != null) {
            ShowGuestsDetail(selectedCat);
        } else {
            selectedCat = null;
            guestsDetailReference.SetActive(false);
        }
    }

    void SetMode(bool setToRequests) {
        takingRequests = setToRequests;
        RefreshGuests();
        RefreshGuestsDetail();

        if (setToRequests) {
            requestsOption.GetComponent<Image>().sprite = selectedButtonSprite;
            currentOption.GetComponent<Image>().sprite = unselectedButtonSprite;
        } else {
            requestsOption.GetComponent<Image>().sprite = unselectedButtonSprite;
            currentOption.GetComponent<Image>().sprite = selectedButtonSprite;
        }
    }

    public void GenerateCat(bool isFirst) {
        CatName newCatName;
        CoatColor newCoat;
        Personality newPersonality;
        int newSpoiledLevel;
        Food newFavFood;
        int newStayLength;

        newCatName = (CatName)Random.Range(0, (int)CatName.Count);
        newCoat = (CoatColor)Random.Range(0, (int)CoatColor.Count);
        newPersonality = (Personality)Random.Range(0, (int)Personality.Count);

        if (KittInnManager.level == 1) {
            newSpoiledLevel = 1;
        } else {
            newSpoiledLevel = Random.Range(1, 3);
        }

        if (newSpoiledLevel == 1) {
            newFavFood = (Food)Random.Range(0, 3);
        } else {
            newFavFood = (Food)Random.Range(3, 6);
        }

        if (isFirst) {
            newStayLength = 1;
        } else {
            newStayLength = Random.Range(1, 6);
        }

        GameObject newCarrier = Instantiate(carrierPrefab, new Vector2(0.5f, -6.264f), Quaternion.identity);
        GameObject newCat = Instantiate(catPrefab, new Vector2(0.5f, -6.264f), Quaternion.identity);
        newCat.GetComponent<CatController>().InitializeCat(
            newCatName,
            newCoat,
            newPersonality,
            newSpoiledLevel,
            newFavFood,
            newStayLength,
            newCarrier
        );
        newCarrier.GetComponent<CarrierController>().GetCat(newCat, alertFrameReference);
        requestedGuests.Add(newCat);
        newCat.SetActive(false);
        newCarrier.SetActive(false);

        RefreshGuests();
        RefreshGuestsDetail();
    }

    public void DespawnCat(bool isCurrent, GameObject cat) {
        if (isCurrent) {
            currentGuests.Remove(cat);
        } else {
            requestedGuests.Remove(cat);
        }
        Destroy(cat.GetComponent<CatController>().carrier);
        Destroy(cat);
        RefreshGuests();
        RefreshGuestsDetail();
    }

    void AcceptCat(bool isFirst) {
        currentGuests.Add(selectedCat.gameObject);
        requestedGuests.Remove(selectedCat.gameObject);

        float newArrivalTime;
        float newCheckOutTime;
        if (isFirst) {
            newArrivalTime = 8.0f;
            newCheckOutTime = 8.0f;
        } else {
            do {
                newArrivalTime = GetRandomTime(true);
            } while (currentGuests.Exists(
                e => (!e.GetComponent<CatController>().isCheckedIn && e.GetComponent<CatController>().arrivalTime == newArrivalTime)
                    || (e.GetComponent<CatController>().isCheckedIn && e.GetComponent<CatController>().checkOutTime == newArrivalTime
                        && (e.GetComponent<CatController>().daysLeft == 0 || e.GetComponent<CatController>().daysLeft == 1)
                    )
            ));

            do {
                newCheckOutTime = GetRandomTime(false);
            } while (currentGuests.Exists(
                e => e.GetComponent<CatController>().isCheckedIn && e.GetComponent<CatController>().checkOutTime == newArrivalTime
                    && (e.GetComponent<CatController>().daysLeft == selectedCat.daysLeft || e.GetComponent<CatController>().daysLeft == selectedCat.daysLeft + 1)
            ));
        }

        selectedCat.arrivalTime = newArrivalTime;
        selectedCat.checkOutTime = newCheckOutTime;

        RefreshGuests();
        selectedCat = null;
        guestsDetailReference.SetActive(false);
    }

    float GetRandomTime(bool isArrival) {
        int earliestTime = 28;
        if (isArrival && TimeManager.time >= 7.0f && TimeManager.time <= 23.0f) {
            earliestTime = (int)(TimeManager.time / 0.25);
        }
        int timeInQuarters = Random.Range(earliestTime, 92);
        float randomTime = timeInQuarters * 0.25f;
        return randomTime;
    }
}
