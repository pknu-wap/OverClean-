using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Photon.Pun;

public class DaveBoxInteractScript : MonoBehaviourPun
{
    // 테두리 없는 상태
    public Material normalState;
    // 테두리 있는 상태
    public Material canInteractState;
    // stagemanager를 참조해서 상호작용 여부를 제어하기 위한 변수
    public StageManager stageManager;
    // 플레이어 위치를 저장할 변수
    public Transform playerLocation;
    // 들 수 있는 거리
    public float canHoldDistance = 2.0f;
    // 들고 있는지 여부
    public bool isHolding = false;
    // 박스를 참조해서 material을 조정하기 위한 spriterenderer 변수
    public SpriteRenderer sr;
    // 박스를 옮길 장소를 참조히기 위한 변수
    public GameObject boxDestination;
    // 선반을 참조하기 위한 변수
    public GameObject matthewShelf;
    // 목적지에 플레이어가 들어왔는지를 체크하는 변수
    public bool isArrive = false;
    // 박스를 놓았는지 확인하는 변수
    public bool boxInteractEnd = false;

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
            if (photonView.gameObject.tag == "Player1")
            {
                playerLocation = photonView.transform;
            }
        }
    }

    void Update()
    {
        if (playerLocation == null)
        {
            AddLocalPlayer();
            return;
        }
        if (boxInteractEnd)
        {
            return;
        }
        // 플레이어와 오브젝트 간 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, playerLocation.position);

        // 테두리 생성
        ShowHighlight();

        //  들 수 있는 거리 안에 있고 아직 들지 않았다면
        if (distanceToPlayer <= canHoldDistance && !isHolding)
        {
            // 스페이스바로 들기
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PhotonView photonView = GetComponent<PhotonView>();
                photonView.RPC("HoldingStartRPC", RpcTarget.All);
                // 투명도 조정
                SpriteRenderer spriteRenderer = boxDestination.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    Color color = spriteRenderer.color;
                    // a값(투명도) 조절
                    color.a = 0.5f;
                    // 변경된 색상 다시 할당
                    spriteRenderer.color = color;
                }
            }

        }
        else
        {
            // 테두리 삭제
            HideHighlight();
        }

        // 들고있는 동안
        if (isHolding)
        {
            Holding();
        }

        // 들고 있는채로 도착지점에 들어왔다면
        if (isHolding && isArrive)
        {
            // 스페이스바로 놓기
            if (Input.GetKeyDown(KeyCode.Space))
            {
                photonView.RPC("HoldingEndRPC", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void HoldingStartRPC()
    {
        isHolding = true;
    }

    // 상자 위치를 옮겨 든 것처럼 보이게 하는 함수
    void Holding()
    {
        transform.position = new Vector3(playerLocation.position.x, playerLocation.position.y + 1.04f, playerLocation.position.z);
    }

    [PunRPC]
    void HoldingEndRPC()
    {
        isHolding = false;
        // 목적지의 x,y 좌표 가져와서 박스 두기
        transform.position = new Vector3(boxDestination.GetComponent<Transform>().position.x, boxDestination.GetComponent<Transform>().position.y + 0.3f, 0.9f);
        boxInteractEnd = true;
        // 선반에 박스 도착했다고 알리기
        matthewShelf.GetComponent<MatthewShelfInteractScript>().isBoxArrived = true;
        // 도착지 상태 갱신
        boxDestination.GetComponent<BoxDestinationZoneScript>().sr.material = normalState;
        // 투명도 조정
        SpriteRenderer spriteRenderer = boxDestination.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            // a값(투명도) 재조절
            color.a = 0;
            // 변경된 색상 다시 할당
            spriteRenderer.color = color;
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
