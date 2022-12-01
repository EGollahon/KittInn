using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static float time = 5.0f;
    public static string timeString = "5:00 AM";
    public static TimeOfDay timeOfDay = TimeOfDay.Morning;
    static float secondTimer = 0.0f;
    public static float lengthOfQuarterHour = 2.0f;
    public static bool isEditMode = false;

    public GameObject cameraReference;
    Camera mainCamera;

    public GameObject timeDisplayReference;
    TextMeshProUGUI timeDisplay;

    public GameObject timeOfDayReference;
    TextMeshProUGUI timeOfDayDisplay;

    void Start()
    {
        mainCamera = cameraReference.GetComponent<Camera>();
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
                    mainCamera.backgroundColor = new Color(0.3411765f, 0.282353f, 0.3215686f);
                } else if (time >= 12.0f && time < 18.0f) {
                    timeOfDay = TimeOfDay.Day;
                    mainCamera.backgroundColor = new Color(0.3411765f, 0.282353f, 0.3215686f);
                } else if (time >= 18.0f && time < 21.5f) {
                    timeOfDay = TimeOfDay.Evening;
                    mainCamera.backgroundColor = new Color(0.3411765f, 0.282353f, 0.3215686f);
                } else {
                    timeOfDay = TimeOfDay.Night;
                    mainCamera.backgroundColor = new Color(0.2941177f, 0.2392157f, 0.2666667f);
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
