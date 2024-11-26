using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HouseBlockPuzzleScript : MonoBehaviour
{
    // 생성될 블록 프리팹 리스트
    public List<GameObject> blockPrefab = new List<GameObject>();
    // 생성될 위치 변수
    private Vector2 minPosition = new Vector2(-58.0f, -54.0f);
    private Vector2 maxPosition = new Vector2(-51.0f, -46.0f);
    // 뚜껑 오브젝트 변수
    public GameObject puzzleCover;
    // 총 체크 프리팹 개수
    public int standardCheckPrefabCount = 45;
    // 현재 체크 프리팹 개수
    public int currentCheckPrefabCount = 0;
    // o,x 이미지
    public Image oImage;
    public Image xImage;
    void Start()
    {
        for (int i = 0; i < blockPrefab.Count; i++)
        {
            // 범위 내 랜덤한 위치 설정
            Vector3 generatePosition = new Vector3(
                Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y),
                -0.5f
            );

            // 0, 90, 180, 270도 중 하나의 랜덤한 회전 각도 선택
            float randomRotationZ = Random.Range(0, 4) * 90f;

            // 해당 설정으로 프리팹 생성
            Instantiate(blockPrefab[i], generatePosition, Quaternion.Euler(0, 0, randomRotationZ));
        }

        // UI 이미지 비활성화
        oImage.gameObject.SetActive(false);
        xImage.gameObject.SetActive(false);
    }

    // 코루틴 시작 함수(버튼과 연결)
    public void StartCheckClear()
    {
        StartCoroutine(CheckClear());
    }

    IEnumerator CheckClear()
    {
        // 열쇠가 정답인지 확인
        if (standardCheckPrefabCount == currentCheckPrefabCount)
        {
            // 정답인 경우, 체크 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(oImage));
            // puzzlesuccess 호출
            PuzzleManager.instance.PuzzleSuccess();
            ClosePuzzleScene();
        }
        else
        {
            // 오답인 경우 X 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(xImage));
            // 뚜껑 원위치
            puzzleCover.transform.position = puzzleCover.GetComponent<HouseBlockCoverScript>().firstLocate;
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
        // 퍼즐 씬에서 사용한 Block 프리팹을 전부 찾아와 제거
        GameObject[] destroyBlock = GameObject.FindGameObjectsWithTag("Block");
        for(int i = 0; i < destroyBlock.Length; i++)
        {
            Destroy(destroyBlock[i]);
        }
        // 퍼즐 씬에서 사용한 checkgrid 프리팹을 전부 찾아와 제거
        GameObject[] destroyCheckGrid = GameObject.FindGameObjectsWithTag("CheckGrid");
        for(int i = 0; i < destroyCheckGrid.Length; i++)
        {
            Destroy(destroyCheckGrid[i]);
        }
        SceneManager.UnloadSceneAsync("HouseBlockPuzzleScene");
    }
}
