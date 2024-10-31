using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrisonPipePuzzleScript : MonoBehaviour
{
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
                // 랜덤한 파이프 프리팹 모양 선택
                int randomShape = Random.Range(0, pipeShapes.Length);
                GameObject pipeTile = Instantiate(pipeShapes[randomShape], pipeGrid.transform);

                // 타일 위치 계산
                Vector3 tilePosition = new Vector3(originXPosition + x * tileSize, originYPosition -y * tileSize, 0);
                pipeTile.transform.localPosition = tilePosition;
                // 0도, 90도, 180도, 270도 중 하나로 회전
                int randomRotation = Random.Range(0, 4) * -90;
                pipeTile.transform.localRotation = Quaternion.Euler(0, 0, randomRotation);

                // 각 파이프 타일의 스크립트에 파이프 모양과 회전 정보를 전해줌
                PipeTileScript pipeTileScript = pipeTile.GetComponent<PipeTileScript>();
                pipeTileScript.pipeShape = randomShape;
                // currentRotation은 실제 각도 (-90, -180 등)이 아니라 0,1,2,3으로 간편히 구분하도록 함
                pipeTileScript.currentRotation = randomRotation / -90;
                Debug.Log($"location : {x}, {y} / pipeshape : {pipeTileScript.pipeShape} , currentRotation : {pipeTileScript.currentRotation}");
            }
        }
    }
    void Update()
    {   
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
