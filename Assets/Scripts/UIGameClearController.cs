using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGameClearController : MonoBehaviour
{
    // =============================
    // ゲームクリアの演出
    // 画面明転, クリアテキスト表示
    // =============================

    public sensorTrigger sensorTrigger;

    // 表示させるパネル
    public Image gameClearPanel;
    private TextMeshProUGUI clearText;
   
    // フェードアウト
    public float fadeDuration = 2.0f; // フェードにかかる時間
    private bool isFadingIn = false;
    // パネルの透明度
    private float alpha = 0f;


    void Start()
    {
        if (gameClearPanel != null)
        {
            clearText = gameClearPanel.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        // ゲームクリア時は画面明転
        if (EventTrigger.GameClearFlag)
        {
            if (!gameClearPanel.enabled) gameClearPanel.enabled = true;
            alpha = 1f;
            if (!clearText.enabled) clearText.enabled = true;
        }
        else // クリアしていないとき
        {
            if (gameClearPanel.enabled) gameClearPanel.enabled = false;
            alpha = 0f;
            if (clearText.enabled) clearText.enabled = false;
        }
    }

    void Update()
    {
        // フェード用
        // クリア時数秒まって画面明転
        if (EventTrigger.GameClearFlag)
        {
            StartCoroutine(DelayedGameClear(1f)); // 数秒後に処理開始

        }
        else // クリアしてないとき
        {
            // パネル、テキスト非表示
            if (gameClearPanel.enabled) gameClearPanel.enabled = false;
            alpha = 0f;
            if (clearText.enabled) clearText.enabled = false;
        }
    }

    private IEnumerator DelayedGameClear(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gameClearPanel != null && !gameClearPanel.enabled)
        {
            gameClearPanel.enabled = true;
        }

        isFadingIn = true; // ゆっくりフェードイン
        ShowGameClear();

        // 透明度が0.8以上になったら、クリアテキスト表示
        while (alpha < 1f)
        {
            yield return null; // フレームごとに待機
        }

        // 決定ボタンでクリア表示
        if (sensorTrigger.PressSubmit)
        {
            if (!clearText.enabled)
            {
                clearText.enabled = true;
            }
            else // 文字表示後はタイトルに戻る
            {
                sensorTrigger.currentPage = -1;
            }
        }
        sensorTrigger.PressSubmit = false;
    }


    public void ShowGameClear()
    {
        if (isFadingIn)
        {
            alpha += 0.01f;
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
        Color color = gameClearPanel.color;
        gameClearPanel.color = new Color(color.r, color.g, color.b, alpha);
    }
}
