using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KittInnManager : MonoBehaviour
{
    public static int lifetimePurr = 0;
    public static int averagePurr = 0;
    public static int totalGuests = 0;
    public static int level = 1;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public static void LevelUp() {
        level++;
    }
}
