using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseFuseScript : MonoBehaviour
{
    // 퓨즈 랜덤 초기 각도
    private float targetAngle;
    // 영역에 퓨즈가 닿였는지 체크 변수
    public bool isConnect = false;
    // 각 퓨즈 회전 속도(컴포넌트 창에서 설정)
    public float rotationSpeed;
    void Start()
    {
        // 초기 회전 각도 랜덤하게 설정
        targetAngle = Random.Range(0f, 360f);
        // 각도에 맞춰 회전
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
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
            isConnect = true;
        }
    }

    // 퓨즈가 영역에서 나가면
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("GoalZone"))
        {
            isConnect = false;
        }
    }
}
