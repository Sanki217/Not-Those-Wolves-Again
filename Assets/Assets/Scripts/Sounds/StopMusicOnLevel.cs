using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMusicOnLevel : MonoBehaviour
{
    void Start()
    {
        GameObject musicPlayer = GameObject.Find("MusicPlayer");
        if (musicPlayer != null)
        {
            AudioSource audioSource = musicPlayer.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Pause(); // Pauzuje muzykê zamiast j¹ niszczyæ
            }
        }
    }
}