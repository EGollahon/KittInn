using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static float time = 7.0f;
    public static string timeString = "7:00 AM";
    public static TimeOfDay timeOfDay = TimeOfDay.Morning;
    static float secondTimer = 0.0f;
    static float lengthOfQuarterHour = 2.0f;
    static bool isEditMode = false;

    public GameObject timeDisplayReference;
    TextMeshProUGUI timeDisplay;

    public GameObject timeOfDayReference;
    TextMeshProUGUI timeOfDayDisplay;

    void Start()
    {
        timeDisplay = timeDisplayReference.GetComponent<TextMeshProUGUI>();
        timeOfDayDisplay = timeOfDayReference.GetComponent<TextMeshProUGUI>();
        TranslateTime();
        secondTimer = lengthOfQuarterHour;
    }

    void Update()
    {
        if (secondTimer >= 0 && !isEditMode) {
            secondTimer -= Time.deltaTime;
            if (secondTimer < 0) {
                if (time < 23.75f) {
                    time += 0.25f;
                } else {
                    time = 0.0f;
                }

                if (time > 6.75f && time < 12.0f) {
                    timeOfDay = TimeOfDay.Morning;
                } else if (time >= 12.0f && time < 18.0f) {
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
        timeOfDayDisplay.text = timeOfDay.ToString();
    }

    public static void PauseTime() {
        Time.timeScale = 0.0f;
    }

    public static void UnpauseTime() {
        Time.timeScale = 1.0f;
    }

    public static void EnterEditMode() {
        isEditMode = true;
    }

    public static void ExitEditMode() {
        isEditMode = false;
    }
}
