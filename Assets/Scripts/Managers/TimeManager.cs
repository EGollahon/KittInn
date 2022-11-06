using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static float time = 7.0f;
    public static string timeString = "7:00 AM";
    public static TimeOfDay timeOfDay = TimeOfDay.Day;
    static float secondTimer = 0.0f;
    static float lengthOfQuarterHour = 2.0f;

    public GameObject timeDisplayReference;
    TextMeshProUGUI timeDisplay;

    void Start()
    {
        timeDisplay = timeDisplayReference.GetComponent<TextMeshProUGUI>();
        TranslateTime();
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

                TranslateTime();
                secondTimer = lengthOfQuarterHour;
            }
        }
    }

    void TranslateTime() {
        string hour = "";
        string minutes = "";
        string ampm = "";

        if (Mathf.Floor(time) == 0.0f) {
            hour = "12";
            ampm = "AM";
        } else if (Mathf.Floor(time) == 12.0f) {
            hour = "12";
            ampm = "PM";
        } else if (Mathf.Floor(time) > 12.0f) {
            hour = Mathf.Floor(time - 12).ToString();
            ampm = "PM";
        } else {
            hour = Mathf.Floor(time).ToString();
            ampm = "AM";
        }

        if ((60 * (time - Mathf.Floor(time))) == 0) {
            minutes = "00";
        } else {
            minutes = (60 * (time - Mathf.Floor(time))).ToString();
        }

        timeString = hour + ":" + minutes + " " + ampm;
        timeDisplay.text = timeString;
    }
}
