using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSoundController : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        // ここで最初に音を止めておく（必要なら）
        audioSource.Stop();
    }

    void Update()
    {
        if (EventTrigger.Raining && sensorTrigger.currentPage == 3)
        {
            // すでに再生中でない場合のみ再生
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // すでに止まっている場合は何もしない
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
