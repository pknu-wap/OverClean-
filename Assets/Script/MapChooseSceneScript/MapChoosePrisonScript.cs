using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapChoosePrisonScript : MonoBehaviour
{
    // 플레이어가 들어왔는지 확인하기 위한 변수
    public bool isPlayer1In = false;
    public bool isPlayer2In = false;

    // material을 조정하기 위한 spriterenderer 변수
    public SpriteRenderer sr;

    // 테두리 없는 상태
    public Material normalState;
    // 테두리 있는 상태
    public Material canInteractState;

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
    }

    void Start()
    {
        // sr을 getcomponent 메서드로 초기화
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // 상호작용 존 안에 두 플레이어 모두가 있다면
        if (isPlayer1In && isPlayer2In)
        {
            // 테두리 생성
            ShowHighlight(); 
            // 스페이스바로 상호작용
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                SceneManager.LoadScene("PrisonScene");
            }
        }
        else
        {
            HideHighlight();
        }
    }

    // 테두리 생성 및 표시
    void ShowHighlight()
    {
        // 테두리가 있는 material로 변경
        sr.material = canInteractState;
    }

    // 테두리 숨기기
    void HideHighlight()
    {
        sr.material = normalState;
    }
}
