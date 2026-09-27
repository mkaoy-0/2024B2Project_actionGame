using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensorTrigger : MonoBehaviour
{
    // 十字キー
    public bool key_W, key_S, key_A, key_D;
    private bool prevW = false, prevS = false, prevA = false, prevD = false;

    // 決定ボタン
    public bool key_submit;
    private bool prevSubmit = false;

    // 連続で押したときに一気に処理が行われないようにする用の変数
    public float waitTime = 0.1f;
        
    // ページをパタパタ
    public bool page_moving;

    // ページの開き具合
    public static int page_opening = 1;

    // ページ
    public static int currentPage = -1;


    void Start()
    {
        // ページの開き具合初期化
        page_opening = 1;
    }

    void Update()
    {
        PuchButton();

        // タイトルシーンで決定ボタンを押してゲームを始めると
        // シーン切り替え可になる
        if (currentPage != -1 && !EventTrigger.GameClearFlag)
        {
            ChangePage();
        }
    }

    // センサの値受け取り
    void PuchButton()
    {
        // 上下左右（WSAD)
        if (Input.GetKeyDown(KeyCode.W))
        {
            key_W = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            key_S = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            key_A = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            key_D = true;
        }

        // 決定（L)
        if (Input.GetKeyDown(KeyCode.L))
        {
            key_submit = true;
           // Debug.Log("Lキーが押された");
        }

        // ページの開き具合検出（１～４）
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (page_opening == 4)
            {
                page_opening = 1;
            }
            else
            {
                page_opening++;
            }
            Debug.Log("ページの開き具合" + page_opening);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (page_opening == 1)
            {
                page_opening = 1;
            }
            else
            {
                page_opening--;
            }
            Debug.Log("ページの開き具合" + page_opening);
        }

        // ページをパタパタする
        // 押している間はずっとtrue
        if (Input.GetKey(KeyCode.Space))
        {
            page_moving = true;
        }
        else
        {
            page_moving = false;
        }
    }

    // ページ切り替え(１～３) と 本を閉じたとき
    void ChangePage()
    {
        // 本を閉じたとき
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            currentPage = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentPage = 1;
            Debug.Log(currentPage);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentPage = 2;
            Debug.Log(currentPage);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentPage = 3;
            Debug.Log(currentPage);
        }
    }


    // センサを連続で反応させる
    // センサを押したときにtrueになる変数をfalseに戻す
    public IEnumerator ResetW()
    {
        yield return new WaitForSeconds(waitTime); // 100ms待ってから
        key_W = false; // Wをfalseに戻す
        prevW = false;
    }
    public IEnumerator ResetS()
    {
        yield return new WaitForSeconds(waitTime); // 100ms待ってから
        key_S = false;
        prevS = false;
    }
    public IEnumerator ResetA()
    {
        yield return new WaitForSeconds(waitTime); // 100ms待ってから
        key_A = false;
        prevA = false;
    }
    public IEnumerator ResetD()
    {
        yield return new WaitForSeconds(waitTime); // 100ms待ってから
        key_D = false;
        prevD = false;
    }
    public IEnumerator ResetSubmit()
    {
        yield return new WaitForSeconds(waitTime); // 100ms待ってから
        key_submit = false;
        prevSubmit = false;
    }


    // 他スクリプトへの値共有用
    public bool PressW
    {
        get { return key_W; }
        set { key_W = value; }
    }
    public bool PressS
    {
        get { return key_S; }
        set { key_S = value; }
    }
    public bool PressA
    {
        get { return key_A; }
        set { key_A = value; }
    }
    public bool PressD
    {
        get { return key_D; }
        set { key_D = value; }
    }
    public bool PressSubmit
    {
        get { return key_submit; }
        set { key_submit = value; }
    }

    // 
    public bool PrevW
    {
        get { return prevW; }
        set { prevW = value; }
    }
    public bool PrevS
    {
        get { return prevS; }
        set { prevS = value; }
    }
    public bool PrevA
    {
        get { return prevA; }
        set { prevA = value; }
    }
    public bool PrevD
    {
        get { return prevD; }
        set { prevD = value; }
    }
    public bool PrevSubmit
    {
        get { return prevSubmit; }
        set { prevSubmit = value; }
    }

    //
    public bool PageMoving
    {
        get { return page_moving; }
        set { page_moving = value; }
    }
    public static int PageOpening
    {
        get { return page_opening; }
        set { page_opening = value; }
    }
    public static int CurrentPage
    {
        get { return currentPage; }
        set { currentPage = value; }
    }
}

