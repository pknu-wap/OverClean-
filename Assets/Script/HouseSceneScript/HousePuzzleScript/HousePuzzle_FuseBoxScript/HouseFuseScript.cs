using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HouseFuseScript : MonoBehaviourPun
{
    // 영역에 퓨즈가 닿였는지 체크 변수
    public bool isConnect = false;
    // 각 퓨즈 회전 속도(컴포넌트 창에서 설정)
    public float rotationSpeed;

    // 상태를 받아서 그 상태로 전환
    public void GetAndSetState(float targetState)
    {
        // 각도에 맞춰 회전
        transform.rotation = Quaternion.Euler(0, 0, targetState);
    }

    // 퓨즈 돌리기
    public void RotateThisFuse()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    // 영역에 퓨즈가 닿이면
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GoalZone"))
        {
            photonView.RPC("IsConnect", RpcTarget.All, true);
        }
    }
    
    // 퓨즈가 영역에서 나가면
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("GoalZone"))
        {
            photonView.RPC("IsConnect", RpcTarget.All, false);
        }
    }

    [PunRPC]
    void IsConnect(bool state)
    {
        isConnect = state;
    }
}
