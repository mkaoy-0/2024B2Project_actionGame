using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// プレイヤーの位置と向きをセットで管理するクラス
[System.Serializable]
public class PositionCondition
{
    public int x;           // X座標
    public int y;           // Y座標
    public Vector2Int dir;  // 向き (例: (0,1) 上, (0,-1) 下, (-1,0) 左, (1,0) 右)
}

public class IconAnimController : MonoBehaviour
{

    // マップや位置取得用スクリプト
    public PlayerMove _playerMove;

    // アイコンのアニメーションを再生する位置
    [Header("上(0,1), 下(0,-1), 左(-1,0), 右(1,0)")]
    public List<PositionCondition> conditions = new List<PositionCondition>(); // 設定する条件リスト

    // アニメーター取得
    private Animator anim;

    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    void Update()
    {
        bool animPlaying = false;
        // 条件リストと一致するかチェック
        if (conditions != null)
        {
            foreach (var condition in conditions)
            {
                if (_playerMove.PlayerPos.x == condition.x &&
                    _playerMove.PlayerPos.y == condition.y &&
                    _playerMove.PlayerDir == condition.dir)
                {
                    // 条件を満たしたときの処理
                    animPlaying = true;
                    break; // 1つでも一致すれば処理を実行
                }
            }
        }

        // アニメーターのBoolパラメータを変更
        if (anim != null)
        {
            anim.SetBool("OnEventPoint", animPlaying);
        }

    }
}
