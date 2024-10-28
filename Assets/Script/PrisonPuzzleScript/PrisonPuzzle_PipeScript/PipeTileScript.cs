using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeTileScript : MonoBehaviour
{
    // 회전 각도 (0, 90, 180, 270도로 제한)
    private int currentRotation;

    // 스프라이트 렌더러
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentRotation = 0; // 초기 회전 각도
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RotateTile()
    {
        // 90도씩 시계방향 회전
        currentRotation = (currentRotation + 90) % 360;
        // Z축을 기준으로 회전
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    void OnMouseDown()
    {
        RotateTile();
    }

}
