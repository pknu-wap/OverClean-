using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HouseWindowPuzzleScript : MonoBehaviour
{
    // 기준 빨강키
    public GameObject pivotKey;
    // 돌아가는 회색키
    public GameObject rotateKey;
    // 퍼즐 카메라 참조
    public Camera puzzleCamera;
    // 초당 회전 속도 (60도/초)
    public float rotationSpeed = 60f; 
    // 오차 범위 (5도)
    public float toleranceAngle = 5f; 
    // pivotkey의 랜덤 초기 각도
    private float targetAngle; 
    // 클리어 조건 확인 버튼
    public Button clearCheckButton;
    // 회전 중 표시 변수
    private bool isRotate;
    // o,x 이미지
    public Image oImage;
    public Image xImage;
    void Start()
    {
        // 퍼즐 카메라가 할당되지 않은 경우, 태그를 사용하여 퍼즐 카메라를 찾음
        if (puzzleCamera == null)
        {
            puzzleCamera = GameObject.FindGameObjectWithTag("PuzzleCamera").GetComponent<Camera>();
        }
        // 기준키 회전 각도 랜덤하게 설정
        targetAngle = Random.Range(0f, 360f);
        // 각도에 맞춰 회전
        pivotKey.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        // O,X 이미지를 초기에는 숨김
        oImage.gameObject.SetActive(false);
        xImage.gameObject.SetActive(false);
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
            isRotate = false;
        }
        if(isRotate)
        {
            rotateKey.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
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
        if (hit.collider != null && hit.collider.gameObject == rotateKey)
        {
            isRotate = true;
        }
    }

    // 코루틴 시작 함수(버튼과 연결)
    public void StartCheckClear()
    {
        StartCoroutine(CheckClear());
    }

    IEnumerator CheckClear()
    {
        // rotatekey의 현재 각도와 pivotkey의 각도 차이를 구함
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(rotateKey.transform.eulerAngles.z, targetAngle));

        // 열쇠가 정답인지 확인
        if (angleDifference <= toleranceAngle)
        {
            // 정답인 경우, 체크 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(oImage));
            // puzzlesuccess 호출
            PuzzleManager.instance.PuzzleSuccess();
            // 제거할 프리팹 없으니 바로 씬 닫기
            SceneManager.UnloadSceneAsync("HouseWindowPuzzleScene");
            ClosePuzzleScene();
        }
        else
        {
            // 오답인 경우 X 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(xImage));
        }
    }

    // 정답, 오답 유무에 따른 이미지 표시 코루틴
    IEnumerator ShowImage(Image image)
    {
        // 이미지 표시
        image.gameObject.SetActive(true);
        // 0.5초 대기(코루틴이 매개변수 시간만큼 일시정지됨)
        yield return new WaitForSeconds(0.5f); 
        // 이미지 숨김
        image.gameObject.SetActive(false); 
    }
    public void OnClosePuzzleButtonClicked()
    {
        PuzzleManager.instance.ClickPuzzleCloseButton();
        ClosePuzzleScene();
    }
    void ClosePuzzleScene()
    {
        // 제거할 프리팹 없으니 바로 씬 닫기
        SceneManager.UnloadSceneAsync("HouseWindowPuzzleScene");
    }
}
