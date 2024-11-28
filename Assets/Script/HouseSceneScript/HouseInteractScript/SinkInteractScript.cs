using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

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
    // 여러 플레이어 위치를 저장할 리스트
    public List<Transform> playerLocations = new List<Transform>();
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
        AddLocalPlayer();
        // sr을 getcomponent 메서드로 초기화
        sr = GetComponent<SpriteRenderer>();
    }
    // 태그를 통해 로컬 플레이어(상호작용은 각각의 클라이언트 관점에서 자신의 캐릭터로만 할 수 있으므로) 할당
    void AddLocalPlayer()
    {
        // 모든 PhotonView 객체 중 로컬 플레이어 소유 프리팹만 필터링
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (var photonView in photonViews)
        {
            // PhotonNetwork.Instantiate로 생성된 로컬 플레이어만 리스트에 추가
            if (photonView.IsMine && photonView.gameObject.name.Contains("Player") && !playerLocations.Contains(photonView.transform))
            {
                playerLocations.Add(photonView.transform);
            }
        }
    }
    void Update()
    {
        bool canInteract = false;
        // 모든 플레이어의 위치와 오브젝트 간 거리 계산
        foreach (var playerLocation in playerLocations)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerLocation.position);

            // 상호작용 가능 거리 내에 있는 플레이어가 있는지 확인
            if (distanceToPlayer <= interactionDistance && !hasInteracted)
            {
                canInteract = true;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // 특정 플레이어와 상호작용
                    Interact(playerLocation);
                    break;
                }
            }
        }

        if (canInteract)
        {
            ShowHighlight();
        }
        else
        {
            HideHighlight();
        }
        if (isPuzzleOpen)
        {
            if (PuzzleManager.instance.isPuzzleSuccess)
            {
                isPuzzleOpen = false;

                PuzzleManager.instance.isPuzzleSuccess = false;

                foreach (var playerLocation in playerLocations)
                {
                    playerLocation.GetComponent<PlayerManager>().canMove = true;
                }
                // 모든 클라이언트에서 DoorInteractRPC을 시작
                PhotonView photonView = GetComponent<PhotonView>();
                // RPC 함수 호출
                photonView.RPC("SinkInteractRPC", RpcTarget.All);
            }
            else if(PuzzleManager.instance.clickPuzzleCloseButton)
            {
                isPuzzleOpen = false;

                PuzzleManager.instance.clickPuzzleCloseButton = false;

                foreach (var playerLocation in playerLocations)
                {
                    playerLocation.GetComponent<PlayerManager>().canMove = true;
                }
            }
        }
    }

    // 상호작용 함수
    void Interact(Transform playerLocation)
    {
        // 퍼즐이 열려 있지 않을 때만 Interact가 실행되었을 때 퍼즐씬이 불러와지도록 조건 추가
        if (!isPuzzleOpen && !PauseManager.Instance.isPaused)
        {
            PuzzleUI.gameObject.SetActive(true);
            // 씬매니저로 퍼즐씬 불러오기
            SceneManager.LoadScene("HouseSinkPuzzleScene", LoadSceneMode.Additive);
            // 퍼즐 오픈 변수 true
            isPuzzleOpen = true;
            // Player.cs의 canMove를 제어해 플레이어 이동 제한
            playerLocation.GetComponent<PlayerManager>().canMove = false;
       }

    }

    [PunRPC]
    void SinkInteractRPC()
    {
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
