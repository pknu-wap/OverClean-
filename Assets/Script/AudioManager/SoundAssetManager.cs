using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SoundAssetManager : MonoBehaviour
{
    public AudioSource sfxSource;
    public List<AudioClip> prisonLockSounds;
    public AudioClip prisonDoorOpenSound;
    public AudioClip prisonDustLeafSound;
    public AudioClip buttonClickSound;

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
        // FindObjectOfType를 사용하여 다른 씬의 AudioSource를 가져오기
        AudioSource prisonSceneAudioSource = FindObjectOfType<AudioSource>();
        if (prisonSceneAudioSource != null)
        {
            prisonSceneAudioSource.PlayOneShot(buttonClickSound, 3.0f);
            Debug.Log("ButtonClickSound played on PrisonScene's AudioSource.");
        }
}
}
