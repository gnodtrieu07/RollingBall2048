using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource mergeSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioSource winSound;

    public void PlayMergeSound()
    {
        mergeSound.Play();
    }

    public void PlayDamageSound()
    {
        damageSound.Play();
    }

    public void PlayHitSound()
    {
        hitSound.Play();
    }
    public void PlayWinSound()
    {
        winSound.Play();
    }
}
