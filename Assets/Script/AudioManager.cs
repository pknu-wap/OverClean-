using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    // 씬별 음악 리스트
    public AudioClip[] backgroundMusic; 
    private AudioSource audioSource;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

void Start()
    {
        // 배경음악 자동 재생
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        // 볼륨 조절
        audioSource.volume = volume; 
    }

    // 오디오의 현재 재생 위치 가져오기
    public float GetCurrentTime()
    {
        return audioSource.time;
    }

    // 오디오의 특정 위치에서 재생 시작
    public void SetCurrentTime(float time)
    {
        audioSource.time = time;
    }

}
