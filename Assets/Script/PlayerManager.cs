using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun, IPunObservable
{
    public Vector2 inputVec;
    public float speed;
    public bool canMove = true;

    private Rigidbody2D rigid;
    private Animator anim;

    public int playerID;
    private string characterName;

    private Vector2 networkedPosition;
    // SmoothDamp에서 속도 추적 변수
    private Vector2 currentVelocity; 
    private int networkedDirection;
    // SmoothDamp 보간 시간
    private float smoothingDelay = 0.33f; 

    void Start()
    {   
        // 커스텀 프로퍼티의 캐릭터 이름값으로 플레이어 id 할당
        if (photonView.IsMine)
        {   
            characterName = PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString();

            if (characterName == "Dave")
            {
                playerID = 1;
            }
            else if (characterName == "Matthew")
            {
                playerID = 2;
            }

            Debug.Log("내 플레이어 ID: " + playerID + ", 캐릭터 이름: " + characterName);
        }

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        networkedPosition = rigid.position;

        if (!photonView.IsMine)
        {
            rigid.isKinematic = true;
        }
    }

    void Update()
    {
        // 원격 플레이어일 경우 보간만 수행
        if (!photonView.IsMine) 
        {
            // SmoothDamp로 부드러운 위치 보간 처리
            rigid.position = Vector2.SmoothDamp(rigid.position, networkedPosition, ref currentVelocity, smoothingDelay);

            // 원격 플레이어의 방향만 받아와서 로컬에서 애니메이션 처리(깜빡임 약간 줄어듬, 완전히 없애진 못함 / 프레임 오류일지도?)
            UpdateAnimationDirection(networkedDirection);
            return;
        }

        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        anim.SetFloat("Speed", inputVec.magnitude);
        UpdateAnimationDirection(CalculateDirection(inputVec));
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine || !canMove) return;

        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void UpdateAnimationDirection(int direction)
    {
        anim.SetInteger("Direction", direction);
    }

    private int CalculateDirection(Vector2 inputVec)
    {
        // 정면
        if (inputVec.y < 0) return 0; 
        // 오른쪽
        else if (inputVec.x > 0) return 1; 
        // 왼쪽
        else if (inputVec.x < 0) return 2; 
        
        else if (inputVec.y > 0) return 3; 
        // 방향 없음
        return -1; 
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어의 데이터를 전송
            stream.SendNext(rigid.position);
            stream.SendNext(CalculateDirection(inputVec));
        }
        else
        {
            // 원격 플레이어의 데이터를 수신
            networkedPosition = (Vector2)stream.ReceiveNext();
            networkedDirection = (int)stream.ReceiveNext();
        }
    }
}
