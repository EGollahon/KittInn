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

    public static Vector2 RetrieveEmptySpace(Room catRoom) {
        List<GameObject> roomToCheck;
        List<Vector2> occupiedSpaces = new List<Vector2>();
        List<Vector2> unoccupiedSpaces = new List<Vector2>();
        if (catRoom == Room.Sunroom) {
            roomToCheck = sunroomItems;
        } else {
            roomToCheck = libraryItems;
        }

        for (int i = 0; i < roomToCheck.Count; i++) {
            for (int j = 0; j < roomToCheck[i].GetComponent<ItemController>().locations.Count; j++) {
                occupiedSpaces.Add(roomToCheck[i].GetComponent<ItemController>().locations[j]);
            }
        }

        if (catRoom == Room.Sunroom) {
            for (int i = 1; i < 8; i++) {
                for (int j = 0; j < 14; j++) {
                    Vector2 possibleSpace = new Vector2(-((float)j + 0.94f), (float)i + 0.8f);
                    if (!occupiedSpaces.Exists(e => e == possibleSpace)) {
                        unoccupiedSpaces.Add(possibleSpace);
                    }
                }
            }
        } else {
            for (int i = 1; i < 7; i++) {
                for (int j = 0; j < 10; j++) {
                    Vector2 possibleSpace = new Vector2((float)j + 0.5f, (float)i + 0.8f);
                    if (!occupiedSpaces.Exists(e => e == possibleSpace)) {
                        unoccupiedSpaces.Add(possibleSpace);
                    }
                }
            }
        }

        Vector2 locationToReturn = unoccupiedSpaces[Random.Range(0, unoccupiedSpaces.Count)];
        return locationToReturn;
    }
}
