using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public bool isPaused = false;

    public bool isTransitioningPauseState = false;

    private void Awake()
    {
        // 싱글톤 패턴 구현: NetworkingManager가 중복되지 않도록 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // 씬 전환 시에도 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
    }
}
