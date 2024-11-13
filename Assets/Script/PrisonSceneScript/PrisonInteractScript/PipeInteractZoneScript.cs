using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class PipeInteractZone : MonoBehaviour
{
    // 플레이어가 들어왔는지 확인하기 위한 변수
    public bool isPlayer1In = false;
    public bool isPlayer2In = false;
    // 현재 상호작용 존 안의 총 플레이어 수 변수
    private int zonePlayerCounter = 0;
    // 시각적으로 플레이어 수를 보여줄 게임오브젝트 변수
    public GameObject pipeInteractZonePlayerCounter1;
    public GameObject pipeInteractZonePlayerCounter2;

    // 플레이어가 구역에 들어온다면 true, 나가면 false (태그로 구분)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            isPlayer1In = true;
            
        }
        else if (other.CompareTag("Player2"))
        {
            isPlayer2In = true;
        }
        // 어떤 플레이어든 상호작용 존에 들어온다면 플레이어 카운터 변수 +1
        zonePlayerCounter++;
        // 플레이어 수에 따라 불빛 개수 갱신
        UpdatePlayerCounter();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            isPlayer1In = false;
            
        }
        else if (other.CompareTag("Player2"))
        {
            isPlayer2In = false;
        }
        // 어떤 플레이어든 상호작용 존에서 나간다면 플레이어 카운터 변수 -1
        zonePlayerCounter--;
        // 플레이어 수에 따라 불빛 개수 갱신
        UpdatePlayerCounter();
    }

    // 현재 상호작용 존 안의 플레이어 수에 따라 불빛 개수를 갱신하는 함수
    void UpdatePlayerCounter()
    {
        // 최대 플레이어는 2명, 가능한 경우의 수는 0,1,2이므로 if-else로 작성
        if(zonePlayerCounter == 0)
        {
            pipeInteractZonePlayerCounter1.GetComponent<SpriteRenderer>().color = new Color(0.152f, 0.075f, 0.329f);
            pipeInteractZonePlayerCounter2.GetComponent<SpriteRenderer>().color = new Color(0.152f, 0.075f, 0.329f);
        }
        else if(zonePlayerCounter == 1)
        {
            pipeInteractZonePlayerCounter1.GetComponent<SpriteRenderer>().color = Color.white;
            pipeInteractZonePlayerCounter2.GetComponent<SpriteRenderer>().color = new Color(0.152f, 0.075f, 0.329f);
        }
        else if(zonePlayerCounter == 2)
        {
            pipeInteractZonePlayerCounter1.GetComponent<SpriteRenderer>().color = Color.white;
            pipeInteractZonePlayerCounter2.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
