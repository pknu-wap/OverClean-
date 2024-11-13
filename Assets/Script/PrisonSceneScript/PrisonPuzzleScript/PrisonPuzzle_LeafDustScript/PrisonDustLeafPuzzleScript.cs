using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrisonDustLeafPuzzleScript : MonoBehaviour
{
    // 먼지 프리팹 목록(이미지가 추가될 때마다 먼지 프리팹 할당)
    public List<GameObject> dustObjectsList = new List<GameObject>();
    // 낙엽 프리팹 목록(이미지가 추가될 때마다 낙엽 프리팹 할당)
    public List<GameObject> leafObjectsList = new List<GameObject>();
    // 현재 퍼즐이 무엇인지 기록용 변수
    private string curPuzzle;
    // 낙엽, 먼지 총 이동 개수 확인용 변수
    public int ansCount;
    // 낙엽,먼지가 생성될 구간 변수
    private Vector2 minPosition = new Vector2(-58.0f, -54.0f);
    private Vector2 maxPosition = new Vector2(-51.0f, -46.0f);

    // 퍼즐이 풀렸는지 정보를 받아올 변수(초기값 false)
    public bool puzzleSolved = false;

    void Start()
    {
        if (SceneManager.GetSceneByName("PrisonDustPuzzleScene").isLoaded)
        {
            for (int i = 0; i < dustObjectsList.Count; i++)
            {
                GenerateObject(dustObjectsList[i]);
            }
            ansCount = dustObjectsList.Count;
            curPuzzle = "Dust";
        }
        else if (SceneManager.GetSceneByName("PrisonLeafPuzzleScene").isLoaded)
        {
            for (int i = 0; i < leafObjectsList.Count; i++)
            {
                GenerateObject(leafObjectsList[i]);
            }
            ansCount = leafObjectsList.Count;
            curPuzzle = "Leaf";
        }
        else
        {
            Debug.Log("씬 인식 안됨 오류");
        }
    }


    void Update()
    {
        // 퍼즐이 해결됐다면
        if (puzzleSolved)
        {
            PuzzleSuccess();
        }

        // X 키를 눌렀을 때 씬 닫기
        // 추후 퍼즐 닫기 같은 버튼 UI와 연결..?
        if (Input.GetKeyDown(KeyCode.X))
        {
            ClosePuzzleScene();
        }
    }

    // 키 생성 함수
    public void GenerateObject(GameObject generateObj)
    {
        // 범위 내 랜덤한 위치 설정
        Vector3 GeneratePosition = new Vector3(
                Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y),
                -2.5f
            );

        // 해당 위치에 프리팹 생성
        Instantiate(generateObj, GeneratePosition, Quaternion.identity);
    }

    // 퍼즐 성공 시 호출되는 함수
    public void PuzzleSuccess()
    {
        Debug.Log("씬이 종료됩니다.");
        // 퍼즐 매니저의 puzzlesuccess 호출
        PuzzleManager.instance.PuzzleSuccess();
        // 더이상 씬을 열 필요가 없으니 씬 닫기. 중간에 UI 삽입을 위한 시간을 추가해도 될듯?
        ClosePuzzleScene();
    }

    // 씬 닫기 함수
    void ClosePuzzleScene()
    {
        // 생성되었던 프리팹 삭제
        if (curPuzzle == "Dust")
        {
            GameObject[] destroyDust = GameObject.FindGameObjectsWithTag("Dust");
            for (int i = 0; i < destroyDust.Length; i++)
            {
                Destroy(destroyDust[i]);

            }
            SceneManager.UnloadSceneAsync("PrisonDustPuzzleScene");
        }
        else if (curPuzzle == "Leaf")
        {
            GameObject[] destroyLeaf = GameObject.FindGameObjectsWithTag("Leaf");
            for (int i = 0; i < destroyLeaf.Length; i++)
            {
                Destroy(destroyLeaf[i]);
            }
            SceneManager.UnloadSceneAsync("PrisonLeafPuzzleScene");
        }

        Debug.Log("씬이 닫혔습니다.");
    }
}
