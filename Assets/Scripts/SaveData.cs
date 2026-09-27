using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    // プレイヤーの位置保存（シーンごとに管理）
    private static Dictionary<string, Vector2Int> playerPositions = new Dictionary<string, Vector2Int>();
    private static Dictionary<string, Vector2Int> playerDirections = new Dictionary<string, Vector2Int>();

    // プレイヤーの位置を保存
    public static void SavePlayerData(string sceneName, Vector2Int position, Vector2Int direction)
    {
        playerPositions[sceneName] = position;
        playerDirections[sceneName] = direction;
    }

    // プレイヤーの位置を取得
    public static Vector2Int LoadPlayerPosition(string sceneName)
    {
        if (!playerPositions.ContainsKey(sceneName))
            return new Vector2Int(-1, -1); // 保存されていない場合の値
        return playerPositions[sceneName];
    }


    // プレイヤーの向きを取得
    public static Vector2Int LoadPlayerDirection(string sceneName)
    {
        return playerDirections.ContainsKey(sceneName) ? playerDirections[sceneName] : Vector2Int.up; // デフォルトは上向き
    }
}
