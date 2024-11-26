using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HouseShelfPuzzleScript : MonoBehaviour
{
    // o 이미지
    public Image oImage;
    // 접시에 생성할 얼룩 프리팹(추후 여러 이미지가 생기면 배열화)
    public List<GameObject> dustPrefabList = new List<GameObject>();
    // 생성할 프리팹 개수
    public int prefabCount = 40;
    // 파괴된 프리팹 개수
    public int destroyPrefabCount = 0;
    // 생성 범위 가장자리값 선언
    private Vector2 maxVal = new Vector2(-45,-47.5f);
    private Vector2 minVal = new Vector2(-56,-53f);
    void Awake()
    {
        // 초기에 이미지 숨김
        oImage.gameObject.SetActive(false); 
        // 프리팹 생성
        SpawnDustPrefab();
    }

    void SpawnDustPrefab()
    {
        for(int i = 0; i < prefabCount; i++)
        {
            float x = Random.Range(minVal.x,maxVal.x);
            float y = Random.Range(minVal.y,maxVal.y);
            Vector3 spawnPosition = new Vector3(x, y, -1);
            int randIdx = Random.Range(0,dustPrefabList.Count);
            // 프리팹 생성
            Instantiate(dustPrefabList[randIdx], spawnPosition, Quaternion.identity);
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
        // 씬 닫기
        SceneManager.UnloadSceneAsync("HouseShelfPuzzleScene");
    }
}
