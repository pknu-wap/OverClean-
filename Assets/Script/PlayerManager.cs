using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun, IPunObservable
{
    public Vector2 inputVec1;
    public Vector2 inputVec2;
    public float speed;
    public bool canMove = true;

    private Rigidbody2D rigid;
    private SpriteRenderer spriter;
    private Animator anim;

    // 플레이어 ID (1번 또는 2번 플레이어)
    public int playerID;
    private PhotonView photonView;

    // 네트워크 동기화용 변수
    private Vector2 networkedPosition;
    private float networkedSpeed;
    private int networkedDirection;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // 초기 위치 설정
        networkedPosition = rigid.position;
        
        // 로컬 플레이어가 아니라면, 물리 상호작용 비활성화
        if (!photonView.IsMine)
        {
            rigid.isKinematic = true;
        }
    }

    void Update()
    {
        // 로컬 플레이어가 아닌 경우 입력 처리하지 않음
        if (!photonView.IsMine) return;

        if (playerID == 1)
        {
            inputVec1.x = Input.GetAxisRaw("Player1HorizontalKey");
            inputVec1.y = Input.GetAxisRaw("Player1VerticalKey");
        }
        else if (playerID == 2)
        {
            inputVec2.x = Input.GetAxisRaw("Player2HorizontalKey");
            inputVec2.y = Input.GetAxisRaw("Player2VerticalKey");
        }
    }

    void FixedUpdate()
    {
        // 로컬 플레이어가 아닌 경우 움직임 처리하지 않음
        if (!photonView.IsMine || !canMove) return;

        Vector2 nextVec = Vector2.zero;
        if (playerID == 1)
        {
            nextVec = inputVec1.normalized * speed * Time.fixedDeltaTime;
        }
        else if (playerID == 2)
        {
            nextVec = inputVec2.normalized * speed * Time.fixedDeltaTime;
        }
        
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        // 로컬 플레이어가 아닌 경우 애니메이션 처리하지 않음
        if (!photonView.IsMine) return;

        Vector2 currentInputVec = (playerID == 1) ? inputVec1 : inputVec2;
        anim.SetFloat("Speed", currentInputVec.magnitude);
        UpdateAnimationDirection(currentInputVec);
    }

    private void UpdateAnimationDirection(Vector2 inputVec)
    {
        if (inputVec.y < 0)
        {
            anim.SetInteger("Direction", 0); // 정면
        }
        else if (inputVec.x > 0)
        {
            anim.SetInteger("Direction", 1); // 오른쪽
        }
        else if (inputVec.x < 0)
        {
            anim.SetInteger("Direction", 2); // 왼쪽
        }
        else if (inputVec.y > 0)
        {
            anim.SetInteger("Direction", 3); // 뒤쪽
        }
    }

    // IPunObservable 인터페이스 구현을 통한 네트워크 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어의 데이터를 전송
            stream.SendNext(rigid.position);
            stream.SendNext(anim.GetFloat("Speed"));
            stream.SendNext(anim.GetInteger("Direction"));
        }
        else
        {
            // 원격 플레이어의 데이터를 수신
            networkedPosition = (Vector2)stream.ReceiveNext();
            networkedSpeed = (float)stream.ReceiveNext();
            networkedDirection = (int)stream.ReceiveNext();

            // 수신한 값을 애니메이션에 반영
            anim.SetFloat("Speed", networkedSpeed);
            anim.SetInteger("Direction", networkedDirection);

            // 원격 플레이어의 위치를 보간 이동
            rigid.position = Vector2.Lerp(rigid.position, networkedPosition, Time.deltaTime * 10);
        }
    }
}
