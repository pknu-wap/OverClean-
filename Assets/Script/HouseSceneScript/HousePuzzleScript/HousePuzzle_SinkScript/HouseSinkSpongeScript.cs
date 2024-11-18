using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSinkSpongeScript : MonoBehaviour
{
    // 퍼즐 카메라 참조
    public Camera puzzleCamera;
    // 객체가 드래그 중인지 여부를 나타내는 플래그
    public bool isDragging = false;
    // 드래그 시 피봇 오프셋
    private Vector3 pivotOffset;
    // Start is called before the first frame update
    void Start()
    {
        // 퍼즐 카메라가 할당되지 않은 경우, 태그를 사용하여 퍼즐 카메라를 찾음
        if (puzzleCamera == null)
        {
            puzzleCamera = GameObject.FindGameObjectWithTag("PuzzleCamera").GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 왼쪽 버튼이 눌렸는지 확인
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDownHandler();
        }

        // 마우스 왼쪽 버튼이 떼어졌는지 확인
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // 객체가 드래그 중인 경우 처리
        if (isDragging)
        {
            DragObject();
        }
    }

    // 마우스 클릭 시 호출되는 함수
    private void OnMouseDownHandler()
    {
        // 마우스 위치를 월드 좌표로 변환 (2D 평면에서의 위치만 사용)
        Vector3 mousePosition = puzzleCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -puzzleCamera.transform.position.z));
        mousePosition.z = 0f; // 2D 평면에서의 위치만 사용
        Debug.Log("클릭 됨(" + mousePosition.x + "," + mousePosition.y + ")");
        // 현재 오브젝트에 마우스 클릭이 감지되었는지 확인
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        // Ray가 오브젝트에 닿았을 때
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            Debug.Log("스펀지 감지");
            // 오브젝트의 중심점과 마우스 포지션의 상대적 위치를 계산해 pivot을 설정, 클릭 위치가 유지되도록 함
            pivotOffset = transform.position - mousePosition;
            isDragging = true;
        }
    }

    // 객체를 드래그하는 동안 호출되는 함수
    private void DragObject()
    {
        // 마우스 위치를 월드 좌표로 변환 (2D 평면에서의 위치만 사용)
        Vector3 mousePosition = puzzleCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -puzzleCamera.transform.position.z));
        // 2D 평면에서의 위치만 사용
        mousePosition.z = 0f; 

        // 객체 위치를 마우스 위치와 피봇 오프셋을 더한 값으로 설정
        transform.position = mousePosition + pivotOffset;
    }
}