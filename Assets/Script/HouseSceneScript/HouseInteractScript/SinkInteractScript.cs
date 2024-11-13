using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SinkInteractScript : MonoBehaviour
{
    // 테두리 없는 상태
    public Material normalState;
    // 테두리 있는 상태
    public Material canInteractState;
    // 오브젝트의 인덱스(감옥 맵에서 0~7)
    public int objectIndex;
    // stagemanager를 참조해서 상호작용 여부를 제어하기 위한 변수
    public StageManager stageManager;
    // 플레이어를 참조해서 위치를 받아오기 위한 변수
    public Transform playerLocation;
    // 상호작용 거리
    public float interactionDistance = 1.0f;
    // 상호작용 여부
    public bool hasInteracted = false;
    // 싱크대를 참조해서 material을 조정하기 위한 spriterenderer 변수
    public SpriteRenderer sr;
    // 퍼즐이 열려있는지 확인하기 위한 변수
    private bool isPuzzleOpen = false;
    
    // 상호작용시 비활성화 되어있는 캔버스를 열기 위한 변수
    public RectTransform PuzzleUI;

    void Start()
    {
        // sr을 getcomponent 메서드로 초기화
        sr = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        // 플레이어와 오브젝트 간 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, playerLocation.position);

        // 테두리 생성
        ShowHighlight();

        // 상호작용 가능한 거리 안에 있고 상호작용하지 않았다면
        if (distanceToPlayer <= interactionDistance && !hasInteracted)
        {
            // 스페이스바로 상호작용
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Interact();
            }
        }
        else
        {
            // 테두리 삭제
            HideHighlight();
        }
        // 퍼즐이 열려 있을 때 퍼즐을 해결하면 상호작용 성공
        if (isPuzzleOpen && PuzzleManager.instance.isPuzzleSuccess)
        {
            // 오브젝트 상호작용됨
            hasInteracted = true;
            // 퍼즐이 닫힘
            isPuzzleOpen = false;
            // statemanager에게 상호작용되었다고 알림
            stageManager.ObjectInteract(objectIndex);
            // 퍼즐매니저의 퍼즐 성공여부를 초기화
            PuzzleManager.instance.isPuzzleSuccess = false;
            // 퍼즐이 성공했으므로 플레이어 이동 가능하게 설정
            playerLocation.GetComponent<Player>().canMove = true;
        }
    }

    // 상호작용 함수
    void Interact()
    {
        // 퍼즐이 열려 있지 않을 때만 Interact가 실행되었을 때 퍼즐씬이 불러와지도록 조건 추가
        if (!isPuzzleOpen)
        {
            PuzzleUI.gameObject.SetActive(true);
            // 씬매니저로 퍼즐씬 불러오기
            SceneManager.LoadScene("HouseSinkPuzzleScene", LoadSceneMode.Additive);
            // 퍼즐 오픈 변수 true
            isPuzzleOpen = true;
            // Player.cs의 canMove를 제어해 플레이어 이동 제한
            playerLocation.GetComponent<Player>().canMove = false;
       }

    }

    // 테두리 생성 및 표시
    void ShowHighlight()
    {
        // 테두리가 있는 material로 변경
        sr.material = canInteractState;
    }

    // 테두리 숨김
    void HideHighlight()
    {
        // 테두리가 없는 material로 변경
        sr.material = normalState;
    }
}
