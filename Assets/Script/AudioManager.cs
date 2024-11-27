using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{
    public AudioSource bgm;
    public AudioClip defaultMusic;
    public AudioClip prisonMusic;
    public AudioSource sfxSource;
    public List<AudioClip> prisonLockSounds;
    public AudioClip prisonDoorOpenSound;
    public AudioClip prisonDustLeafSound;
    public AudioClip buttonClickSound;

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
        // 기본적으로 반복 재생
        bgm.loop = true; 
        // 기본 음악 설정
        bgm.clip = defaultMusic; 
        bgm.Play();
    }
     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 다른 씬으로 돌아왔을 경우
        if (scene.name == "TitleScene" && scene.name == "LobbyScene" && scene.name == "RoomScene")
        {
            ChangeMusic(defaultMusic);
            bgm.volume = 1.0f;
        }
        // PrisonScene일 경우
        if (scene.name == "PrisonScene") 
        {
            if(scene.name == "PrisonDoorPuzzleScene" || 
            scene.name == "PrisonDustPuzzleScene" || 
            scene.name == "PrisonLeafPuzzleScene" || 
            scene.name == "PrisonPipePuzzleScene") 
            return;
            bgm.volume = 0.2f;
            ChangeMusic(prisonMusic);
        }
    }

    private void ChangeMusic(AudioClip newMusic)
    {
        // 이미 해당 음악이 재생 중이면 변경하지 않음
        if (bgm.clip == newMusic) return; 

        bgm.Stop();
        bgm.clip = newMusic;
        bgm.Play();
    }

    // 랜덤 효과음 재생 메서드
    public void PlayRandomPrisonDoorPuzzleKeySound()
    {
        if (prisonLockSounds.Count == 0) 
        {
            Debug.Log("PrisonLockSounds.Count == 0");   
            return;
        }
        AudioClip randomClip = prisonLockSounds[Random.Range(0, prisonLockSounds.Count)];
        float volumeScale = 3.0f;
        // 열쇠소리를 더 크게 하고자 함.
        sfxSource.PlayOneShot(randomClip, volumeScale);
        Debug.Log("PrisonLockSound : 정상적으로 사운드가 재생되었습니다!");
    }

    public void PrisonDoorOpenSound()
    {  
        float volumeScale = 3.0f;
        sfxSource.PlayOneShot(prisonDoorOpenSound, volumeScale);
        Debug.Log("PrisonDoorOpen : 정상적으로 사운드가 재생되었습니다!");
    }

    public void PrisonDustLeafRustingSound()
    {
        float volumeScale = 3.0f;
        sfxSource.PlayOneShot(prisonDustLeafSound,volumeScale);
        Debug.Log("PrisonDustLeafRusting : 정상적으로 사운드가 재생되었습니다!");
    }

    public void ButtonClickSound()
    {
        float volumeScale = 3.0f;
        sfxSource.PlayOneShot(buttonClickSound, volumeScale);
        Debug.Log("ButtonClickSound : 정상적으로 사운드가 재생되었습니다!");
    }
}
