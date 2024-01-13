using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioSource music;
    private void Awake()
    {
        music = GetComponent<AudioSource>();
    }
    public void SetMusic(float value)
    {
        music.volume = value;
    }
}
