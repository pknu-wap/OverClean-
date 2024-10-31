using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeTileScript : MonoBehaviour
{
    // 회전 각도 (0, 90, 180, 270도로 제한)
    private int currentRotation;
    // currentRotation, pipeShape는 타일이 생성될 때 PrisonPipePuzzleScript.cs에서 정보를 받아 초기화됨
    public int currentRotation;
    public int pipeShape;

    // 스프라이트 렌더러
    private SpriteRenderer spriteRenderer;

    public Camera puzzleCamera;

    void Start()
    {
         // 초기 회전 각도
        currentRotation = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (puzzleCamera == null)
        {
            puzzleCamera = GameObject.FindGameObjectWithTag("PuzzleCamera").GetComponent<Camera>();
        }
    }

    public void RotateTile()
    {
        // 파이프 타일이 기존 회전 각도를 유지한 상태에서 90도 회전
        transform.Rotate(0, 0, -90);
        // 0,1,2,3 으로 회전 정보 구분
        currentRotation = (currentRotation + 1) % 4;
        Debug.Log($"회전함! / pipeshape : {pipeShape} , currentRotation : {currentRotation}");
    }

    // 다른 타일과의 연결을 검사하는 함수 예시
    public bool IsConnected(PipeTileScript otherTile)
    {
        // 구현할 내용: otherTile과의 방향 및 연결 상태 검사 로직
        // 연결되어 있다고 가정
        return true;
    }

     private void OnMouseDownHandler()
    {
        // 마우스 위치를 월드 좌표로 변환 (2D 평면에서의 위치만 사용)
        Vector3 mousePosition = puzzleCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -puzzleCamera.transform.position.z));
        // 2D 평면에서의 위치만 사용
        mousePosition.z = 0f;

        // 현재 오브젝트에 마우스 클릭이 감지되었는지 확인
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            RotateTile(); 
            Debug.Log("회전함!");
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDownHandler();
        }
    }
}
