using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingButtons : MonoBehaviour
{
    // ゲーム終了
    public void EndGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

    // シーン切り替え ===================================
    public void ChangeDesertScene()
    {
        sensorTrigger.currentPage = 1;
    }

    public void ChangeDungeonScene()
    {
        sensorTrigger.currentPage = 2;
    }

    public void ChangeForestScene()
    {
        sensorTrigger.currentPage = 3;
    }
    // ================================================--

    // アイテム入手
    // 砂漠
    public void GetWeapon()
    {
        EventTrigger.GetWeapon = true;
    }
    public void GetKeyInDesert()
    {
        EventTrigger.GetKeyD = true;
    }

    // ダンジョン
    public void GetFire()
    {
        EventTrigger.GetFire = true;
    }
    public void DefeatedMonster()
    {
        EventTrigger.Monster = true;
    }
    public void Revival() // 復活
    {
        Events_Dungeon.playerLife = 5; // ライフリセット
        EventTrigger.GameOverFlag = false;
    }

    // 森林
    public void IsRainning()
    {
        EventTrigger.Raining = false;
    }
    public void GetKeyInForest()
    {
        EventTrigger.GetKeyF = true;
    }
}
