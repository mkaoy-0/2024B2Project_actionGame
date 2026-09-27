using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions; // 追加





public class ESP32KeyController : MonoBehaviour
{

   // private string esp32IP = "192.168.3.247"; // ← ここをESP32のIPアドレスに変更
    private string esp32IP = "192.168.11.25"; // ← ここをESP32のIPアドレスに変更
    private string lastKey = "";
    public sensorTrigger sensorTrigger;  // sensorTriggerクラスのインスタンスを参照

    void Start()
    {

      // sensorTriggerがInspectorで設定されている場合、Startで参照を取得
        if (sensorTrigger == null)
        {
            sensorTrigger = GameObject.FindObjectOfType<sensorTrigger>();
        }

        StartCoroutine(GetKeyData());
    }

    IEnumerator GetKeyData()
    {
        while (true)
        {
          using (UnityWebRequest request = UnityWebRequest.Get("http://" + esp32IP + "/key"))

            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    string key = ParseKey(json);

                    if (key != lastKey)
                    {
                        ProcessKeyInput(key);
                        lastKey = key;
                    }
                }
                else
                {
                    Debug.LogError("ESP32通信エラー: " + request.error);
                }
            }

            yield return new WaitForSeconds(0.5f); // 100msごとにデータ取得
        }
    }

  string ParseKey(string json)
{
   // Debug.Log("受信したJSON: " + json); // JSONの中身を確認
    Match match = Regex.Match(json, "\"key\":\\s*\"(.*?)\"");
    return match.Success ? match.Groups[1].Value : "";
}


    void ProcessKeyInput(string key)
    {
           Debug.Log("受信キー: " + key);  // 受信したキーを確認
        switch (key)
        {
        case "w":
            Debug.Log("前進 (W)");
            sensorTrigger.PressW = true;  // sensorTriggerのWフラグをtrueに設定
            break;

        case "a":
            Debug.Log("左へ (A)");
            sensorTrigger.PressA = true;  // sensorTriggerのAフラグをtrueに設定
            break;

        case "s":
            Debug.Log("後退 (S)");
            sensorTrigger.PressS = true;  // sensorTriggerのSフラグをtrueに設定
            break;
            
        case "d":
            Debug.Log("右へ (D)");
            sensorTrigger.PressD = true;  // sensorTriggerのDフラグをtrueに設定
            break;
        }
            // フラグの状態を確認
    Debug.Log("PressW: " + sensorTrigger.PressW);
    Debug.Log("PressA: " + sensorTrigger.PressA);
    Debug.Log("PressS: " + sensorTrigger.PressS);
    Debug.Log("PressD: " + sensorTrigger.PressD);


    }
}
