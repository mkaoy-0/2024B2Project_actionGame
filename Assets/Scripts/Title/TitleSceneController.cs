using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleSceneController : MonoBehaviour
{
    public sensorTrigger sensorTrigger;

    public GameObject titlePanel;
    public CanvasGroup introPanel;
    public TextMeshProUGUI introText;
    public float alpha = 0; // フェード
    public bool isFade = false;

    public bool startGame = false;

    // 効果音
    public AudioSource audioSource;
    public AudioClip soundEffect;

    void Start()
    {
        
    }

    void Update()
    {
        StartFade();

        if (sensorTrigger.PressSubmit)
        {
            if (!startGame)
            {
                isFade = true;
                audioSource.PlayOneShot(soundEffect);
            }
            else
            {
                sensorTrigger.currentPage = 1;
            }
        }
        sensorTrigger.PressSubmit = false;
    }


    // 
    public void StartFade()
    {
        if (isFade)
        {
            introPanel.alpha = alpha;
            if (alpha != 1f) alpha += 0.05f;
            if (alpha >= 1f)
            {
                if (titlePanel.activeSelf) titlePanel.SetActive(false);
                alpha = 1f;
                StartCoroutine(ShowIntroText());
                isFade = false;
            }
        }
    }

    // 説明表示
    IEnumerator ShowIntroText()
    {
        yield return new WaitForSeconds(1f);
        startGame = true;
    }
}
