using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위한 네임스페이스
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public bool canMove = true;

    private Rigidbody2D rigid;
    private Animator anim;
    
    public int playerID;
    private string characterName;

    // 현재 씬에서 달리기 기능을 활성화할지 여부를 제어하는 변수
    private bool allowRun = true;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
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

        // 현재 씬 이름을 확인하고 속도와 달리기 기능 설정
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "PrisonScene")
        {
            speed = 3;
            allowRun = false; // 달리기 기능 비활성화
        }
        else if (currentScene == "HouseScene")
        {
            speed = 2;
            allowRun = true; // 달리기 기능 활성화
        }
        AudioListener audioListener = GetComponentInChildren<AudioListener>();

        if (photonView != null && !photonView.IsMine)
        {
            // 원격 플레이어의 Audio Listener를 비활성화
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
        }
    }

    void Update()
    {
        // 원격 플레이어일 경우 위치 및 방향 데이터를 수신하지 않음
        if(!photonView.IsMine)
        {
            return;
        }
        // 움직일 수 없는 상태일 때, 애니메이션 속도 0으로 초기화
        if(!canMove)
        {
            inputVec = Vector2.zero;
            anim.SetFloat("Speed", 0);
            return;
        }

        inputVec.x = Input.GetAxisRaw("Player1HorizontalKey");
        inputVec.y = Input.GetAxisRaw("Player1VerticalKey");

        // 달리기 기능이 허용된 경우만 처리
        if (allowRun)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed = 4; // 달리기 속도
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speed = 2; // 기본 속도로 복구
            }
        }
        UpdateAnimationDirection(CalculateDirection(inputVec));
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        // 움직일 수 없는 상태일 때, 이동속도를 0으로 초기화
        if (!canMove)
        {
            rigid.velocity = Vector2.zero;
            return;
        }

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
