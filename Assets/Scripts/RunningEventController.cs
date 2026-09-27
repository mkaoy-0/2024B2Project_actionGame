using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningEventController : MonoBehaviour
{
    // イベント実行中かどうか

    public bool eventRunning = false; // オブジェクトがアクティブならtrue

    void OnEnable()
    {
        eventRunning = true; // オブジェクトがアクティブになったとき
    }

    void OnDisable()
    {
        eventRunning = false; // オブジェクトが非アクティブになったとき
    }
}
