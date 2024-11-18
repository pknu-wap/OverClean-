using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    // 파이프 길 생성 시에, 길의 꺾이는 파이프의 위치 정보 모음
    List<(int, int)> curvedPipePositions;
    // 퍼즐이 풀렸는지 정보를 받아올 변수(초기값 false)
    public bool puzzleSolved = false;
    // 체크 이미지를 표시하기 위한 UI 이미지
    public Image oImage;
    // 틀렸을 때 표시하기 위한 UI 이미지
    public Image xImage;

    public void Awake()
    {
        // 이미지 참조
        oImage = GameObject.Find("CorrectAnswerImage").GetComponent<Image>();
        xImage = GameObject.Find("WrongAnswerImage").GetComponent<Image>();
        // 초기에는 이미지 숨김
        oImage.gameObject.SetActive(false); 
        xImage.gameObject.SetActive(false); 
    }

    public void Start()
    {
        pipeTileScripts = new PipeTileScript[gridWidth, gridHeight];
        curvedPipePositions = GenerateCurvedPipePairs();
        CreatePipeGrid();
    }

    public void CreatePipeGrid()
    {
        // 꺾이는 파이프 (시작 및 종료 파이프 포함) 사이는 일자 파이프를 생성해야 함
        // 일자 파이프를 만들어야 하는 길인지 확인하는 변수
        bool IsPathBetweenCurvedPipes = true;
        // 파이프 모양을 저장하기 위한 변수
        int pipeShape;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (IsPathBetweenCurvedPipes || curvedPipePositions.Contains((x,y)))
                {
                    // 꺾이는 파이프가 생성되야 하는 좌표라면
                    if (curvedPipePositions.Contains((x,y)))
                    {
                        // 이후론 일자 파이프만 생성할 필요 없기 때문에 false로 만들어줌
                        // 다시 꺾이는 파이프를 만나면 true로 바꿔줌
                        IsPathBetweenCurvedPipes = !IsPathBetweenCurvedPipes;
                        // 파이프 모양 L자 또는 T자 중 랜덤으로 선택
                        pipeShape = Random.value < 0.5f ? 1 : 2;
                    }
                    else
                    {
                        // 파이프 모양 일자 선택
                        pipeShape = 0;
                    }
                }
                else
                {
                    // 파이프 모양 랜덤으로 선택
                    pipeShape = Random.Range(0, 3);
                }

                // 로직에 따라 변경된 파이프 프리팹 모양으로 인스턴스화
                GameObject pipeTile = Instantiate(pipeShapes[pipeShape], pipeGrid.transform);

                // 타일 위치 계산
                UnityEngine.Vector3 tilePosition = new UnityEngine.Vector3(originXPosition + x * tileSize, originYPosition -y * tileSize, 0);
                pipeTile.transform.localPosition = tilePosition;
                // 0도, 90도, 180도, 270도 중 하나로 회전
                int randomRotation = Random.Range(0, 4) * -90;
                pipeTile.transform.localRotation = UnityEngine.Quaternion.Euler(0, 0, randomRotation);

                // 각 파이프 타일의 스크립트에 파이프 모양, 좌표와 회전 정보를 전해줌
                PipeTileScript pipeTileScript = pipeTile.GetComponent<PipeTileScript>();
                pipeTileScript.x = x;
                pipeTileScript.y = y;
                pipeTileScript.pipeShape = pipeShape;
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
        bool isStartPipeConnectedToLeft = pipeTileScripts[0, 0].connectableDirections[PipeTileScript.Direction.Left] == true;
        bool isEndPipeConnectedToRight = pipeTileScripts[9, 4].connectableDirections[PipeTileScript.Direction.Right] == true;

        return DFS(0, 0, visited) && isStartPipeConnectedToLeft && isEndPipeConnectedToRight;
    }

    // 특정 좌표의 파이프와 연결된 다른 파이프를 확인하는 함수
    // 종료 조건이 충족될 때까지, 다른 파이프의 좌표를 인자로 재귀적으로 호출됨
    private bool DFS(int x, int y, bool[,] visited)
    {
        // 종료 조건: 종료 파이프에 도달
        if (x == gridWidth - 1 && y == gridHeight - 1)
        {
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

    // 성공 경로에 쓰일 꺾이는 파이프 묶음을 랜덤하게 결정하는 함수
    List<(int, int)> GenerateCurvedPipePairs()
    {
        List<(int, int)> curvedPipePositions = new List<(int, int)>();
        HashSet<int> usedXPositions = new HashSet<int>(); // 중복 방지용

        for (int i = 0; i < 4; i++)
        {
            int x, y;
            do
            {
                x = Random.Range(1, 9); // x 범위 : 1~8
            } while (usedXPositions.Contains(x)); // 이미 존재하는 x면 다시 선택하러 반복
            usedXPositions.Add(x);

            y = i; // y는 0~3에서 순서대로 배치하여 y축 중복을 방지

            // (x, y)와 (x, y + 1) 두 개 위치를 리스트에 추가
            curvedPipePositions.Add((x, y));
            curvedPipePositions.Add((x, y + 1));
            Debug.Log($"curvedPipe location : {x}, {y} / {x} , {y + 1}");
        }

        return curvedPipePositions;
    }

    // 퍼즐 성공 시 호출되는 함수
    public void PuzzleSuccess()
    {
        Debug.Log("씬이 종료됩니다.");
        PuzzleManager.instance.PuzzleSuccess();
        ClosePuzzleScene();
    }

    IEnumerator puzzleSolveCheck()
    {
        // 연결 성공인지 확인
        if (IsPathConnectedToEnd())
        {
            // 체크 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(oImage));
            // 퍼즐 매니저에 퍼즐 성공 상태 전달
            puzzleSolved = true;
        }
        else
        {
            yield return StartCoroutine(ShowImage(xImage));
        }
    }

    IEnumerator ShowImage(Image image)
    {
        Debug.Log("성공 이미지가 표시되었습니다.");
        // 이미지 표시
        image.gameObject.SetActive(true);
        // 0.5초 대기(코루틴이 매개변수 시간만큼 일시정지됨)
        yield return new WaitForSeconds(0.5f); 
        // 이미지 숨김
        image.gameObject.SetActive(false); 
    }

    public void Update()
    {   
        // Z 키를 눌렀을 때 씬 닫기
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ClosePuzzleScene();
        }
        // L 키를 눌렀을 때 파이프 연결 확인
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(puzzleSolveCheck());
            Debug.Log($"경로 연결 성공 여부 : {puzzleSolved}");
        }
        // 퍼즐이 해결됐다면
        if (puzzleSolved)
        {
            PuzzleSuccess();
        }
    }
    public void OnClosePuzzleButtonClicked()
    {
        ClosePuzzleScene();
    }


    // 씬 닫기 함수
    void ClosePuzzleScene()
    {
        // 현재 씬 닫기
        SceneManager.UnloadSceneAsync("PrisonPipePuzzleScene");
        Debug.Log("씬이 닫혔습니다.");
    }
}
