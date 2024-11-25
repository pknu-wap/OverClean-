using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class HouseBookPuzzleScript : MonoBehaviour
{
    // 생성될 책 프리팹
    public GameObject bookPrefab;
    // 뚜껑 오브젝트 변수
    public GameObject bookShelfCover;
    // 책 프리팹 생성 위치
    private Vector2 bookMinPosition = new Vector2(-35.0f, -43.0f);
    private Vector2 bookMaxPosition = new Vector2(-47.5f, -47.5f);
    // 생성할 프리팹 (사용할 프리팹을 할당)
    public GameObject checkPrefab;
    // 체크 프리팹 총 개수
    public int prefabCount = 7;
    // 체크 프리팹을 담아둘 배열(답 확인할 때 참조용)
    public GameObject[] checkPrefabList = new GameObject[7];
    // 생성할 프리팹 시작 지점과 끝 지점
    float startX = -64.1f;
    float endX = -53.8f;
    // o,x 이미지
    public Image oImage;
    public Image xImage;
    // 책 목록 배열 리스트
    private string[] bookList = { "빨강", "주황", "노랑", "초록", "파랑", "보라", "검정" };
    // 힌트 UI 텍스트 참조
    public TMP_Text uiHintText;


    void Start()
    {
        // 배열 섞기
        ShuffleArray(bookList);

        // 섞인 배열 출력 (디버그용)
        Debug.Log("섞기 완료");
        foreach (var book in bookList)
        {
            Debug.Log(book);
        }
        // 프리팹 x좌표 간격 계산
        float step = (endX - startX) / (prefabCount - 1);
        // 반복문으로 책, 체크 프리팹 생성
        for (int i = 0; i < prefabCount; i++)
        {
            // 체크 프리팹 현재 x 좌표 계산
            float x = startX + i * step; 
            // 체크 프리팹 위치
            Vector3 position = new Vector3(x, -54.0f, 0); 

            // 체크 프리팹 생성
            GameObject newCheckPrefab = Instantiate(checkPrefab, position, Quaternion.identity);
            // 체크 프리팹에 체크해야 할 색상 할당
            newCheckPrefab.GetComponent<CheckPrefabScript>().thisLocationBook = bookList[i];
            // 체크프리팹 배열에 할당
            checkPrefabList[i] = newCheckPrefab;

            // 책 오브젝트 생성 위치 지정
            float randomX = Random.Range(bookMinPosition.x, bookMaxPosition.x);
            float randomY = Random.Range(bookMinPosition.y, bookMaxPosition.y);
            Vector3 bookPosition = new Vector3(randomX, randomY, -0.5f);

            // 책 프리팹 반환된 참조 저장
            GameObject newBook = Instantiate(bookPrefab, bookPosition, Quaternion.identity);

            // 새로 생성된 책 오브젝트에 색상 할당
            newBook.GetComponent<BookPrefabScript>().thisBookColor = bookList[i];
        }

        // UI 이미지 비활성화
        oImage.gameObject.SetActive(false);
        xImage.gameObject.SetActive(false);

        // UI 텍스트 초기화
        uiHintText.text = string.Format(
            bookList[0] + " 책은 " + bookList[2] + " 책의 왼쪽에 있습니다.\n\n" +
            bookList[3] + " 책은 왼쪽 " + bookList[1] + " 책과 오른쪽 " + bookList[4] + " 책 사이에 있고 " + bookList[2] + " 책의 오른쪽에 있습니다.\n\n" +
            bookList[5] + " 책은 " + bookList[4] + " 책 오른쪽이며 끝자리가 아닙니다.\n\n" +
            bookList[1] + " 책은 양 끝자리가 아닙니다.\n\n" +
            bookList[2] + " 책은 3번째 자리입니다."
        );
    }

    // 배열 섞는 메서드
    private void ShuffleArray<String>(String[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            // 0에서 i 사이의 랜덤 인덱스
            int j = Random.Range(0, i + 1);
            // 요소 교환
            String temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    // 코루틴 시작 함수
    public void StartCheckClear()
    {
        StartCoroutine(CheckClear());
    }

    IEnumerator CheckClear()
    {
        bool isCorrect = true;
        for(int i = 0; i < prefabCount; i++)
        {
            // 만약 올바르지 않은 쌍이 연결되어 있다면
            if(!checkPrefabList[i].GetComponent<CheckPrefabScript>().isCorrectBook)
            {
                // 틀림을 할당하고 탈출
                isCorrect = false;
                break;
            }
        }
        if (isCorrect)
        {
            // 정답인 경우, 체크 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(oImage));
            // puzzlesuccess 호출
            PuzzleManager.instance.PuzzleSuccess();
            // 퍼즐 씬에서 사용한 book 프리팹을 전부 찾아와 제거
            GameObject[] destroyBook = GameObject.FindGameObjectsWithTag("Book");
            for(int i = 0; i < destroyBook.Length; i++)
            {
                Destroy(destroyBook[i]);
            }
            // 퍼즐 씬에서 사용한 checkgrid 프리팹을 전부 찾아와 제거
            GameObject[] destroyCheckGrid = GameObject.FindGameObjectsWithTag("CheckGrid");
            for(int i = 0; i < destroyCheckGrid.Length; i++)
            {
                Destroy(destroyCheckGrid[i]);
            }
            SceneManager.UnloadSceneAsync("HouseBookPuzzleScene");
        }
        else
        {
            // 오답인 경우 X 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(xImage));
            // 뚜껑 원위치
            bookShelfCover.transform.position = bookShelfCover.GetComponent<HouseBookCoverScript>().firstLocate;
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
}
