using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class Events_Forest : MonoBehaviour
{
    // マップや位置取得用スクリプト
    public PlayerMove _playerMove;
    public sensorTrigger sensorTrigger;

    // テキスト
    public GameObject textField;
    public TextMeshProUGUI eventText;
    // 雨のときに表示しておくテキスト
    public GameObject RainningTextField;

    // テキスト表示用変数
    private static int state = 0;
    private static int state_tb = 0;
    private static bool showText = false;
    private static string text;
   // private string weatherText;

    // 火をつけたかどうか
    private static bool lightFire = false;

    // 
    public Material skybox_rain; // 空
    public Material skybox_sun; // 空
    public Light lightColor; // 光の色
    public GameObject fire; // 火
    public GameObject fence; // 柵
    public GameObject tresureBox; // 宝箱
    public Mesh treasureBoxOpen; // 開いた宝箱のメッシュ
    public GameObject particleRain; // 雨

    // 雨が降っているかどうかの変数が変わったかどうか
    private bool previousRaining; // 以前の状態を保存

    // アイコン
    public GameObject icon_fire; // 火
    public GameObject icon_tb; // 宝箱

    // 火を煽いだ時のパーティクルの処理関連
    private ParticleSystem _particleSystem;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;
    private ParticleSystem.ShapeModule shapeModule;
    [Header("パタパタする時間")]
    public float transitionDuration = 3f; // 3秒以上パタパタして完了
    private float elapsedTime = 0f; // 経過時間
    // 火を煽ぐイベント完了フラグ
    private static bool FinishedFireEvent = false;
    private float fenceEventTime = 0f; // FinishedFireEvent が true になった時の時間

    // シーンの Post Process Volume
    public PostProcessVolume postProcessVolume; 
    private DepthOfField depthOfField;


    // 効果音
    public PlaySoundEffects playSoundEffects;

    void Start()
    {
       // sensorTrigger = GameObject.Find("sensorTrigger").GetComponent<sensorTrigger>();

        // テキスト取得
       // eventText = textField.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        eventText.text = text;

        // ===========================================
        // 雨関連
        // post process
        // Depth of Field を取得
        if (postProcessVolume.profile.TryGetSettings(out depthOfField))
        {
            Debug.Log("Depth of Field 設定取得成功");
        }
        // 
        // 初期状態を保存
        // 雨のときの空とかの設定
        previousRaining = EventTrigger.Raining;
        RainWeather();

        // ===================================================
        // 火のパーティクル設定
        if (fire != null)
        {
            _particleSystem = fire.GetComponent<ParticleSystem>();
            if (_particleSystem != null)
            {
                // 各モジュールを取得
                velocityModule = _particleSystem.velocityOverLifetime;
                shapeModule = _particleSystem.shape;
            }
        }

        // ===================================================
        // パタパタ完了後 or 宝箱を開けた後、シーン遷移した場合の処理
        GameObject fireOnFence = fence.transform.GetChild(0).gameObject;
        GameObject centerFence = fence.transform.GetChild(1).gameObject;
        if ((FinishedFireEvent && centerFence.activeSelf) || EventTrigger.GetKeyF)
        {      
            if (fireOnFence.activeSelf) fireOnFence.SetActive(false);
            if (centerFence.activeSelf) centerFence.SetActive(false);
        }

        // ===================================================
        // 鍵入手後
        if (EventTrigger.GetKeyF)
        {
            // アイコン非表示
            if (icon_tb.activeSelf) icon_tb.SetActive(false);
            if (icon_fire.activeSelf) icon_fire.SetActive(false);

            // 宝箱メッシュ変更
            if (treasureBoxOpen != null)
            {
                tresureBox.GetComponent<MeshFilter>().mesh = treasureBoxOpen;
            }
        }

        // ===================================================
        // ページ遷移時のテキスト表示処理
        if (showText)
        {
            textField.SetActive(true);
        }
        else
        {
            textField.SetActive(false);
        }
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
            eventText.text = text;
            if (eventText.text == "一晩休めば回復するだろう。")
            {
                if (textField.activeSelf) textField.SetActive(false);
            }
        }

        // =========================================

        // 火表示設定
        if (lightFire)
        {
            if (!fire.activeSelf) fire.SetActive(true); // 火非表示  
        }
        else
        {
            if (fire.activeSelf) fire.SetActive(false); // 火非表示  
        }

        // =========================================

        // 状態が変わったときのみ関数を呼び出す
        if (previousRaining != EventTrigger.Raining)
        {
            RainWeather();
            previousRaining = EventTrigger.Raining; // 状態を更新
        }

        // ========================================

        // イベント諸々
        EventAtForest();


        if (sensorTrigger.PressSubmit)
        {
            sensorTrigger.PressSubmit = false;
        }
    }

    
    public void EventAtForest()
    {
        if (sensorTrigger.PressSubmit)
            Debug.Log("Lキー：" + sensorTrigger.PressSubmit);

        if (_playerMove.MapGrid == ForestMap.mapGrid)
        {
            // 火の周り(12,1,down)(11,2,right)(13,2,left)(12,3,up)にいるとき
            if ((_playerMove.PlayerPos == new Vector2Int(12, 1) && _playerMove.PlayerDir == Vector2Int.down)
                || (_playerMove.PlayerPos == new Vector2Int(11, 2) && _playerMove.PlayerDir == Vector2Int.right)
                || (_playerMove.PlayerPos == new Vector2Int(13, 2) && _playerMove.PlayerDir == Vector2Int.left)
                || (_playerMove.PlayerPos == new Vector2Int(12, 3) && _playerMove.PlayerDir == Vector2Int.up))
            {
                if (sensorTrigger.PressSubmit)
                {
                    // 晴れているとき
                    if (!EventTrigger.Raining)
                    {
                        // 松明の有無に応じて火をつける
                        Event_LightFire();
                    }
                    else // 雨のとき
                    {
                        Event_CantLightFire();
                    }
                    sensorTrigger.PressSubmit = false;

                }
            }

            // 柵の前(12,1,up)にいるとき
            if (_playerMove.PlayerPos == new Vector2Int(12, 1) && _playerMove.PlayerDir == Vector2Int.up)
            {
                // 柵が表示されていたら
                if (fence.transform.GetChild(1).gameObject.activeSelf)
                {
                    if (sensorTrigger.PressSubmit)
                    {
                        if (!textField.activeSelf)
                        {
                            textField.SetActive(true);
                            showText = true;
                            text = "宝箱がある。柵が邪魔で開けられない。";
                            eventText.text = text;
                        }
                        else
                        {
                            textField.SetActive(false);
                            showText = false;
                        }
                    }
                }
            }           

            // 火がついているとき
            // 火を煽ぐ
            if (lightFire)
            {
                if (_particleSystem == null) return;

                // パタパタする
                Event_FireMoving();
                // 正規化された時間の割合（0 ~ 1）
                float t = elapsedTime / transitionDuration;

                // (12,1,down)のとき、pos.y:-
                if (_playerMove.PlayerPos == new Vector2Int(12, 1) && _playerMove.PlayerDir == Vector2Int.down)
                {
                    // Shape の Position.y を 0 → -1.5 に変更
                    shapeModule.position = new Vector3(0, Mathf.Lerp(0f, -1.5f, t), 0);
                    // Shape の Scale.y を 1 → 3.5 に変更
                    shapeModule.scale = new Vector3(1, Mathf.Lerp(1f, 3.5f, t), 1);
                }

                // (11,2,right)のとき。pos.x:-
                if (_playerMove.PlayerPos == new Vector2Int(11, 2) && _playerMove.PlayerDir == Vector2Int.right)
                {
                    shapeModule.position = new Vector3(Mathf.Lerp(0f, -1.5f, t), 0, 0);
                    shapeModule.scale = new Vector3(Mathf.Lerp(1f, 3.5f, t), 1, 1);
                }

                // (12,3,up)のとき、pos.y:+
                if (_playerMove.PlayerPos == new Vector2Int(12, 3) && _playerMove.PlayerDir == Vector2Int.up)
                {
                    shapeModule.position = new Vector3(0, Mathf.Lerp(0f, 1.5f, t), 0);
                    shapeModule.scale = new Vector3(1, Mathf.Lerp(1f, 3.5f, t), 1);
                }

                // 正解の場所
                // (13,2,left)(13,1,left)のとき、pos.x:+
                if ((_playerMove.PlayerPos == new Vector2Int(13, 2) || _playerMove.PlayerPos == new Vector2Int(13, 1)) && _playerMove.PlayerDir == Vector2Int.left)
                {
                    shapeModule.position = new Vector3(Mathf.Lerp(0f, 1.5f, t), 0, 0);
                    shapeModule.scale = new Vector3(Mathf.Lerp(1f, 3.5f, t), 1, 1);

                    // 一定時間パタパタしたら完了フラグをtrueに
                    if (elapsedTime >= transitionDuration)
                    {
                        FinishedFireEvent = true;
                        Debug.Log("パタパタ完了");
                    }
                }
            }

            // パタパタ完了時
            if (FinishedFireEvent)
            {
                // 柵が表示されているとき
                if (fence.transform.GetChild(1).gameObject.activeSelf)
                {
                    // 柵を燃やす
                    Event_FireOnFence();
                }
                // 柵が消えた後
                if (!fence.transform.GetChild(1).gameObject.activeSelf)
                {
                    // 宝箱の前(11,1,up)(12,1,up)にいるとき
                    if ((_playerMove.PlayerPos == new Vector2Int(12, 1) || _playerMove.PlayerPos == new Vector2Int(11, 1)) && _playerMove.PlayerDir == Vector2Int.up)
                    {
                        if (sensorTrigger.PressSubmit)
                        {
                            Event_OpenTreasureBox();
                            sensorTrigger.PressSubmit = false;
                        }
                    }
                }
            }
            
        }
    }

    // パタパタさせたときの火のパーティクルの挙動のための設定
    public void Event_FireMoving()
    {
        // パタパタ が true の間、時間をカウント
        if (sensorTrigger.PageMoving)
        {
            elapsedTime += Time.deltaTime;
        }
        else
        {
            elapsedTime -= Time.deltaTime;
        }

        // 火の位置が(0,0,0)かつ大きさが(1,1,1)のときのみ、 velocityModuleをOFF
        if (shapeModule.position == new Vector3(0f, 0f, 0f) && shapeModule.scale == new Vector3(1f, 1f, 1f))
        {
            velocityModule.enabled = true;
        }
        else
        {
            velocityModule.enabled = false;
        }

        // 時間制限の範囲を確保（負の値にならないように）
        elapsedTime = Mathf.Clamp(elapsedTime, 0f, transitionDuration);
    }

    // 柵を燃やす
    public void Event_FireOnFence()
    {
        // 柵部分の火を表示
        GameObject fireOnFence = fence.transform.GetChild(0).gameObject;
        if (!fireOnFence.activeSelf) fireOnFence.SetActive(true);

        // 2秒経過で柵非表示
        if (fenceEventTime == 0f)
        {
            fenceEventTime = Time.time;
        }
        Debug.Log("Time : " + Time.time + " | fenceEvent : " + fenceEventTime);
        if (Time.time - fenceEventTime >= 2f)
        {
            fireOnFence.SetActive(false); // 火非表示
            fence.transform.GetChild(1).gameObject.SetActive(false); // 柵非表示
        }
    }

    // 宝箱を開けるイベント
    public void Event_OpenTreasureBox()
    {
        // 石板入手前
        if (!EventTrigger.GetKeyF)
        {
            switch (state_tb)
            {
                case 0:
                    if (!textField.activeSelf) textField.SetActive(true);
                    showText = true;
                    text = "宝箱だ。";
                    eventText.text = text;
                    state_tb = 1;
                    break;

                case 1:
                    text = "石板を入手した。";
                    eventText.text = text;
                    EventTrigger.GetKeyF = true;
                    // アイコン非表示
                    if (icon_tb.activeSelf) icon_tb.SetActive(false);
                    if (icon_fire.activeSelf) icon_fire.SetActive(false);
                    // 宝箱メッシュ変更
                    if (treasureBoxOpen != null)
                    {
                        if (tresureBox.GetComponent<MeshFilter>().mesh != treasureBoxOpen)
                            tresureBox.GetComponent<MeshFilter>().mesh = treasureBoxOpen;
                    }
                    state_tb = 2;
                    playSoundEffects.PlayGetItemSound();
                    break;
            }
        }
        else // 入手済み
        {
            switch (state_tb)
            {
                case 2:
                    if (textField.activeSelf) textField.SetActive(false);
                    showText = false;
                    state_tb = 3;
                    break;

                case 3:
                    if (!textField.activeSelf) textField.SetActive(true);
                    showText = true;
                    text = "もう何もない。";
                    eventText.text = text;
                    state_tb = 2;
                    break;
            }
        }
    }

    // 火をつけるイベント詳細
    public void Event_LightFire()
    {
        // 火をつける前
        if (!lightFire)
        {
            switch (state)
            {
                case 0:
                    if (!textField.activeSelf) textField.SetActive(true);
                    showText = true;
                    text = "焚き火台がある。";
                    eventText.text = text;
                    // 松明入手済
                    if (EventTrigger.GetFire)
                    {
                        state = 1;
                    }
                    else // 未入手
                    {
                        state = 5;
                    }
                    break;

                case 5:
                    if (textField.activeSelf) textField.SetActive(false);
                    showText = false;
                    state = 0;
                    break;

                case 1:
                    text = "焚き火台に火をつけた。";
                    eventText.text = text;
                    lightFire = true;
                    state = 2;
                    playSoundEffects.PlayGetItemSound();
                    break;
            }
        }
        else // つけた後
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
                    text = "火がついている。";
                    eventText.text = text;
                    state = 2; 
                    break;
            }
        }
    }


    // 雨のときの火をつけるイベント詳細
    public void Event_CantLightFire()
    {
        if (!textField.activeSelf)
        {
            textField.SetActive(true);
            showText = true;
            text = "焚き火台がある。";
            eventText.text = text;
        }
        else
        {
            textField.SetActive(false);
            showText = false;
        }
    }

    // 天気設定
    public void RainWeather()
    {
        // 雨のとき
        if (EventTrigger.Raining)
        {
            if (!RainningTextField.activeSelf) RainningTextField.SetActive(true); // テキスト表示

            RenderSettings.skybox = skybox_rain; // 雨空
            RenderSettings.reflectionIntensity = 0.4f;
            lightColor.color = new Color32(65, 71, 104, 255); // 光の色変更
            DynamicGI.UpdateEnvironment();
            if (!particleRain.activeSelf) particleRain.SetActive(true); // 雨表示
            // 画面ぼかしエフェクト
            if (depthOfField != null)
            {
                depthOfField.active = true;
            }
            lightFire = false;
            state = 0;
        }
        else // 雨が止んでいるとき
        {
            if (RainningTextField.activeSelf) RainningTextField.SetActive(false); // テキスト非表示

            RenderSettings.skybox = skybox_sun; // 空
            RenderSettings.reflectionIntensity = 1f;
            lightColor.color = new Color32(255, 244, 214, 255); // 光の色変更
            DynamicGI.UpdateEnvironment();
            if (particleRain.activeSelf) particleRain.SetActive(false); // 雨非表示
            // 画面ぼかしエフェクト
            if (depthOfField != null)
            {
                depthOfField.active = false;
            }
        }
    }

}
