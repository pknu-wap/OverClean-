using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrisonPipePuzzleScript : MonoBehaviour
{
    public PipeTileScript pipeTileScript;
    
    // Grid Layout Group을 포함한 빈 오브젝트
    public GameObject pipeGrid;
    // 파이프 모양 배열 (일자, L자, T자)
    public GameObject[] pipeShapes;
    
    private int gridWidth = 10;
    private int gridHeight = 5;

    // 파이프 타일 생성의 초기 x,y 좌표
    private int originXPosition = -494;
    private int originYPosition = 216;
    // 각 타일 간격 (약간의 여유 공간 포함)
    private int tileSize = 110;

    void Start()
    {
        CreatePipeGrid();
    }

    void CreatePipeGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                // 랜덤한 파이프 프리팹 선택
                int randomIndex = Random.Range(0, pipeShapes.Length);
                GameObject pipeTile = Instantiate(pipeShapes[randomIndex], pipeGrid.transform);
                
                // 위치 및 회전 설정
                // pipeTile.transform.localPosition = Vector3.zero;
                // 타일 위치 계산
                Vector3 tilePosition = new Vector3(originXPosition + x * tileSize, originYPosition -y * tileSize, 0);
                pipeTile.transform.localPosition = tilePosition;
                // 0도, 90도, 180도, 270도 중 하나로 회전
                pipeTile.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 4) * 90);
            }
        }
    }
    void Update()
    {   
        // if (pipeTileScript != null)
        // {
        //     if (Input.GetMouseButtonDown(0))
        //     {
        //         pipeTileScript.RotateTile();
        //     }
        // }
        // Z 키를 눌렀을 때 씬 닫기
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ClosePuzzleScene();
        }
    }

    // 씬 닫기 함수
    void ClosePuzzleScene()
    {
        // 현재 씬 닫기
        SceneManager.UnloadSceneAsync("PrisonPipePuzzleScene");
        Debug.Log("씬이 닫혔습니다.");
    }
}
