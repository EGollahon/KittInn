using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    static float time = 7.0f;
    static TimeOfDay timeOfDay = TimeOfDay.Day;
    static float secondTimer = 0.0f;
    static float lengthOfQuarterHour = 5.0f;

    void Start()
    {
        Debug.Log(time);
        secondTimer = lengthOfQuarterHour;
    }

    void Update()
    {
        if (secondTimer >= 0) {
            secondTimer -= Time.deltaTime;
            if (secondTimer < 0) {
                if (time < 23.75f) {
                    time += 0.25f;
                } else {
                    time = 0.0f;
                }

                if (time > 6.75f && time < 18.0f) {
                    timeOfDay = TimeOfDay.Day;
                } else if (time >= 18.0f && time < 23.25f) {
                    timeOfDay = TimeOfDay.Evening;
                } else {
                    timeOfDay = TimeOfDay.Night;
                }

                Debug.Log(time);
                secondTimer = lengthOfQuarterHour;
            }
        }
    }
}
