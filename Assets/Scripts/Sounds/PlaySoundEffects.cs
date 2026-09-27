using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEffects : MonoBehaviour
{
    // Œø‰Ê‰¹
    public AudioSource audioSource;

    public AudioClip attack;
    public AudioClip damaged;
    public AudioClip getItem;

    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(attack);
    }

    public void PlayDamagedSound()
    {
        audioSource.PlayOneShot(damaged);
    }

    public void PlayGetItemSound()
    {
        audioSource.PlayOneShot(getItem);
    }
}
