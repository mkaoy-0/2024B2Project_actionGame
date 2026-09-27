using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    private static BGMController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Šù‚É‘¶İ‚·‚éê‡‚ÍV‚µ‚¢•û‚ğíœ
        }
    }
}
