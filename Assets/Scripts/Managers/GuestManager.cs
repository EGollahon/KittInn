using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GuestManager : MonoBehaviour
{
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
    
    public GameObject promptManagerReference;
    PromptManager promptManager;
    public GameObject catPrefab;

    void Start()
    {
        promptManager = promptManagerReference.GetComponent<PromptManager>();
        currentOption.GetComponent<Button>().onClick.AddListener(() => SetMode(false));
        requestsOption.GetComponent<Button>().onClick.AddListener(() => SetMode(true));
        RefreshGuests();
        GenerateCat();
    }

    void Update()
    {
        if (
            Input.GetKeyDown("x")
            && PromptManager.currentActionSet == AvailableActionSet.CloseComputerPrompt
            && ComputerManager.currentTab == ComputerTab.Guests
            && takingRequests && currentGuests.Count < 18
        ) {
            AcceptCat();
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

        guestsDetailReference.transform.Find("Food/Food Name").gameObject.GetComponent<TextMeshProUGUI>().text = cat.favFood.ToString();
        int index = foods.FindIndex(element => element.foodName == cat.favFood);
        if (index > -1) {
            guestsDetailReference.transform.Find("Food/Food Sprite").gameObject.GetComponent<Image>().sprite = foods[index].displaySprite;
        }

        if (takingRequests) {
            guestsDetailReference.transform.Find("Stay").gameObject.GetComponent<TextMeshProUGUI>().text = "Stays for " + cat.stayLength.ToString() + " Days";
        } else {
            if (cat.stayLength == 0) {
                guestsDetailReference.transform.Find("Stay").gameObject.GetComponent<TextMeshProUGUI>().text = "Leaves Today";
            } else if (cat.stayLength == 1) {
                guestsDetailReference.transform.Find("Stay").gameObject.GetComponent<TextMeshProUGUI>().text = "Leaves Tomorrow";
            } else {
                guestsDetailReference.transform.Find("Stay").gameObject.GetComponent<TextMeshProUGUI>().text = "Leaves in " + cat.stayLength.ToString() + " Days";
            }
        }

        if (takingRequests && currentGuests.Count < 18) {
            acceptPrompt.SetActive(true);
            guestsDetailReference.transform.Find("Status").gameObject.SetActive(false);
        } else {
            acceptPrompt.SetActive(false);
            // set status sprite and text
            guestsDetailReference.transform.Find("Status").gameObject.SetActive(true);
            // set purr
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

    void GenerateCat() {
        CatName newCatName;
        CoatColor newCoat;
        Personality newPersonality;
        int newSpoiledLevel;
        Food newFavFood;
        float newArrivalTime;
        float newCheckOutTime;
        int newStayLength;

        newCatName = (CatName)Random.Range(0, (int)CatName.Count - 1);
        newCoat = (CoatColor)Random.Range(0, (int)CoatColor.Count - 1);
        newPersonality = (Personality)Random.Range(0, (int)Personality.Count - 1);

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

        // need to randomize
        newArrivalTime = 0.0f;
        newCheckOutTime = 0.0f;

        newStayLength = Random.Range(1, 6);

        GameObject newCat = Instantiate(catPrefab, new Vector2(0.5f, -10.52f), Quaternion.identity);
        newCat.GetComponent<CatController>().IntializeCat(
            newCatName,
            newCoat,
            newPersonality,
            newSpoiledLevel,
            newFavFood,
            newArrivalTime,
            newCheckOutTime,
            newStayLength
        );
        requestedGuests.Add(newCat);
        newCat.SetActive(false);

        RefreshGuests();
        RefreshGuestsDetail();
    }

    void DespawnCat(bool isCurrent, GameObject cat) {
        if (isCurrent) {
            currentGuests.Remove(cat);
        } else {
            requestedGuests.Remove(cat);
        }
        Destroy(cat);
    }

    void AcceptCat() {
        currentGuests.Add(selectedCat.gameObject);
        requestedGuests.Remove(selectedCat.gameObject);
        RefreshGuests();
        selectedCat = null;
        guestsDetailReference.SetActive(false);
    }
}
