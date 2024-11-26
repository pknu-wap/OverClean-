using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class FuseBoxInteractScript : MonoBehaviourPun
{
    // 테두리 없는 상태
    public Material normalState;
    // 테두리 있는 상태
    public Material canInteractState;
    // 오브젝트의 인덱스(감옥 맵에서 0~7)
    public int objectIndex;
    // stagemanager를 참조해서 상호작용 여부를 제어하기 위한 변수
    public StageManager stageManager;
    // 상호작용 구역을 참조하기 위한 변수
    public FuseInteractZone fuseInteractZone;
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

        // 상호작용 존 안에 두 플레이어 모두가 있고 상호작용하지 않았다면
        if (fuseInteractZone != null && fuseInteractZone.isPlayer1In && fuseInteractZone.isPlayer2In && !hasInteracted)
        {
            // 테두리 생성
            ShowHighlight();
            // 스페이스바로 상호작용
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 모든 플레이어가 씬 로드 시작
                photonView.RPC("LoadFusePuzzleScene", RpcTarget.AllBuffered);
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
                photonView.RPC("FuseClearRPC", RpcTarget.All);
            }
            else if(PuzzleManager.instance.clickPuzzleCloseButton)
            {
                photonView.RPC("FuseCloseRPC", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void LoadFusePuzzleScene()
    {
        if (!isPuzzleOpen)
        {
            PuzzleUI.gameObject.SetActive(true);
            // Additive로 씬 로드
            SceneManager.LoadScene("HouseFuseBoxPuzzleScene", LoadSceneMode.Additive);
            isPuzzleOpen = true;
            stageManager.SetPlayerMovement(false);
        }
    }

    [PunRPC]
    void FuseClearRPC()
    {
        isPuzzleOpen = false;
        // 오브젝트 상호작용됨
        hasInteracted = true;
        // statemanager에게 상호작용되었다고 알림
        stageManager.ObjectInteract(objectIndex);
        // 퍼즐매니저의 퍼즐 성공여부를 초기화
        PuzzleManager.instance.isPuzzleSuccess = false;
        // 퍼즐이 성공했으므로 플레이어 이동 가능하게 설정
        stageManager.SetPlayerMovement(true);
    }

    [PunRPC]
    void FuseCloseRPC()
    {
        isPuzzleOpen = false;
        PuzzleManager.instance.clickPuzzleCloseButton = false;
        stageManager.SetPlayerMovement(true);
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
