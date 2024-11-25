using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageManager : MonoBehaviour
{
    // 오브젝트 관련 변수
    public GameObject[] interactObject; 
    public GoalZone goalZone;
    public Color goalZoneColor;
    public bool[] interactionsCompleted;
    public int interactCount = 0;
    public static bool fusePuzzleSolved = false;
    
    // 플레이어 관리
    public PlayerManager player1;
    public PlayerManager player2;
    public float player1StartX;
    public float player1StartY;
    public float player1StartZ;
    public float player2StartX;
    public float player2StartY;
    public float player2StartZ;
    
    // 타이머 관련 변수
    public float limitTime;
    public float remainTime;
    public float elapsedTime = 0f;
    public bool isTimeOver = false;
    public TMP_Text timerText;
    public Slider timeSlider;
    public bool isPaused;
    void Awake()
    {
        goalZone = FindAnyObjectByType<GoalZone>();
        // 플레이어 생성 및 할당
        if (PhotonNetwork.IsConnected)
        {
            // 데이브 할당 / 생성
            if (PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString() == "Dave")
            {
                GameObject playerObj1 = PhotonNetwork.Instantiate("Player1", new Vector3(player1StartX, player1StartY, player1StartZ), Quaternion.identity);
                player1 = playerObj1.GetComponent<PlayerManager>();
            }
            // 매튜 할당 / 생성
            else
            {
                GameObject playerObj2 = PhotonNetwork.Instantiate("Player2", new Vector3(player2StartX, player2StartY, player2StartZ), Quaternion.identity);
                player2 = playerObj2.GetComponent<PlayerManager>();
            }
        }
    }
    
    void Start()
    { 
        // 상호작용 오브젝트 개수만큼 bool 배열 정의
        interactionsCompleted = new bool[interactObject.Length];
        remainTime = limitTime;

        // 슬라이더 초기화
        if (timeSlider != null)
        {
            timeSlider.maxValue = limitTime;
            timeSlider.value = limitTime; 
        }
    }

    void Update()
    {
        // 타임오버 및 스테이지 클리어 체크
        if (!isTimeOver && !goalZone.stageClear && !isPaused)
        {
            remainTime -= Time.deltaTime;
            elapsedTime += Time.deltaTime;

            // 슬라이더 업데이트
            if (timeSlider != null)
            {
                timeSlider.value = remainTime; 
            }
            if (remainTime <= 0)
            {
                TimeOver();
            }
            else
            {
                UpdateTimerText();
            } 
        }

    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainTime / 60F);
        int seconds = Mathf.FloorToInt(remainTime % 60F);
        int milliseconds = Mathf.FloorToInt((remainTime * 1000F) % 1000) / 10;

        timerText.text = string.Format("{0}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public void ObjectInteract(int index)
    {
        if (!interactionsCompleted[index])
        {
            interactionsCompleted[index] = true;
            interactCount++;

            if (interactCount == interactObject.Length)
            {
                ActiveGoalZone();
            }
        }
    }

    // 2인용 퍼즐에 상호작용했을 때, 두 플레이어 모두 이동을 제한하는 메서드
    public void SetPlayerMovement(bool setcanMove)
    {
        if (player1 != null) 
        {
            player1.canMove = setcanMove;
        }
        if (player2 != null) 
        {
            player2.canMove = setcanMove;
        }
    }

    public void ActiveGoalZone()
    {
        SpriteRenderer sr = goalZone.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Debug.Log("색 바뀜");
            sr.color = goalZoneColor;
        }
    }

    public void TimeOver()
    {
        Debug.Log("타임오버");
        isTimeOver = true;
    }

}
