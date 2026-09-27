using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    private string[] sceneName = new string[3];

    void Start()
    {
        // シーン名を設定
        sceneName[0] = "Desert";
        sceneName[1] = "Dungeon";
        sceneName[2] = "Forest";
    }

    void Update()
    {
        sceneChange();
    }

    void sceneChange()
    {
        // 押したキー(sensorTrigger参照)に応じてシーン変更

        Scene currentScene = SceneManager.GetActiveScene();
        switch (sensorTrigger.CurrentPage)
        {
            case 1:
                if (currentScene.name != sceneName[0]) 
                { 
                    SceneManager.LoadScene(sceneName[0], LoadSceneMode.Single);
                }
                break;

            case 2:
                if (currentScene.name != sceneName[1])
                {
                    SceneManager.LoadScene(sceneName[1], LoadSceneMode.Single);
                }
                break;

            case 3:
                if (currentScene.name != sceneName[2])
                {
                    SceneManager.LoadScene(sceneName[2], LoadSceneMode.Single);
                }
                break;

            case -1: // タイトルシーン
                if (currentScene.name != "Title")
                {
                    SceneManager.LoadScene("Title", LoadSceneMode.Single);
                }
                break;

            default:
                break;
        }
    }
}
