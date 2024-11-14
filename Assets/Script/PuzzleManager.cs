using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static PuzzleManager instance;
    // 플레이어를 참조해서 위치를 받아오기 위한 변수
    public Transform playerLocation;

    // 맵 씬과 퍼즐 씬의 퍼즐 성공 여부를 연결하기 위한 변수
    public bool isPuzzleSuccess = false;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayerCanMove()
    {
        playerLocation.GetComponent<PlayerManager>().canMove = true;
    }

    // 퍼즐 성공 시 호출되는 함수
    public void PuzzleSuccess()
    {
        isPuzzleSuccess = true;
    }
}
