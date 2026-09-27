using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOverController : MonoBehaviour
{
    // ==============================
    // ゲームオーバー時の処理
    // ==============================

    // 時間計測スクリプト
    public BookClosingTimeMeasurement bookClosingTimeMeasurement;

    // ゲームオーバー時の復活用
    // 日数カウント
    private static int previousDayCount;
    // 一回しか処理を行わないように
    public static bool hasResetTriggered = false;


    // フェードアウト
    public Image gameOverPanel;
    public float fadeDuration = 2.0f; // フェードにかかる時間

    private bool isFadingIn = false;

    private float alpha = 0f;


    void Start()
    {
        // ゲームオーバー時は画面暗転
        if (EventTrigger.GameOverFlag)
        {
            if (!gameOverPanel.enabled) gameOverPanel.enabled = true;
            alpha = 1f;
        }
        else // 復活したら画面明転
        {
            if (gameOverPanel.enabled) gameOverPanel.enabled = false;
            alpha = 0f;
        }
    }

    void Update()
    {
        // 日数経過取得用
        if (EventTrigger.GameOverFlag && !hasResetTriggered)
        {
            InitializeDayCount();
            hasResetTriggered = true; // 一度実行したら再実行しない
        }

        // フェード用
        if (EventTrigger.GameOverFlag)
        {
            if (gameOverPanel != null && !gameOverPanel.enabled) gameOverPanel.enabled = true;
            isFadingIn = true;; // ゆっくりフェードイン
            ShowGameOver();
        }
        else // 復活したら画面明転
        {
            if (gameOverPanel.enabled) gameOverPanel.enabled = false;
            alpha = 0f;
        }
    }


    // ゲームオーバー時の復活用
    public void CheckDayChange()
    {
        int currentDayCount = UIWatchController.hour / 24;
        Debug.Log("current : " + currentDayCount + " | prev : " + previousDayCount);
        if (currentDayCount > previousDayCount)
        {
            Debug.Log("日が変わった！復活！現在の経過日数: " + currentDayCount);
            previousDayCount = currentDayCount;
            Events_Dungeon.playerLife = 5; // ライフリセット
            EventTrigger.GameOverFlag = false;
        }
    }
    public void InitializeDayCount()
    {
        previousDayCount = UIWatchController.hour / 24;
        Debug.Log("previousDayCount を " + previousDayCount + " にリセットしました。");
    }

    // フェード用
    public void ShowGameOver()
    {
        if (isFadingIn)
        {
            alpha += 0.002f;
            SetPanelAlpha(alpha);
            if (alpha >= 1f) // フェードイン完了
            {
                alpha = 1f;
                isFadingIn = false;
            }
        }
    }
    private void SetPanelAlpha(float alpha)
    {
        Color color = gameOverPanel.color;
        gameOverPanel.color = new Color(color.r, color.g, color.b, alpha);
    }

}
