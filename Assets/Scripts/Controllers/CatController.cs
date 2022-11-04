using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    public CatName catName;
    // public Sprite coat;
    public Personality personality;
    public Food favFood;
    public int spoiledLevel;

    public Status status = Status.Content;
    public float purr = 50.0f;
    public Activity activity;
    // public GameObject interactingWith;

    public float arrivalTime;
    public float checkOutTime;
    public int stayLength;

    public float newStatusTime = 7.5f;

    void Start()
    {
        IntializeCat(
            CatName.Petunia,
            // newCoat,
            Personality.Agreeable,
            Food.SavoryKibble,
            1,
            7.5f,
            10.0f,
            1
        );
    }

    void Update()
    {
        if (status == Status.Content) {
            if (TimeManager.timeOfDay == TimeOfDay.Night && activity != Activity.Sleeping) {
                // status = Status.Tired;
                activity = Activity.Sleeping;
                Debug.Log(status);
            } else if (TimeManager.time == newStatusTime || TimeManager.time == 7.0f) {
                GetNewStatus();
            }
        } else if (status != Status.Upset) {
            if (TimeManager.time == newStatusTime + 0.5f) {
                SatisfyCat();
            }
        }
    }

    void GetNewStatus()
    {
        int newStatus = Random.Range(1, (int)Status.Count - 1);
        status = (Status)newStatus;
        Debug.Log(status);

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
            case Status.Restless:
                activity = Activity.Exploring;
                break;
            case Status.Bored:
                activity = Activity.Playing;
                break;
            case Status.Lonely:
                activity = Activity.BeingPetted;
                break;
            default:
                break;
        }
    }

    public void SatisfyCat() {
        status = Status.Content;
        Debug.Log(status);

        if (
            activity == Activity.Sleeping
            || activity == Activity.Exploring
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

    public void StartPettingCat() {
        activity = Activity.BeingPetted;
        SatisfyCat();
    }

    public void StopPettingCat() {
        GetNewStatus();
    }

    public void PickCatUp() {
        if (personality == Personality.Aloof) {
            status = Status.Upset;
        }
    }

    public void PutCatDown() {
        if (personality == Personality.Aloof) {
            status = Status.Content;
        }
    }

    public void IntializeCat(
        CatName newCatName,
        // Sprite newCoat,
        Personality newPersonality,
        Food newFavFood,
        int newSpoiledLevel,
        float newArrivalTime,
        float newCheckOutTime,
        int newStayLength
    )
    {
        catName = newCatName;
        // Sprite coat = newCoat;
        personality = newPersonality;
        favFood = newFavFood;
        spoiledLevel = newSpoiledLevel;
        arrivalTime = newArrivalTime;
        checkOutTime = newCheckOutTime;
        stayLength = newStayLength;
    }
}
