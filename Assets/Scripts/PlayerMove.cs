using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    /// <summary>
    /// terrainを使用
    /// terrainは回転や中心座標が変えられない（できるかもしれないが変え方がわからない）
    /// Playerの座標(0, 4, 0)と、mapGrid[0,0]の位置が合うようにfieldの座標を自分で調整する
    /// Player座標のy軸の4は実際に試していいと思った値
    /// </summary>

    sensorTrigger sensorTrigger;

    public float gridSize = 3.262488f; // 一マスの実際の大きさ
    public float moveSpeed = 5f; // 移動速度

    public int[,] mapGrid;      // マップデータ
    private Vector2Int playerPosition; // プレイヤーの現在位置（論理マップの座標）
    [Header("プレイヤーの初期位置。マップ配列[15*15]を参照")]
    public int startX, startY; // 開始位置

    [Header("初期向き。0:左, 1:上, 2:右, 3:下")]
    // プレイヤーの初期向きをインスペクタから指定できるようにする
    public int directionIndex = 0; // 0:左, 1:上, 2:右, 3:下
    private Vector2Int currentDirection; // プレイヤーの進行方向

    [Header("イベント実行中か判定用")]
    // TextFieldをアタッチ。これが表示中の時は移動不可
    public RunningEventController textField;

    // ダンジョンでのバトル時の移動制限に関して
    public bool stopMoveInButtle = false;
    public bool OnEventMasInDungeon = false;
    void Start()
    {
        sensorTrigger = GameObject.Find("sensorTrigger").GetComponent<sensorTrigger>();

        //currentMap();

        // 現在のシーン名
        string currentScene = SceneManager.GetActiveScene().name;

        // 保存されたデータをロード
        playerPosition = SaveData.LoadPlayerPosition(currentScene);
        currentDirection = SaveData.LoadPlayerDirection(currentScene);

        // 保存されていない場合（初期値を設定）
        if (playerPosition == null || playerPosition == new Vector2Int(-1, -1))
        {
            // 初期位置をインスペクタで指定
            playerPosition = new Vector2Int(startX, startY);
            transform.position = new Vector3(-playerPosition.y * gridSize, this.transform.position.y, -playerPosition.x * gridSize);

            // 初期向きをインスペクタで指定
            SetDirectionFromIndex(directionIndex);

            // 初期値を保存
            SaveData.SavePlayerData(currentScene, playerPosition, currentDirection);
        }
        else
        {
            // 保存されたデータに基づき位置と向きを設定
            transform.position = new Vector3(-playerPosition.y * gridSize, this.transform.position.y, -playerPosition.x * gridSize);
            transform.rotation = Quaternion.LookRotation(new Vector3(currentDirection.y, 0, -currentDirection.x));
        }
    }


    void Update()
    {
        currentMap();
        // テキスト表示中 or クリア時は移動不可
        if ((textField != null && textField.eventRunning) || EventTrigger.GameClearFlag) 
        {
            // イベント実行中は移動の入力を無効にする
            sensorTrigger.PressW = false;
            sensorTrigger.PressS = false;
            sensorTrigger.PressA = false;
            sensorTrigger.PressD = false;
           // sensorTrigger.PressSubmit = false;

            return;
        }

        // プレイヤーの移動
        playerMovement();
    }


    // シーン切り替え時に位置を保存しておく
    void OnDisable()
    {
        // 現在のシーン名
        string currentScene = SceneManager.GetActiveScene().name;

        // 現在の位置と向きを保存
        SaveData.SavePlayerData(currentScene, playerPosition, currentDirection);
    }

    // 現在のシーンのマップ
    public void currentMap()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Forest":
                mapGrid = ForestMap.mapGrid;
                break;

            case "Desert":
                mapGrid = DesertMap.mapGrid;
                break;

            case "Dungeon":
                mapGrid = DungeonMap.mapGrid;
                break;

            default:
                break;
        }
    }
    void playerMovement()
    {
        // Wキー
        if (sensorTrigger.PressW && !sensorTrigger.PrevW)
        {
            MoveForward();
            // ダンジョンでのイベントとの競合防止
            if (!OnEventMasInDungeon)
            {
               // sensorTrigger.PressW = false;
                StartCoroutine(sensorTrigger.ResetW());
            }
        }
         sensorTrigger.PrevW = sensorTrigger.PressW;

        // S
        if (sensorTrigger.PressS && !sensorTrigger.PrevS)
        {
            // sensorTrigger.PressS = false;
            StartCoroutine(sensorTrigger.ResetS());
        }
        sensorTrigger.PrevS = sensorTrigger.PressS;


        // A
        if (sensorTrigger.PressA && !sensorTrigger.PrevA)
        {
           TurnLeft();
            //  sensorTrigger.PressA = false;
            StartCoroutine(sensorTrigger.ResetA());
        }
        sensorTrigger.PrevA = sensorTrigger.PressA;

        // D
        if (sensorTrigger.PressD && !sensorTrigger.PrevD)
        {
            TurnRight();
            // sensorTrigger.PressD = false;
            StartCoroutine(sensorTrigger.ResetD());
        }
        sensorTrigger.PrevD = sensorTrigger.PressD;

    }

    // インスペクタで指定された方向インデックスから初期方向を設定
    void SetDirectionFromIndex(int index)
    {
        switch (index)
        {
            case 0:
                currentDirection = Vector2Int.left;  // 左
                transform.rotation = Quaternion.Euler(0, 0, 0); // 左を向く
                break;
            case 1:
                currentDirection = Vector2Int.up;  // 上
                transform.rotation = Quaternion.Euler(0, 90, 0); // 上を向く
                break;
            case 2:
                currentDirection = Vector2Int.right;  // 右
                transform.rotation = Quaternion.Euler(0, 180, 0); // 右を向く
                break;
            case 3:
                currentDirection = Vector2Int.down;  // 下
                transform.rotation = Quaternion.Euler(0, -90, 0); // 下を向く
                break;
        }
    }

    /**/ // スムーズに移動
    IEnumerator MoveSmoothly(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end; // 最後に位置を確定
    }

    IEnumerator RotateSmoothly(Quaternion start, Quaternion end, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = end; // 最後に回転を確定
    }

    void MoveForward()
    {
        Vector2Int newPosition = new Vector2Int(playerPosition.x + currentDirection.x, playerPosition.y - currentDirection.y);

        if (newPosition.x >= 0 && newPosition.x < mapGrid.GetLength(1) &&
            newPosition.y >= 0 && newPosition.y < mapGrid.GetLength(0) &&
            mapGrid[newPosition.y, newPosition.x] != 1)
        {
            playerPosition = newPosition;
            Vector3 start = transform.position;
            Vector3 end = new Vector3(-playerPosition.y * gridSize, this.transform.position.y, -playerPosition.x * gridSize);
            StartCoroutine(MoveSmoothly(start, end, 0.2f)); // 0.2秒かけて移動
        }
    }

    void TurnLeft()
    {
        Quaternion startRotation = transform.rotation;
        currentDirection = new Vector2Int(-currentDirection.y, currentDirection.x);
        Quaternion endRotation = Quaternion.LookRotation(new Vector3(currentDirection.y, 0, -currentDirection.x));
        StartCoroutine(RotateSmoothly(startRotation, endRotation, 0.2f)); // 0.2秒かけて回転
    }

    void TurnRight()
    {
        Quaternion startRotation = transform.rotation;
        currentDirection = new Vector2Int(currentDirection.y, -currentDirection.x);
        Quaternion endRotation = Quaternion.LookRotation(new Vector3(currentDirection.y, 0, -currentDirection.x));
        StartCoroutine(RotateSmoothly(startRotation, endRotation, 0.2f)); // 0.2秒かけて回転
    }

    /**/


    // マップとプレイヤーの位置・向き
    public int[,] MapGrid
    {
        get { return mapGrid; }
        set { mapGrid = value; }
    }
    public Vector2Int PlayerPos
    {
        get { return playerPosition; }
        set { playerPosition = value; }
    }
    public Vector2Int PlayerDir
    {
        get { return currentDirection; }
        set { currentDirection = value; }
    }
}
