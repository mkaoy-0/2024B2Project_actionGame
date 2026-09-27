using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookClosingTimeMeasurement : MonoBehaviour
{
    // =======================
    // 本を閉じている時間を計測
    // =======================


    // 時間計測のための変数
    private float timer = 0f;
    public static float bookClosingTime = 0f;

    // 計測を開始するフラグ
    private static bool isTiming = false;

    //
    public UIWatchController _UIWatchController;
    public UIGameOverController _UIGameOverController;

    void Start()
    {
        timer = 0f;
    }

    void Update()
    {
        if (sensorTrigger.currentPage == 0)
        {
            if (!isTiming)
            {
                // 計測開始
                isTiming = true;
                 timer = 0f; // 0秒から計測し直す
            }

            // 時間を計測
            timer += Time.deltaTime;
        }
        else
        {
            if (isTiming)
            {
                // 計測終了（ログ出力）
                bookClosingTime = timer; 
                Debug.Log("Time measured: " + bookClosingTime + " seconds");
                // 時間変更
                _UIWatchController.ChangeTime();
                // 雨を止ませる
                _UIWatchController.StopRaining();
                if (EventTrigger.GameOverFlag)
                {
                    // 経過日数を確認
                    _UIGameOverController.CheckDayChange();
                }
                isTiming = false; // 計測停止
            }
        }
    }

    public float BookClosingTime
    {
        get { return bookClosingTime; }
        set { bookClosingTime = value; }
    }
    public static bool IsTiming
    {
        get { return isTiming; }
        set { isTiming = value; }
    }
}
