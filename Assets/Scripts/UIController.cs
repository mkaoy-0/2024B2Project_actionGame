using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    // 時間UI
    private UIWatchController _UIWatchController;

    // 天気
    public GameObject weather;
    private TextMeshProUGUI text_weather;
    private Image weather_img;
    public Sprite sun;
    public Sprite rain;
    public Sprite night;

    // アイコン
    public GameObject item;
    private GameObject text_item;
    private GameObject icon_weapon;
    private GameObject icon_fire;
    private GameObject icon_key;
    private TextMeshProUGUI keyNum;

    // ライフ
    public GameObject life;
    private Image[] lifeImg;


    void Start()
    {
        _UIWatchController = this.GetComponent<UIWatchController>();

        if (weather != null)
        {
            text_weather = weather.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            weather_img = weather.transform.GetChild(1).gameObject.GetComponent<Image>();
        }
        if (item != null)
        {
            text_item = item.transform.GetChild(0).gameObject;
            icon_weapon = item.transform.GetChild(1).gameObject;
            icon_fire = item.transform.GetChild(2).gameObject;
            icon_key = item.transform.GetChild(3).gameObject;
            keyNum = icon_key.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        }
        if (life != null)
        {
            lifeImg = new Image[life.transform.childCount];
            for(int i = 0; i < life.transform.childCount; i++)
            {
                lifeImg[i] = life.transform.GetChild(i).gameObject.GetComponent<Image>();
            }
        }
    }

    void Update()
    {

        ShowItemIcon();
        ShowWeather();
        ShowLife();

    }

    // HP表示
    public void ShowLife()
    {
        if (Events_Dungeon.playerLife == 5)
        {
            for (int i = 0; i < lifeImg.Length; i++)
            {
                lifeImg[i].enabled = true;
            }
        }
        else if (Events_Dungeon.playerLife >= 0)
        {
            int lostLife = life.transform.childCount - Events_Dungeon.playerLife;

            for (int i = 0; i < lostLife; i++)
            {
                int index = life.transform.childCount - 1 - i;
                if (index >= 0 && lifeImg[index].enabled)
                {
                    lifeImg[index].enabled = false;
                }
            }
        }
    }



    // アイテム入手時にアイテムのアイコン表示
    public void ShowItemIcon()
    {
       // 松明入手
       if (EventTrigger.GetFire)
        {
            if (!text_item.activeSelf) text_item.SetActive(true);
            if (!icon_fire.activeSelf) icon_fire.SetActive(true);
        }
        // 武器入手
        if (EventTrigger.GetWeapon)
        {
            if (!text_item.activeSelf) text_item.SetActive(true);
            if (!icon_weapon.activeSelf) icon_weapon.SetActive(true);
        }
        // 鍵入手
        if (EventTrigger.GetKeyD && EventTrigger.GetKeyF)
        {
            if (!text_item.activeSelf) text_item.SetActive(true);
            if (!icon_key.activeSelf) icon_key.SetActive(true);
            keyNum.text = "2";
        }
        else if (EventTrigger.GetKeyD || EventTrigger.GetKeyF)
        {
            if (!text_item.activeSelf) text_item.SetActive(true);
            if (!icon_key.activeSelf) icon_key.SetActive(true);
            keyNum.text = "1";
        }
    }

    // 天気表示
    public void ShowWeather()
    {
        // 森
        if (SceneManager.GetActiveScene().name == "Forest")
        {
            // 雨
            if (EventTrigger.Raining)
            {
                weather_img.sprite = rain;
                text_weather.text = "雨";
            }
            else
            {
                weather_img.sprite = sun;
                text_weather.text = "晴れ";
            }
        }
        else // 砂漠 or ダンジョン
        {
            text_weather.text = "晴れ";
            // 昼
            if (_UIWatchController.AtNoon)
            {
                weather_img.sprite = sun;
            }
            else // 夜
            {
                weather_img.sprite = night;
            }

        }
    }
}
