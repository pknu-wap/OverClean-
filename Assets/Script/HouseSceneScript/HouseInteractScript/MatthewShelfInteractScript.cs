using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Unity.VisualScripting;

public class MatthewShelfInteractScript : MonoBehaviour
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
    public float interactionDistance = 1.3f;
    // 상호작용 여부
    public bool hasInteracted = false;
    // 선반을 참조하기 위한 변수
    public GameObject houseShelf;
    // 선반을 참조해서 material을 조정하기 위한 spriterenderer 변수
    public SpriteRenderer sr;
    // 퍼즐이 열려있는지 확인하기 위한 변수
    private bool isPuzzleOpen = false;
    // 상자가 옮겨져있는지 확인하기 위한 변수
    public bool isBoxArrived = false;
    
    // 상호작용시 비활성화 되어있는 캔버스를 열기 위한 변수
    public RectTransform PuzzleUI;
    // 현재 클라이언트가 매튜인지(디폴트 false)
    public bool matthewIsMe = false;

    // 맵에서 위에 있던 종이/먼지들
    public GameObject dust1;
    public GameObject dust2;

    void Start()
    {
        AddLocalPlayer();
        // sr을 getcomponent 메서드로 초기화
        sr = GetComponent<SpriteRenderer>();
    }

    void AddLocalPlayer()
    {
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (var photonView in photonViews)
        {
            if (photonView.gameObject.tag == "Player2")
            {
                playerLocation = photonView.transform;
            }
        }
    }

    void Update()
    {
        if(playerLocation == null)
        {
            AddLocalPlayer();
            return;
        }
        // 플레이어와 오브젝트 간 거리 계산
        float distanceToPlayer = Vector2.Distance(transform.position, playerLocation.position);

        // 테두리 생성
        ShowHighlight();

        // 상호작용 가능한 거리 안에 있고 상호작용하지 않았다면, 그리고 상자가 도착해 있다면
        if (distanceToPlayer <= interactionDistance && !hasInteracted && isBoxArrived)
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
        if (isPuzzleOpen)
        {
            if (PuzzleManager.instance.isPuzzleSuccess)
            {
                // 퍼즐이 닫힘
                isPuzzleOpen = false;
                // 퍼즐매니저의 퍼즐 성공여부를 초기화
                PuzzleManager.instance.isPuzzleSuccess = false;
                playerLocation.GetComponent<PlayerManager>().canMove = true;
                PhotonView photonView = GetComponent<PhotonView>();
                // RPC 함수 호출
                photonView.RPC("ShelfInteractRPC", RpcTarget.All);
            }
            else if (PuzzleManager.instance.clickPuzzleCloseButton)
            {
                isPuzzleOpen = false;

                PuzzleManager.instance.clickPuzzleCloseButton = false;

                playerLocation.GetComponent<PlayerManager>().canMove = true;
            }
        }
    }

    // 상호작용 함수
    void Interact()
    {
        // 퍼즐이 열려 있지 않을 때만 Interact가 실행되었을 때 퍼즐씬이 불러와지도록 조건 추가
        if (!isPuzzleOpen && !PauseManager.Instance.isPaused)
        {
            PuzzleUI.gameObject.SetActive(true);
            // 씬매니저로 퍼즐씬 불러오기
            SceneManager.LoadScene("HouseShelfPuzzleScene", LoadSceneMode.Additive);
            // 퍼즐 오픈 변수 true
            isPuzzleOpen = true;
            // Player.cs의 canMove를 제어해 플레이어 이동 제한
            playerLocation.GetComponent<PlayerManager>().canMove = false;
        }
    }

    [PunRPC]
    void ShelfInteractRPC()
    {
        Destroy(dust1);
        Destroy(dust2);
        // 상호작용 완료됨
        hasInteracted = true;
        // 해당 오브젝트 인덱스 상호작용 완료를 stageManager에게 전달
        stageManager.ObjectInteract(objectIndex);
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
