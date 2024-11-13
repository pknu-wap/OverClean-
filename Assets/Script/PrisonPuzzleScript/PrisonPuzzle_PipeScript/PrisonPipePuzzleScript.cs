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
    // 파이프들의 세로줄 갯수
    private int gridWidth = 10;
    // 파이프들의 가로줄 갯수
    private int gridHeight = 5;
    // 파이프 타일 생성의 초기 x,y 절대 좌표
    private int originXPosition = -494;
    private int originYPosition = 216;
    // 각 파이프 타일 간격 (약간의 여유 공간 포함)
    private int tileSize = 110;
    // 각 파이프 타일의 변수에 접근하기 위해, 스크립트를 저장하는 배열
    public PipeTileScript[,] pipeTileScripts;

    void Start()
    {
        pipeTileScripts = new PipeTileScript[gridWidth, gridHeight];
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

                // 각 파이프 타일의 스크립트에 파이프 모양, 좌표와 회전 정보를 전해줌
                PipeTileScript pipeTileScript = pipeTile.GetComponent<PipeTileScript>();
                pipeTileScript.x = x;
                pipeTileScript.y = y;
                pipeTileScript.pipeShape = randomShape;
                // currentRotation은 실제 각도 (-90, -180 등)가 아니라 0,1,2,3으로 간편히 구분하도록 함
                pipeTileScript.currentRotation = randomRotation / -90;

                // 각 pipeTileScript를 배열에 저장
                pipeTileScripts[x, y] = pipeTileScript;
                Debug.Log($"location : {x}, {y} / pipeshape : {pipeTileScript.pipeShape} , currentRotation : {pipeTileScript.currentRotation}");
            }
        }
    }

    // 시작 파이프부터 종료 파이프까지 연결되었는지 확인하는 함수
    public bool IsPathConnectedToEnd()
    {
        bool[,] visited = new bool[gridWidth, gridHeight];
        return DFS(0, 0, visited);
    }

    // 특정 좌표의 파이프와 연결된 다른 파이프를 확인하는 함수
    // 종료 조건이 충족될 때까지, 다른 파이프의 좌표를 인자로 재귀적으로 호출됨
    private bool DFS(int x, int y, bool[,] visited)
    {
        // 종료 조건: 종료 파이프에 도달
        if (x == gridWidth - 1 && y == gridHeight - 1)
        {
            Debug.Log("경로가 시작 파이프에서 종료 파이프까지 연결되었습니다.");
            return true;
        }
        // 중복 탐색을 방지하기 위해, 해당 좌표에 방문했음을 표시
        visited[x, y] = true;
        PipeTileScript currentPipe = pipeTileScripts[x, y];
        
        // 각 방향에 대해 연결 확인 및 DFS 재귀 호출
        foreach (var direction in currentPipe.connectableDirections)
        {
            // 이 방향으로 연결되지 않았다면 다음 방향으로 넘어감
            if (!direction.Value) continue;

            int nx = x, ny = y;
            // 연결된 방향 중 어느 방향을 검사할지 결정하여 인접한 파이프 좌표를 설정
            switch (direction.Key)
            {
                case PipeTileScript.Direction.Top: ny -= 1; break;
                case PipeTileScript.Direction.Right: nx += 1; break;
                case PipeTileScript.Direction.Bottom: ny += 1; break;
                case PipeTileScript.Direction.Left: nx -= 1; break;
            }
            // 해당 좌표가 유효한지, 방문하지 않았는지 확인
            if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight && !visited[nx, ny])
            {
                PipeTileScript adjacentPipe = pipeTileScripts[nx, ny];
                // 현재 파이프와 연결되어 있는지 확인
                if (currentPipe.IsConnectedTo(adjacentPipe, direction.Key))
                {
                    // DFS 재귀 호출
                    if (DFS(nx, ny, visited))
                    {
                        // 경로 연결에 성공했다면, 제일 마지막에 실행되고 IsPathConnectedToEnd의 반환 값이 됨
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void Update()
    {   
        // Z 키를 눌렀을 때 씬 닫기
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ClosePuzzleScene();
        }
        // L 키를 눌렀을 때 파이프 연결 확인
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("keyon");
            IsPathConnectedToEnd();
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
