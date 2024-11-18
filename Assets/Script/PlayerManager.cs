using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun//, IPunObservable
{
    public Vector2 inputVec;
    public float speed;
    public bool canMove = true;

    private Rigidbody2D rigid;
    private Animator anim;

    public int playerID;
    private string characterName;

    // 제거된 변수
    // private Vector2 networkedPosition;
    // private Vector2 currentVelocity; 
    // private int networkedDirection;
    // private float smoothingDelay = 0.33f; 

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

        if (!photonView.IsMine)
        {
            rigid.isKinematic = true;
        }
    }

    void Update()
    {
        // 원격 플레이어일 경우 위치 및 방향 데이터를 수신하지 않음
        if (!photonView.IsMine) 
        {
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
        if (inputVec.y < 0) return 0; 
        else if (inputVec.x > 0) return 1; 
        else if (inputVec.x < 0) return 2; 
        else if (inputVec.y > 0) return 3; 
        return -1; 
    }

}
