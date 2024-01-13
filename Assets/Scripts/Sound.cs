using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public static Sound Instance;
    public AudioSource audioSource;
    public AudioClip correct;
    public AudioClip wrong;
    public AudioClip endgame;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }
    public void SetSound(float value)
    {
        audioSource.volume = value;
    }
    public void CorrectSound()
    {
        audioSource.PlayOneShot(correct);
    }
    public void WrongSound()
    {
        audioSource.PlayOneShot(wrong);
    }
    public void EndGameSound()
    {
        audioSource.PlayOneShot(endgame);
    }
}
