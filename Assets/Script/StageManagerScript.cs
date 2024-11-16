using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using TMPro;

public class StageManager : MonoBehaviour
{
    // 오브젝트 관련 변수
    // 상호작용될 오브젝트들 참조(개수 확인용)
    public GameObject[] interactObject; 
    // 최종 도달지점 오브젝트(goalZone 속성에 직접 접근을 위해 GoalZone 타입으로 변경)
    public GoalZone goalZone;
    // 클리어 조건이 완수되면 표시될 색
    public Color goalZoneColor;
    // 각 오브젝트의 상호작용 상태
    public bool[] interactionsCompleted;
    // 상호작용 완료된 오브젝트 개수
    public int interactCount = 0;
    // 플레이어를 관리하는 변수 추가
    public PlayerManager player1;
    public PlayerManager player2;

    // 타이머 관련 변수
    // 주어진 시간 - Inspector에서 설정 가능
    public float limitTime;
    // 남은 시간
    public float remainTime;
    // 타임오버 여부
    public bool isTimeOver = false;
    // 타이머 텍스트를 표시하기 위한 text ui 참조
    public TMP_Text timerText;

    // Slider UI 컴포넌트를 연결할 변수 
    public Slider timeSlider;

    void Start()
    { 
        // 상호작용 오브젝트 개수만큼 bool 배열 정의
        interactionsCompleted = new bool[interactObject.Length];
        // 남은 시간을 주어신 시간으로 세팅
        remainTime = limitTime;

        // 슬라이드 최대값 설정
        if(timeSlider != null)
        {
            // 최댓값은 제한시간
            timeSlider.maxValue = limitTime;

            // 초기값은 제한시간과 동일
            timeSlider.value = limitTime; 
        }
    }

    void Update()
    {
        // 게임오버(시간초과)되지 않았고 스테이지가 클리어되지 않았다면
        if(!isTimeOver && !goalZone.stageClear)
        {
            // 시간을 점차적으로 감소
            remainTime -= Time.deltaTime;

            // 슬라이더 업데이트
            if(timeSlider != null)
            {
                // 남은 시간을 슬라이더에 반영
                timeSlider.value = remainTime; 
            }
            // 남은 시간이 0 이하로 떨어지면
            if(remainTime <= 0)
            {
                // 타임오버 처리
                TimeOver();
            }
            else
            {
                // 남은 시간이 0보다 크다면 타이머 업데이트
                UpdateTimerText();
            } 
        }
    }

    void UpdateTimerText()
    {
        // 분단위 텍스트 수 계산
        int minutes = Mathf.FloorToInt(remainTime / 60F);
        // 초단위 텍스트 수 계산
        int seconds = Mathf.FloorToInt(remainTime % 60F);
        // 밀리초 단위 텍스트 수 계산
        int milliseconds = Mathf.FloorToInt((remainTime * 1000F) % 1000) / 10;

        // 분(1자리):초(2자리):밀리초(2자리) 형식으로 표시
        timerText.text = string.Format("{0}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public void ObjectInteract(int index)
    {
        // 만약 상호작용하지 않았는데 함수가 실행된다면
        if(!interactionsCompleted[index])
        {
            // 해당 인덱스 오브젝트 상호작용 완료
            interactionsCompleted[index] = true;
            // 카운트 증가
            interactCount++;
            // 상호작용 수 == 오브젝트 수라면
            if(interactCount == interactObject.Length)
            {
                // 도착지점 활성화
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
        if(sr != null)
        {
            Debug.Log("색 바뀜");
            // 도착지점의 색을 바꿔서 활성화된 것을 보임
            sr.color = goalZoneColor;
        }
    }

    public void TimeOver()
    {
        // 타임오버 로직 추가
        Debug.Log("타임오버");
        isTimeOver = true;
    }
}
