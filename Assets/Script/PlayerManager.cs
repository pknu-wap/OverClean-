using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun
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

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // 로컬 플레이어가 아니라면, 스크립트 비활성화
        if (!photonView.IsMine)
        {
            enabled = false;
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

        if (playerID == 1)
        {
            anim.SetFloat("Speed", inputVec1.magnitude);
            UpdateAnimationDirection(inputVec1);
        }
        else if (playerID == 2)
        {
            anim.SetFloat("Speed", inputVec2.magnitude);
            UpdateAnimationDirection(inputVec2);
        }
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
}
