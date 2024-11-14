using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class BoxDestinationZoneScript : MonoBehaviour
{
    // 테두리 없는 상태
    public Material normalState;
    // 테두리 있는 상태
    public Material canInteractState;
    // material을 조정하기 위한 spriterenderer 변수
    public SpriteRenderer sr;
    // 박스 참조 변수
    public GameObject daveBox;

    void Start()
    {
        // sr을 getcomponent 메서드로 초기화
        sr = GetComponent<SpriteRenderer>();
    }

    // 존에 데이브가 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1") && daveBox.GetComponent<DaveBoxInteractScript>().isHolding)
        {
            // 도착 변수 true로 변환
            daveBox.GetComponent<DaveBoxInteractScript>().isArrive = true;
            // 테두리가 있는 material로 변경
            sr.material = canInteractState;
        }
    }

    // 나갔을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player1") && daveBox.GetComponent<DaveBoxInteractScript>().isHolding)
        {
            // 도착 변수 false로 변환
            daveBox.GetComponent<DaveBoxInteractScript>().isArrive = false;
            // 테두리가 없는 material로 변경
            sr.material = normalState;
        }
    }
}
