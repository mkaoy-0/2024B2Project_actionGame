using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIWatchController : MonoBehaviour
{
    // ================================
    // –{‚ğ•Â‚¶‚Ä‚¢‚éŠÔ‚¾‚¯AUI‚ÌŒv‚ÆŠÔ‚ği‚ß‚é
    // Šm—¦‚Å‰J‚ğ~‚Ü‚¹‚é
    // •Â‚¶‚Ä‚¢‚éŠÔ‚Ì‡Œvihour‚Ì’lj‚ª‘å‚«‚­‚È‚é‚Ù‚ÇŠm—¦‚ªã‚ª‚é
    // ================================

    // ŠÔŒv‘ªƒXƒNƒŠƒvƒg
    public BookClosingTimeMeasurement bookClosingTimeMeasurement;

    public static int hour = 23;
    public static int minute = 20;

    private int showHour;
    private int showMin;

    private bool atNoon;

    // ŠÔUI
    public GameObject time;
    private TextMeshProUGUI text_time;
    private GameObject watch_long;
    private GameObject watch_short;

    // ‰J‚ª~‚ŞŠm—¦
    [Header("‚±‚Ì•b”‚ğ’´‚¦‚é‚Æ100%‰J‚ª~‚Ş")]
    public float threshold = 10f;

    


    void Start()
    {
        if (time != null)
        {
            text_time = time.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            watch_long = time.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).gameObject;
            watch_short = time.transform.GetChild(1).transform.GetChild(0).transform.GetChild(1).gameObject;
        }

        showHour = hour % 24;
        showMin = minute % 60;

        ShowTime();
    }

    void Update()
    {
        // ’‹‚ÌŠÔ
        if (6 <= showHour && showHour <= 17)
        {
            atNoon = true;
        }
        else // –é
        {
            atNoon = false;
        }
    }

    public void ChangeTime()
    {
        hour += (int)(bookClosingTimeMeasurement.BookClosingTime * 2);
        showHour = hour % 24;

        minute += (int)(bookClosingTimeMeasurement.BookClosingTime * 60);
        showMin = minute % 60;

        ShowTime();
    }

    // ŠÔ
    public void ShowTime()
    {
        // ƒeƒLƒXƒg
        text_time.text = $"{showHour:D2}:{showMin:D2}";

        // Œv
        // Œ»İ‚Ì‰ñ“]Šp“x‚ğæ“¾
        Vector3 currentRotation_l = watch_long.transform.eulerAngles;
        // Z²‚¾‚¯‚ğ•ÏX
        currentRotation_l.z = -6 * showMin;
        // •ÏXŒã‚Ì‰ñ“]‚ğ“K—p
        watch_long.transform.eulerAngles = currentRotation_l;

        // ’Zj
        Vector3 currentRotation_s = watch_short.transform.eulerAngles;
        currentRotation_s.z = -30 * showHour + 180;
        watch_short.transform.eulerAngles = currentRotation_s;
    }


    public void StopRaining()
    {
        // •Ï”‚ªthreshold‚ğ’´‚¦‚½‚çŠm—¦100“‚É‚È‚é
        float probability = Mathf.Clamp01(bookClosingTimeMeasurement.BookClosingTime / threshold); // 0`1‚Ì”ÍˆÍ‚Éû‚ß‚é
        Debug.Log("‰J‚ª~‚ŞŠm—¦ : " + probability);

        // ‰J‚ª~‚Á‚Ä‚¢‚½‚ç
        if (EventTrigger.Raining)
        {
            if (Random.Range(0f, 1f) < probability)
            {
                // Šm—¦‚Å‰J‚ğ~‚Ü‚¹‚é
                EventTrigger.Raining = false;
                Debug.Log("‰J‚ª~‚ñ‚¾");

            }
        }
        else // ‰J‚ª~‚ñ‚Å‚¢‚½‚ç
        {
            if (Random.Range(0f, 1f) < 0.2f)
            {
                // ‚Q‚O“‚ÅÄ‚Ñ‰J‚ğ~‚ç‚¹‚é
                EventTrigger.Raining = true;
                Debug.Log("‰J‚ª~‚èn‚ß‚½");

            }
        }
    }


    // ’‹–é”»’è
    public bool AtNoon
    {
        get { return atNoon; }
        set { atNoon = value; }
    }

    // ŠÔ
    public int ShowHour
    {
        get { return showHour; }
        set { showHour = value; }
    }
    public int ShowMin
    {
        get { return showMin; }
        set { showMin = value; }
    }
}
