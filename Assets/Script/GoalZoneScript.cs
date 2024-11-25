using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GoalZone : MonoBehaviour
{
    // 플레이어 1이 구역에 들어왔는지 여부
    private bool player1InZone = false; 
    // 플레이어 2가 구역에 들어왔는지 여부 
    private bool player2InZone = false; 
    // 상호작용 개수를 확인하기 위한 stagemanager 참조
    public StageManager stageManager;    
    // 스테이지 클리어 여부를 stageManager에게 전달하기 위한 변수
    public bool stageClear = false;
    // MapClearPanel UI를 참조할 변수
    public GameObject MapClearPanel;
    // 타이머 텍스트를 표시하기 위한 text ui 참조
    public TMP_Text clearTimeText;

    // 플레이어가 구역에 들어왔을 때 처리
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            player1InZone = true;
            Debug.Log("플레이어 1 구역 도착");
            CheckForClear();
        }
        else if (other.CompareTag("Player2"))
        {
            player2InZone = true;
            Debug.Log("플레이어 2 구역 도착");
            CheckForClear();
        }
    }

    // 플레이어가 구역에서 나갔을 때 처리
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            player1InZone = false;
        }
        else if (other.CompareTag("Player2"))
        {
            player2InZone = false;
        }

    }

    void CheckForClear()
    {
        // 두 플레이어가 구역 안에 들어왔고, 시간 내에 모든 상호작용이 완료됐다면
        if (player1InZone && player2InZone && stageManager.interactCount == stageManager.interactObject.Length && !stageManager.isTimeOver)
        {
            ClearStage();
        }
    }

    void ClearStage()
    {
        // 스테이지 클리어
        stageClear = true;
        Debug.Log("스테이지 클리어");

        // 시간을 정지하기 전에 경과된 시간을 저장
        float finalTime = stageManager.elapsedTime;

        if(MapClearPanel != null)
        {
            MapClearPanel.SetActive(true);
        }

        // 시간 포맷으로 변환하여 텍스트 표시
        if (clearTimeText != null)
        {
            int minutes = Mathf.FloorToInt(finalTime / 60F);
            int seconds = Mathf.FloorToInt(finalTime % 60F);
            int milliseconds = Mathf.FloorToInt((finalTime * 1000F) % 1000) / 10;
            clearTimeText.text = string.Format("{0}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }

        stageManager.isPaused = true;

         // 플레이어 이동 제한
        if (stageManager != null)
        {
            stageManager.SetPlayerMovement(false);
        }

        // SceneManager.LoadScene("MapChooseScene");
    }
}
