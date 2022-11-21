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

    public List<GameObject> libraryItems = new List<GameObject>();
    public List<GameObject> sunroomItems = new List<GameObject>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
