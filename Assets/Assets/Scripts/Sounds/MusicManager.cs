using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // Sprawd�, czy instancja ju� istnieje
        if (instance == null)
        {
            // Ustaw instancj�
            instance = this;
            // Nie niszcz obiektu przy zmianie sceny
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Je�li instancja ju� istnieje, zniszcz duplikat
            Destroy(gameObject);
        }
    }
}