using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // SprawdŸ, czy instancja ju¿ istnieje
        if (instance == null)
        {
            // Ustaw instancjê
            instance = this;
            // Nie niszcz obiektu przy zmianie sceny
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Jeœli instancja ju¿ istnieje, zniszcz duplikat
            Destroy(gameObject);
        }
    }
}