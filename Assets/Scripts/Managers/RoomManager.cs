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
}
