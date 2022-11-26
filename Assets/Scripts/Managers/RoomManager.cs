using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static float libraryLeftX = 0.5f;
    public static float libraryRightX = 9.5f;
    public static float libraryTopY = 6.8f;
    public static float libraryBottomY = 1.8f;

    public static float sunroomLeftX = -13.94f;
    public static float sunroomRightX = -0.94f;
    public static float sunroomTopY = 8.8f;
    public static float sunroomBottomY = 1.8f;

    public static List<GameObject> libraryItems = new List<GameObject>();
    public static List<GameObject> sunroomItems = new List<GameObject>();

    void Start()
    {
        
    }

    void Update()
    {

    }

    public static void AddItemToRoom(GameObject item, Room room) {
        if (room == Room.Library) {
            libraryItems.Add(item);
        } else if (room == Room.Sunroom) {
            sunroomItems.Add(item);
        }
    }

    public static void RemoveItemFromRoom(GameObject item, Room room) {
        if (room == Room.Library) {
            int index = libraryItems.FindIndex(element => element.GetComponent<ItemController>().locations == item.GetComponent<ItemController>().locations);
            libraryItems.Remove(libraryItems[index]);
            Destroy(item);
        } else if (room == Room.Sunroom) {
            int index = sunroomItems.FindIndex(element => element.GetComponent<ItemController>().locations == item.GetComponent<ItemController>().locations);
            sunroomItems.Remove(sunroomItems[index]);
            Destroy(item);
        }
    }

    public static GameObject RetrieveItemForCat(Room catRoom, InventoryType itemType, Vector2 catLocation) {
        List<GameObject> roomToCheck;
        GameObject itemToReturn = null;
        if (catRoom == Room.Sunroom) {
            roomToCheck = sunroomItems;
        } else {
            roomToCheck = libraryItems;
        }
        for (int i = 0; i < roomToCheck.Count; i++) {
            if (roomToCheck[i].GetComponent<ItemController>().type == itemType && !roomToCheck[i].GetComponent<ItemController>().isOccupied) {
                if (itemToReturn == null) {
                    itemToReturn = roomToCheck[i];
                } else {
                    int distanceToOldTile = (int)(Mathf.Abs(catLocation.x - itemToReturn.transform.position.x) + Mathf.Abs(catLocation.y - itemToReturn.transform.position.y));
                    int distanceToNewTile = (int)(Mathf.Abs(catLocation.x - roomToCheck[i].transform.position.x) + Mathf.Abs(catLocation.y - roomToCheck[i].transform.position.y));
                    if (distanceToNewTile < distanceToOldTile) {
                        itemToReturn = roomToCheck[i];
                    }
                }
            }
        }

        return itemToReturn;
    }

    // public static Vector2 RetrieveRandomOpenSpot(Room catRoom) {
    //     List<GameObject> roomToCheck;
    //     Vector2 locationToReturn;
    //     if (catRoom == Room.Sunroom) {
    //         roomToCheck = sunroomItems;
    //     } else {
    //         roomToCheck = libraryItems;
    //     }
    //     for (int i = 0; i < roomToCheck.Count; i++) {
    //         for (int j = 0; j < roomToCheck[i].GetComponent<ItemController>().locations.Count; j++) {
                
    //         }
    //     }

    //     return itemToReturn;
    // }
}
