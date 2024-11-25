using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseFuseButtonScript : MonoBehaviour
{
    // 퍼즐 카메라 참조
    public Camera puzzleCamera;
    // 해당 버튼에 할당된 퓨즈(2개,리스트)
    public List<GameObject> fuseList = new List<GameObject>();
    // 퓨즈의 스크립트를 참조하는 리스트
    public List<HouseFuseScript> scriptList = new List<HouseFuseScript>();
    // 버튼이 클릭 중인지 변수
    private bool isOnClick = false;
    // Start is called before the first frame update
    void Start()
    {
        // 퍼즐 카메라가 할당되지 않은 경우, 태그를 사용하여 퍼즐 카메라를 찾음
        if (puzzleCamera == null)
        {
            puzzleCamera = GameObject.FindGameObjectWithTag("PuzzleCamera").GetComponent<Camera>();
        }
        // 스크립트 각 퓨즈에서 참조
        for(int i = 0; i < 2; i++)
        {
            scriptList[i] = fuseList[i].GetComponent<HouseFuseScript>();
        }
    }

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
            isOnClick = false;
        }
        if(isOnClick)
        {
            for(int i = 0; i < 2; i++)
            {
                scriptList[i].RotateThisFuse();
            }
        }
    }

    // 마우스 클릭 시 호출되는 함수
    private void OnMouseDownHandler()
    {
        // 마우스 위치를 월드 좌표로 변환 (2D 평면에서의 위치만 사용)
        Vector3 mousePosition = puzzleCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -puzzleCamera.transform.position.z));
        mousePosition.z = 0f; // 2D 평면에서의 위치만 사용
        // 현재 오브젝트에 마우스 클릭이 감지되었는지 확인
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            isOnClick = true;
        }
    }
}
