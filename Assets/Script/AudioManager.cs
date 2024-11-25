using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{
    public AudioSource bgm;
    public AudioClip defaultMusic;
    public AudioClip prisonMusic;

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
    private void OnEnable()
    {
        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 씬 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        bgm.loop = true; // 기본적으로 반복 재생
        bgm.clip = defaultMusic; // 기본 음악 설정
        bgm.Play();
    }
     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PrisonScene") // PrisonScene일 경우
        {
            ChangeMusic(prisonMusic);
        }
        else // 다른 씬으로 돌아왔을 경우
        {
            ChangeMusic(defaultMusic);
        }
    }

    private void ChangeMusic(AudioClip newMusic)
    {
        if (bgm.clip == newMusic) return; // 이미 해당 음악이 재생 중이면 변경하지 않음

        bgm.Stop();
        bgm.clip = newMusic;
        bgm.Play();
    }
}
