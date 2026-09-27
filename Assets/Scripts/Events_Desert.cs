using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Events_Desert : MonoBehaviour
{
    // マップや位置取得用スクリプト
    public PlayerMove _playerMove;
    public sensorTrigger sensorTrigger;

    // テキスト
    public GameObject textField;
    public TextMeshProUGUI eventText;

    // テキスト表示用変数
    private static int state = 0;
    private static int stateGetKey = 0;
    private static int stateGetHint = 0;
    private static bool showText = false;
    private static string text;

    // 
    public GameObject weapon; // 武器
    public GameObject particle; // 砂嵐

    // アイコン
    public GameObject icon_weapon;
    public GameObject icon_particle;


    public GameObject key; // 石板
    private Renderer targetRenderer; //石板表示切り替え用変数

    // ページの影
    public UIWatchController _UIWatchController; // 昼夜の時間取得
    public GameObject shadow;
    private int previousPageOpening = 1; // 前回の PageOpening の値
    private Vector3 targetPosition;  // 目標位置
    private Vector3 originalShadowPos; // 最初の影の位置
    [Header("影の移動速度")]
    public float moveSpeed = 5.0f;   // 移動速度
    [Header("影と鍵が重なる範囲")]
    public float minPosZ = 17.5f;
    public float maxPosZ = 31.4f;
    [Header("影の座標")]
    public float[] targetPosZ = new float[4] { 0f, 18f, 30f, 51f };

    // 空
    private bool previousAtNoon; // 以前の状態を保存
    public Material skybox_noon; // 昼空
    public Material skybox_night; // 夜空
    public Light lightColor; // 光の色

    // 背景画像
    public GameObject bg_img;

    // 効果音
    public PlaySoundEffects playSoundEffects;


    void Start()
    {
       // sensorTrigger = GameObject.Find("sensorTrigger").GetComponent<sensorTrigger>();

        // ======================================-
        // テキスト
      //  eventText = textField.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        eventText.text = text;

        // ページ遷移時のテキスト表示処理
        if (showText)
        {
            textField.SetActive(true);
        }
        else
        {
            textField.SetActive(false);
        }

        // ======================================

        // 武器入手後は非表示
        if (EventTrigger.GetWeapon)
        {
            if (weapon.activeSelf) weapon.SetActive(false);
            // アイコン非表示
            if (icon_weapon.activeSelf) icon_weapon.SetActive(false);
        }

        // 敵倒した後
        if (EventTrigger.Monster)
        {
            // アイコン非表示
            if (icon_particle.activeSelf) icon_particle.SetActive(false);
        }

        // =====================================--
        // 影の最初の位置を設定
        originalShadowPos = shadow.transform.localPosition;
        targetPosition = new Vector3(originalShadowPos.x, originalShadowPos.y, targetPosZ[0]);
        // 影と石板の重なり検出用
        targetRenderer = key.GetComponent<Transform>().GetComponent<Renderer>();

        // ======================================
        // 昼夜設定
        // 初期状態を保存
        previousAtNoon = _UIWatchController.AtNoon;
        ChangeSkybox(); // 初期状態で一度実行

    }

    void Update()
    {
        // ゲームオーバー ========================
        // 他のページに移動で復活
        if (EventTrigger.GameOverFlag)
        {
            if (!textField.activeSelf) textField.SetActive(true);
            eventText.text = "一晩休めば回復するだろう。";
        }
        else // 復活時テキスト非表示
        {
            if (eventText.text == "一晩休めば回復するだろう。")
            {
                eventText.text = text;
                if (textField.activeSelf) textField.SetActive(false);
            }
        }
        // =========================================

        // 昼夜設定
        // 状態が変わったときのみ関数を呼び出す
        if (previousAtNoon != _UIWatchController.AtNoon)
        {
            ChangeSkybox();
            previousAtNoon = _UIWatchController.AtNoon; // 状態を更新
        }

        // イベント諸々
        EventAtDesert();

        // センサリセット
        if (sensorTrigger.PressSubmit)
        {
             sensorTrigger.PressSubmit = false;
        }
    }

    public void EventAtDesert()
    {
        
        if (_playerMove.MapGrid == DesertMap.mapGrid)
        {
            // 武器入手(12,12)
            if (_playerMove.PlayerPos == new Vector2Int(12, 12) && _playerMove.PlayerDir == Vector2Int.right )
            {
                if (!sensorTrigger.PrevSubmit && sensorTrigger.PressSubmit)
                {
                    Event_GetWeapon();
                  //  sensorTrigger.PressSubmit = false;
                }
            }
            // 砂嵐(7,10)で進めない場所
            if (_playerMove.PlayerPos == new Vector2Int(7, 10) && _playerMove.PlayerDir == Vector2Int.up)
            {
                // 敵を倒していない場合
                if (!EventTrigger.Monster)
                {
                    if (sensorTrigger.PressW)
                    {
                        if (!textField.activeSelf) textField.SetActive(true);
                        showText = true;
                        text = "風の暴れ方が尋常じゃない。おそらく、どこかに砂嵐を操るものがいるに違いない。\nこの砂嵐が止むまではこの先に進めそうにない。";
                        eventText.text = text;
                        sensorTrigger.PressW = false;
                    }
                    if (sensorTrigger.PressSubmit)
                    {
                        if (textField.activeSelf) textField.SetActive(false);
                        showText = false;
                      //  sensorTrigger.PressSubmit = false;
                    }
                }
            }


            // (2, 12, left)影のヒント入手
            if (_playerMove.PlayerPos == new Vector2Int(2, 12) && _playerMove.PlayerDir == Vector2Int.left)
            {
                if (sensorTrigger.PressSubmit)
                {
                    Event_GetHint();
                    //  sensorTrigger.PressSubmit = false;
                }
            }

            // (7,2,up)(6,1,right)(8,1,left)石板入手
            if ((_playerMove.PlayerPos == new Vector2Int(7, 2) && _playerMove.PlayerDir == Vector2Int.up)
                || (_playerMove.PlayerPos == new Vector2Int(6, 1) && _playerMove.PlayerDir == Vector2Int.right)
                || (_playerMove.PlayerPos == new Vector2Int(8, 1) && _playerMove.PlayerDir == Vector2Int.left))
            {
                if ( sensorTrigger.PressSubmit)
                {
                    Event_GetKeyD();
                  //  sensorTrigger.PressSubmit = false;
                }
            }


            // 敵を倒した後の処理
            if (EventTrigger.Monster)
            {
                // 砂嵐非表示
                if (particle.activeSelf) particle.SetActive(false);
                if (DesertMap.GetDesertMap(9, 7) == 1)
                {
                    if (!textField.activeSelf) textField.SetActive(true);
                    showText = true;
                    text = "砂嵐が止んだようだ。";
                    eventText.text = text;
                    // 武器をとるマスにいるときにそっちのテキスト表示と競合しないように
                    if (_playerMove.PlayerPos == new Vector2Int(12, 12) && _playerMove.PlayerDir == Vector2Int.right)
                    {
                        state = 2;
                    }
                }
                if (text == "砂嵐が止んだようだ。")
                {
                    if (sensorTrigger.PressSubmit)
                    {
                        if (textField.activeSelf) textField.SetActive(false);
                        showText = false;
                        
                      //  sensorTrigger.PressSubmit = false;
                    }
                }
                // マップ変更
                DesertMap.SetDesertMap(9, 7, 0);
            }

            // 影
            // 昼のとき
            if (_UIWatchController.AtNoon)
            {
                // 影表示
                if (!shadow.activeSelf) shadow.SetActive(true);

                // PageOpening が変わったときだけ Event_Shadow() を呼ぶ
                if (sensorTrigger.PageOpening != previousPageOpening)
                {
                    ShadowSetting();
                    previousPageOpening = sensorTrigger.PageOpening; // 値を更新
                }
                // 目標地点に向かって滑らかに移動
                shadow.transform.localPosition = Vector3.Lerp(shadow.transform.localPosition, targetPosition, Time.deltaTime * moveSpeed);

                // 影と石板が重なり検出
                // 鍵の場所表示
                Event_ShadowOnKey();
            }
            else // 夜のとき
            {
                // 影非表示
                if (shadow.activeSelf) shadow.SetActive(false);
                targetRenderer.enabled = false; // 3Dオブジェクトを非表示
            }

        }
    }


    public void ShadowSetting()
    {
        targetPosition = new Vector3(originalShadowPos.x, originalShadowPos.y, targetPosZ[sensorTrigger.PageOpening - 1]);
    }

    public void Event_ShadowOnKey()
    {
        if (IsOverlapping())
        {
            targetRenderer.enabled = true;  // 3Dオブジェクトを表示
            // Debug.Log("石板表示");
        }
        else
        {
            targetRenderer.enabled = false; // 3Dオブジェクトを非表示
        }
    }

    bool IsOverlapping()
    {
        bool isOverlapping = false ;

        Vector3 shadowPos = shadow.transform.localPosition;

        if (minPosZ <= shadowPos.z && shadowPos.z <= maxPosZ)
        {
            isOverlapping = true; ;
        }

        return isOverlapping;
    }


    public void Event_GetKeyD()
    {
        // 石板未入手
        if (!EventTrigger.GetKeyD)
        {
            switch (stateGetKey)
            {
                case 0:
                    if (!textField.activeSelf) textField.SetActive(true);
                    showText = true;
                    text = "何か埋まっている。";
                    eventText.text = text;
                    stateGetKey = 1;
                    break;

                case 1:
                    text = "石板を手に入れた。";
                    eventText.text = text;
                    EventTrigger.GetKeyD = true;
                    if (weapon.activeSelf) weapon.SetActive(false);
                    stateGetKey = 2;
                    playSoundEffects.PlayGetItemSound(); // 効果音
                    break;
            }
        }
        else // 入手済
        {
            switch (stateGetKey)
            {
                case 2:
                    if (textField.activeSelf) textField.SetActive(false);
                    showText = false;
                    stateGetKey = 3;
                    break;

                case 3:
                    if (!textField.activeSelf) textField.SetActive(true);
                    showText = true;
                    text = "もう何もない。";
                    eventText.text = text;
                    stateGetKey = 2;
                    break;
            }
        }
    }

    public void Event_GetWeapon()
    {
        // 武器未入手
        if ( !EventTrigger.GetWeapon )
        {
            switch (state)
            {
                case 0:
                    if (!textField.activeSelf) textField.SetActive(true);
                    showText = true;
                    text = "剣が落ちている。";
                    eventText.text = text;
                    state = 1;
                    break;

                case 1:
                    text = "剣を手に入れた。";
                    eventText.text = text;
                    EventTrigger.GetWeapon = true;
                    if (weapon.activeSelf) weapon.SetActive(false);
                    // アイコン非表示
                    if (icon_weapon.activeSelf) icon_weapon.SetActive(false);
                    state = 2;
                    playSoundEffects.PlayGetItemSound(); // 効果音
                    break;
            }
        }
        else // 武器入手済
        {
            switch (state)
            {
                case 2:
                    if (textField.activeSelf) textField.SetActive(false);
                    showText = false;
                    state = 3;
                    break;

                case 3:
                    if (!textField.activeSelf) textField.SetActive(true);
                    showText = true;
                    text = "もう何もない。";
                    eventText.text = text;
                    state = 2;
                    break;
            }
        }
    }


    // 影のヒント入手
    public void Event_GetHint()
    {
        switch (stateGetHint)
        {
            case 0:
                if (!textField.activeSelf) textField.SetActive(true);
                showText = true;
                text = "何かのメモが落ちている。";
                eventText.text = text;
                stateGetHint = 1;
                break;

            case 1:
                text = "『影の中には、目に見えないものが潜んでいることがある。』";
                eventText.text = text;
                stateGetHint = 2;
                break;

            case 2:
                text = "『昼が終わると、影は消え、見つけるのは難しくなる。』";
                eventText.text = text;
                stateGetHint = 3;
                break;

            case 3:
                if (textField.activeSelf) textField.SetActive(false);
                showText = false;
                stateGetHint = 0;
                break;
        }
    }


    // 昼夜変更
    public void ChangeSkybox()
    {
        
        // 本を閉じていた時間に応じて昼夜変更
        if (_UIWatchController.AtNoon)
        {
            // 昼のとき
            RenderSettings.skybox = skybox_noon;
            lightColor.color = new Color32(238, 209, 150, 255); // 光の色変更
            DynamicGI.UpdateEnvironment();

            // 背景画像の色を変更
            foreach (Transform child in bg_img.transform)
            {
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color32(217, 217, 217, 255);
                }
            }
        }
        else // 夜
        {
            RenderSettings.skybox = skybox_night;
            lightColor.color = new Color32(50, 60, 130, 255); // 光の色変更
            DynamicGI.UpdateEnvironment();

            // 背景画像の色を変更
            foreach (Transform child in bg_img.transform)
            {
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color32(86, 87, 99, 255);
                }
            }
        }
    }
}
