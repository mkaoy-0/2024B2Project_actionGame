using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Events_Dungeon : MonoBehaviour
{
    // 効果音スクリプト
    public PlaySoundEffects playSoundEffects;   
    
    // マップや位置取得用スクリプト
    public GameObject _player;
    public PlayerMove _playerMove;
    public sensorTrigger sensorTrigger;

    // 敵の動き
    public EnemyMoveController enemyMove;

    // テキスト
    public GameObject textField;
    public TextMeshProUGUI eventText;

    // テキスト表示用変数
    private static int stateInDungeon = 0;
    private static int state_hint = 0;
    private static bool showText = false;
    private static string text;

    // 
    public GameObject fire; // 松明
    public GameObject enemy; // 敵
    public GameObject lastDoor; // 最後のドア
    private GameObject key_1, key_2, magicParticle; // lastDoorの子オブジェクト
    private static bool checkedDoor = false; // 石板を一つ持っている状態でドアを調べたか

    // アイコン
    public GameObject icon_fire; // 松明アイコン
    public GameObject icon_enemy; // 敵エリアアイコン
    public GameObject icon_door; // ドアアイコン


    // 敵とのバトル 
    public static int playerLife = 5; // プレイヤーHP
    public float attackCooldown = 1.0f; // 攻撃クールタイム
    private bool isCoolingDown = false;
    public static int hitCount = 0; // ヒット回数
    // 攻撃アイコン
    public GameObject attackIcon;
    // 攻撃エフェクト
    public ParticleSystem attackParticle;
    // ゲームオーバーで画面暗転
    public Image gameOverPanel;



    void Start()
    {
      //  _playerMove = _player.GetComponent<PlayerMove>();
      //  sensorTrigger = GameObject.Find("sensorTrigger").GetComponent<sensorTrigger>();

     //   eventText = textField.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        eventText.text = text;

        key_1 = lastDoor.transform.GetChild(1).transform.GetChild(0).gameObject; // 石板右
        key_2 = lastDoor.transform.GetChild(2).transform.GetChild(0).gameObject; // 石板真ん中
        magicParticle = lastDoor.transform.GetChild(3).gameObject; // 魔法陣

        // ページ遷移時のテキスト表示処理
        if (showText)
        {
            textField.SetActive(true);
        }
        else
        {
            textField.SetActive(false);
        }

        // 松明入手後は非表示
        if (EventTrigger.GetFire)
        {
            if (fire.activeSelf) fire.SetActive(false);
            // アイコン非表示
            if (icon_fire.activeSelf) icon_fire.SetActive(false);
        }

        // 鍵を一つ持っている状態でボス部屋ドアを調べた後
        if (checkedDoor)
        {
            // 片方の石板を表示
            if (!key_1.activeSelf) key_1.SetActive(true);
        }
        // ボス部屋開いた後
        if (EventTrigger.GameClearFlag)
        {
            // アイコン非表示
            if (icon_door.activeSelf) icon_door.SetActive(false);
        }

        // 敵移動
        if (EventTrigger.Monster)
        {
            if(enemy.activeSelf) enemy.SetActive(false);
        }


        // 武器入手後にマップ変更
        if (EventTrigger.GetWeapon)
        {
            if (DungeonMap.GetDungeonMap(10, 14) == 1)
            {
                // マップ変更
                DungeonMap.SetDungeonMap(10, 14, 0);
            }
            if (showText && text == "この先にモンスターがいる。今の状態では倒せない。何か武器があれば…。")
            {
                if (textField.activeSelf) textField.SetActive(false);
                showText = false;
            }
            // アイコン非表示
            if (icon_enemy.activeSelf) icon_enemy.SetActive(false);
        }
    }

    void Update()
    {
        // 敵移動に関する処理
        // 敵オブジェクトが非アクティブなら移動を停止
        if (!enemy.activeSelf)
        {
            StopAllCoroutines(); // すべてのコルーチンを停止
        }
        else if (!enemyMove.IsMoving && !enemyMove.isMovingCoroutineRunning) // 一度停止したら再開
        {
            StartCoroutine(enemyMove.MoveEnemy());
        }

        // ゲームオーバー ========================
        // 他のページに移動で復活
        if (playerLife <= 0)
        {         
            GameOver();
            EventTrigger.GameOverFlag = true;
            enemyMove.Victory(true);
            // 攻撃アイコン非表示
            if (attackIcon.activeSelf) attackIcon.SetActive(false);
            textField.SetActive(true);
            eventText.text = "負けてしまった…。一晩休めば回復するだろう。";
        }
        else // 復活時テキスト非表示
        {
            if (eventText.text == "負けてしまった…。一晩休めば回復するだろう。")
            {
                if (textField.activeSelf) textField.SetActive(false);
            }
            enemyMove.Victory(false);
        }
        // =========================================

        // その他イベント
        EventAtDungeon();

        // 決定ボタンのboolを元に戻す
        if (sensorTrigger.PressSubmit)
        {
            sensorTrigger.PressSubmit = false;
        }
    }

    public void EventAtDungeon()
    {
        // 松明入手(1, 13) (0, 14)
        if ((_playerMove.PlayerPos == new Vector2Int(1, 13) && _playerMove.PlayerDir == Vector2Int.down)
            || (_playerMove.PlayerPos == new Vector2Int(0, 14) && _playerMove.PlayerDir == Vector2Int.right))
        {
            if (sensorTrigger.PressSubmit)
            {
                Event_GetFire();
              //  sensorTrigger.PressSubmit = false;
            }
        }

        // ヒント入手(3,5, left)
        if (_playerMove.PlayerPos == new Vector2Int(3, 5) && _playerMove.PlayerDir == Vector2Int.left)
        {
            if (sensorTrigger.PressSubmit)
            {
                Event_GetHint();
              //  sensorTrigger.PressSubmit = false;
            }
        }

        // 敵発見(14, 9)
        if (_playerMove.PlayerPos == new Vector2Int(14, 9) && _playerMove.PlayerDir == Vector2Int.down)
        {
            // 武器を入手していない場合
            if (!EventTrigger.GetWeapon)
            {
                // playerMoveとWキーが競合しないように
                _playerMove.OnEventMasInDungeon = true;
                if (sensorTrigger.PressW)
                {
                    if (!textField.activeSelf) textField.SetActive(true);
                    showText = true;
                    text = "この先に風のモンスターがいる。今の状態では倒せない。何か武器があれば…。";
                    eventText.text = text;
                    sensorTrigger.PressW = false;
                }
                if (sensorTrigger.PressSubmit)
                {
                    if (textField.activeSelf) textField.SetActive(false);
                    showText = false;
                    //   sensorTrigger.PressSubmit = false;
                }
            }
            else
            {
                _playerMove.OnEventMasInDungeon = false;
                if (textField.activeSelf) textField.SetActive(false);
                showText = false;

            }
        }
        else
        {
            _playerMove.OnEventMasInDungeon = false;
        }

        // 武器入手時かつ敵を倒していないとき、敵とのバトル
        if (EventTrigger.GetWeapon && !EventTrigger.Monster && playerLife > 0)
        {
            // 敵と隣り合ってる＋敵の方を向いている
            if (enemyMove.IsPlayerNearby() && _playerMove.PlayerDir == Vector2Int.down)
            {
                if (!attackIcon.activeSelf) attackIcon.SetActive(true);                              // 
                Event_Buttle();
            }
            else
            {
                if (attackIcon.activeSelf) attackIcon.SetActive(false);
            }
        }
        // 敵撃破後は攻撃アイコン非表示、Wキー移動解禁
        if (EventTrigger.Monster)
        {
            // 敵のマスにもいけるようにする
            if (DungeonMap.GetDungeonMap(12, 11) == 1)
            {
                DungeonMap.SetDungeonMap(12, 11, 0);
                DungeonMap.SetDungeonMap(12, 12, 0);
                DungeonMap.SetDungeonMap(12, 13, 0);
                DungeonMap.SetDungeonMap(12, 14, 0);

                textField.SetActive(true);
                showText = true;
                text = "モンスターを倒した！";
                eventText.text = text;
            }

            if (attackIcon.activeSelf) attackIcon.SetActive(false);
            if (sensorTrigger.PressSubmit && eventText.text == "モンスターを倒した！")
            {
                textField.SetActive(false);
                showText = false;
            }
        }


        // ボス部屋ドア(8, 5)
        if (_playerMove.PlayerPos == new Vector2Int(8, 5) && _playerMove.PlayerDir == Vector2Int.up)
        {
            if (sensorTrigger.PressSubmit)
            {
                Event_Boss();
             //   sensorTrigger.PressSubmit = false;
            }
        }
    }

    // 松明入手イベント
    public void Event_GetFire()
    {
        // 未入手
        if (!EventTrigger.GetFire)
        {
            switch (stateInDungeon)
            {
                case 0:
                    if (!textField.activeSelf) textField.SetActive(true);
                    showText = true;
                    text = "松明が落ちている。";
                    eventText.text = text;
                    stateInDungeon = 1;
                    break;

                case 1:
                    text = "松明を手に入れた。";
                    eventText.text = text;
                    EventTrigger.GetFire = true;
                    if (fire.activeSelf) fire.SetActive(false);
                    // アイコン非表示
                    if (icon_fire.activeSelf) icon_fire.SetActive(false);
                    stateInDungeon = 2;
                    playSoundEffects.PlayGetItemSound();
                    break;
            }
        }
        else // 入手済
        {
            switch (stateInDungeon)
            {
                case 2:
                    if (textField.activeSelf) textField.SetActive(false);
                    showText = false;
                    stateInDungeon = 3;
                    break;

                case 3:
                    if (!textField.activeSelf) textField.SetActive(true);
                    showText = true;
                    text = "もう何もない。";
                    eventText.text = text;
                    stateInDungeon = 2;
                    break;
            }
        }
    }

    // モンスターに関するヒント
    public void Event_GetHint()
    {
        switch (state_hint)
        {
            case 0:
                if (!textField.activeSelf) textField.SetActive(true);
                showText = true;
                text = "何かのメモが落ちている。";
                eventText.text = text;
                state_hint = 1;
                break;

            case 1:
                text = "『モンスターが砂嵐を支配している。倒せば嵐は収まるだろう。』";
                eventText.text = text;
                state_hint = 2;
                break;

            case 2:
                if (textField.activeSelf) textField.SetActive(false);
                showText = false;
                state_hint = 0;
                break;
        }

    }

    // ボスイベント
    public void Event_Boss()
    {
        if (!textField.activeSelf)
        {
            textField.SetActive(true);
            showText = true;
            // 鍵を二つ入手済み
            if (EventTrigger.GetKeyD && EventTrigger.GetKeyF)
            {
                text = "魔法陣が起動した。";
                eventText.text = text;
                EventTrigger.GameClearFlag = true;
                // if (icon_door.activeSelf) icon_door.SetActive(false);　// アイコン非表示
                // 石板表示
                if (!key_1.activeSelf) key_1.SetActive(true);
                if (!key_2.activeSelf) key_2.SetActive(true);
                // 魔法陣表示
                if (!magicParticle.activeSelf) magicParticle.SetActive(true);
            }
            // 鍵を片方入手済み
            else if (EventTrigger.GetKeyD || EventTrigger.GetKeyF)
            {
                text = "もう1つの台にも同じように石板を置くことができそうだ。";
                eventText.text = text;
                // 石板表示
                if (!key_1.activeSelf) key_1.SetActive(true);
                checkedDoor = true;
            }
            else // 鍵未入手
            {
                text = "台の上に石板が置いてある。残りの2つの台にも何か置くことができそうだ。";
                eventText.text = text;
            }
        }
        else // テキストが表示されているときは非表示にする
        {
            textField.SetActive(false);
            showText = false;
        }
    }

    // ========================================
    // 敵とのバトル処理
    void Event_Buttle()
    {
        if (!isCoolingDown && sensorTrigger.PressSubmit)
        {

            StartCoroutine(Attack());
        }

        // 一定回数攻撃を当てたら
        if (hitCount >= enemyMove.enemyMaxHP)
        {
            enemyMove.Die();
        }

    }
    IEnumerator Attack()
    {
        hitCount++;
        isCoolingDown = true;
        sensorTrigger.PressSubmit = false;
        Debug.Log("ヒット回数: " + hitCount);

        // 攻撃アイコン非表示
        Image[] iconImgs = attackIcon.GetComponentsInChildren<Image>(true);
        TextMeshProUGUI iconTexts = attackIcon.GetComponentInChildren<TextMeshProUGUI>(true);
        foreach (Image img in iconImgs)
        {
            img.enabled = false;
        }
        iconTexts.enabled = false;
      //   attackIcon.SetActive(false);

        // 攻撃エフェクト
        attackParticle.Play();

        // 効果音
        playSoundEffects.PlayAttackSound();

        // 敵被弾アニメーション
        enemyMove.GetHit();

        yield return new WaitForSeconds(attackCooldown);

        isCoolingDown = false;
        // 攻撃アイコン表示
        foreach (Image img in iconImgs)
        {
            img.enabled = true;
        }
        iconTexts.enabled = true;
    }


    void GameOver()
    {
        // ゲームオーバー時初期位置に戻す
        if (gameOverPanel.color.a >= 0.9f)
        {
            _playerMove.PlayerPos = new Vector2Int(7, 13);
            _playerMove.PlayerDir = Vector2Int.up;
            _player.transform.position = new Vector3(-_playerMove.PlayerPos.y * _playerMove.gridSize, 4f, -_playerMove.PlayerPos.x * _playerMove.gridSize);
            _player.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

}
