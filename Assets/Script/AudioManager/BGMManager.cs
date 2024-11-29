using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;
    public AudioSource bgm;
    public AudioClip defaultMusic;
    public AudioClip prisonMusic;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning($"Duplicate BGMManager detected and destroyed: {gameObject.name}");
            Destroy(gameObject);
        }
        var soundManangers = FindObjectsOfType<BGMManager>();
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
        // 기본적으로 반복 재생
        bgm.loop = true; 
        // 기본 음악 설정
        bgm.clip = defaultMusic; 
        bgm.Play();
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 다른 씬으로 돌아왔을 경우
        if (scene.name == "TitleScene" && scene.name == "LobbyScene" && scene.name == "RoomScene")
        {
            ChangeMusic(defaultMusic, 1.0f);
        }
        // PrisonScene일 경우
        if (scene.name == "PrisonScene") 
        {
            if(scene.name == "PrisonDoorPuzzleScene" || 
            scene.name == "PrisonDustPuzzleScene" || 
            scene.name == "PrisonLeafPuzzleScene" || 
            scene.name == "PrisonPipePuzzleScene") 
            return;
            ChangeMusic(prisonMusic, 0.3f);
        }
    }
    public void SetMusicForScene(string sceneName)
    {
        // 씬 이름에 따라 배경음악 설정
        switch (sceneName)
        {
            case "MapChooseScene":
                ChangeMusic(defaultMusic, 1.0f); // MapChooseScene 음악 설정
                break;
            case "LobbyScene":
                ChangeMusic(defaultMusic, 1.0f); // 기본 음악 설정
                break;
            default:
                Debug.Log("해당 씬에 맞는 음악이 없습니다.");
                break;
        }
    }

    private void ChangeMusic(AudioClip newMusic, float volume)
    {
        // 이미 해당 음악이 재생 중이면 변경하지 않음
        if (bgm.clip == newMusic) return; 

        bgm.Stop();
        bgm.clip = newMusic;
        bgm.volume = volume;
        bgm.Play();
    }
}
