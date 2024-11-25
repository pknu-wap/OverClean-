using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{
    public AudioSource bgm;

    private void Awake()
    {
        var soundManangers = FindObjectsOfType<AudioManager>();
        if(soundManangers.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        bgm.Play();
    }
}
