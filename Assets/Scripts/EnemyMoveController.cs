using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveController : MonoBehaviour
{
   
    public PlayerMove _playerMove;

    public GameObject enemy; // 敵
    public GameObject enemyEffect; // パーティクル

    [Header("敵のHP")]
    public int enemyMaxHP = 3;

    [Header("敵の行動クールタイム")]
    public float enemySpeed = 2.0f;

    public bool IsMoving = false; // 移動中かどうか
    public bool isMovingCoroutineRunning = false; // コルーチンが動いているか

    // アニメーター
    private Animator enemyAnimator;


    void Start()
    {
        enemyAnimator = enemy.GetComponent<Animator>();
    }

    void Update()
    {

        if (EventTrigger.GetWeapon) // 武器を持っていたらプレイヤーを追う
        {
            //   ChasePlayer();
            if (10 <= _playerMove.PlayerPos.x && 10 <= _playerMove.PlayerPos.y) // プレイヤーが一定範囲に近付いたら
            {
                enemyAnimator.SetTrigger("wakeUp");
            }
        }

    }

    // ===================================
    // 敵の移動
    public IEnumerator MoveEnemy()
    {
        isMovingCoroutineRunning = true;
        while (enemy.activeSelf)
        {
            yield return new WaitForSeconds(enemySpeed); // 一定間隔で行動

            if (IsPlayerNearby()) // プレイヤーが隣接していたら攻撃
            {
                if (!IsAnimationPlaying("Get Hit"))
                {
                    AttackPlayer();
                }
            }
        }

        isMovingCoroutineRunning = false;
    }

    // アニメーション再生中かどうか
    bool IsAnimationPlaying(string animName)
    {
        AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animName) && stateInfo.normalizedTime < 1.0f;
    }



    // プレイヤーが隣接マスにいるかチェック
    public bool IsPlayerNearby()
    {
        bool isPlayerNearby = false;
        if (_playerMove.PlayerPos == new Vector2Int(12, 11) || _playerMove.PlayerPos == new Vector2Int(13, 11))
        {
            isPlayerNearby = true;
        }

        return isPlayerNearby;
    }

    // 攻撃
    void AttackPlayer()
    {
        // アニメーション
        enemyAnimator.SetTrigger("attack");

        // プレイヤーにダメージを与えた
         ReducePlayerLife();
    }

    // プレイヤーのライフを減らす
    public void ReducePlayerLife()
    {
        if (Events_Dungeon.hitCount < enemyMaxHP && !IsAnimationPlaying("Scream"))
        {
            // プレイヤーにダメージを与えた
            Events_Dungeon.playerLife--;
            Debug.Log("モンスターの攻撃！" + Events_Dungeon.playerLife);

        }
    }

    // 攻撃を喰らったとき
    public void GetHit()
    {
        enemyAnimator.SetTrigger("getHit");
    }
    // HPが０になったとき
    public void Die()
    {
        enemyAnimator.SetBool("die", true);
        if (IsAnimationFinished("Die"))
        {
            enemy.SetActive(false);
            enemyEffect.SetActive(false);
            EventTrigger.Monster = true;
        }
    }
    // プレイヤーのHPが０になったとき
    public void Victory(bool a)
    {
        enemyAnimator.SetBool("victory", a);
    }

    // アニメーション再生が終了したか
    bool IsAnimationFinished(string animName)
    {
        AnimatorStateInfo stateInfo = enemyAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animName) && stateInfo.normalizedTime >= 1.0f;
    }
}
