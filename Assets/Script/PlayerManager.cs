using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위한 네임스페이스

public class PlayerManager : MonoBehaviour
{
    public Vector2 inputVec1;
    public Vector2 inputVec2;
    public float speed;
    public bool canMove = true;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    public int playerID;

    // 현재 씬에서 달리기 기능을 활성화할지 여부를 제어하는 변수
    private bool allowRun = true;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
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
    }

    void Update()
    {
        if (playerID == 1)
        {
            inputVec1.x = Input.GetAxisRaw("Player1HorizontalKey");
            inputVec1.y = Input.GetAxisRaw("Player1VerticalKey");

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
        }
        else if (playerID == 2)
        {
            inputVec2.x = Input.GetAxisRaw("Player2HorizontalKey");
            inputVec2.y = Input.GetAxisRaw("Player2VerticalKey");

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
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;

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
        if (playerID == 1)
        {
            anim.SetFloat("Speed", inputVec1.magnitude);
            UpdateDirection(inputVec1);
        }
        else if (playerID == 2)
        {
            anim.SetFloat("Speed", inputVec2.magnitude);
            UpdateDirection(inputVec2);
        }
    }

    // 애니메이션 방향 업데이트 함수
    private void UpdateDirection(Vector2 inputVec)
    {
        if (inputVec.y < 0)
            anim.SetInteger("Direction", 0); // 정면
        else if (inputVec.x > 0)
            anim.SetInteger("Direction", 1); // 오른쪽
        else if (inputVec.x < 0)
            anim.SetInteger("Direction", 2); // 왼쪽
        else if (inputVec.y > 0)
            anim.SetInteger("Direction", 3); // 뒤쪽
    }
}
