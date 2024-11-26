using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HouseSinkPuzzleScript : MonoBehaviour
{
    // o 이미지
    public Image oImage;
    // 접시에 생성할 얼룩 프리팹(추후 여러 이미지가 생기면 배열화)
    public GameObject dirtyPrefab;
    // 생성할 프리팹 개수
    public int prefabCount = 15;
    // 파괴된 프리팹 개수
    public int destroyPrefabCount = 0;
    // 접시(원)의 중심 미리 선언
    private Vector2 dishCenter = new Vector2(-50,-50);
    // 생성될 원 반지름
    private float radius = 4f;
    void Start()
    {
        // 초기에 이미지 숨김
        oImage.gameObject.SetActive(false); 
        // 프리팹 생성
        SpawnDirtyPrefab();
    }

    void SpawnDirtyPrefab()
    {
        for(int i = 0; i < prefabCount; i++)
        {
            // 랜덤한 각도와 거리 생성
            float angle = Random.Range(0f, Mathf.PI * 2);
            float distance = Random.Range(0f, radius);

            // Polar to Cartesian 변환
            float x = dishCenter.x + distance * Mathf.Cos(angle);
            float y = dishCenter.y + distance * Mathf.Sin(angle);
            Vector2 spawnPosition = new Vector2(x, y);

            // 프리팹 생성
            Instantiate(dirtyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 파괴된 프리팹 카운트가 기존 프리팹 카운트와 같아진다면
        if(prefabCount == destroyPrefabCount)
        {
            // 클리어 코루틴 실행
            StartCoroutine(ShowImage(oImage));
        }
    }

    // o 이미지를 띄우고 퍼즐 종료
    IEnumerator ShowImage(Image image)
    {
        // 이미지 표시
        image.gameObject.SetActive(true);
        // 0.5초 대기(코루틴이 매개변수 시간만큼 일시정지됨)
        yield return new WaitForSeconds(0.5f); 
        // 이미지 숨김
        image.gameObject.SetActive(false); 
        // puzzlesuccess 호출
        PuzzleManager.instance.PuzzleSuccess();
        ClosePuzzleScene();
    }

    public void OnClosePuzzleButtonClicked()
    {
        PuzzleManager.instance.ClickPuzzleCloseButton();
        ClosePuzzleScene();
    }
    void ClosePuzzleScene()
    {
        // 씬 닫기
        SceneManager.UnloadSceneAsync("HouseSinkPuzzleScene");
    }
}
