using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class PrisonPipePuzzleScript : MonoBehaviourPun
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
    private double originXPosition = -406.8;
    private double originYPosition = 169.4;
    // 각 파이프 타일 간격 (약간의 여유 공간 포함)
    private double tileSize = 89.7;
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
    // 각 파이프의 위치를 저장
    private Vector3[] pipePositions;
    // 각 파이프의 회전 정보를 저장
    private float[] pipeRotations;
    // 각 파이프 모양 정보 저장
    private int[] pipeShapesArray;
    // 마스터 클라이언트 퍼즐 초기화 완료 상태
    public bool isMasterPuzzleReady = false;
    // 나머지 클라이언트 퍼즐 초기화 완료 상태
    public bool isElsePuzzleReady = false;

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
        // 마스터 클라이언트만 퍼즐 생성
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString() + "가 MasterClient로써 퍼즐 생성.");
            curvedPipePositions = GenerateCurvedPipePairs();
            // 퍼즐 생성
            CreatePipeGrid();
        }
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
                if (IsPathBetweenCurvedPipes || curvedPipePositions.Contains((x, y)))
                {
                    // 꺾이는 파이프가 생성되야 하는 좌표라면
                    if (curvedPipePositions.Contains((x, y)))
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
                UnityEngine.Vector3 tilePosition = new UnityEngine.Vector3((float)originXPosition + x * (float)tileSize, (float)originYPosition - y * (float)tileSize, 0);
                pipeTile.transform.localPosition = tilePosition;
                // 0도, 90도, 180도, 270도 중 하나로 회전
                int randomRotation = Random.Range(0, 4) * -90;
                pipeTile.transform.localRotation = UnityEngine.Quaternion.Euler(0, 0, randomRotation);

                int viewID = x + y * gridWidth + 100;
                PhotonView photonView = pipeTile.GetComponent<PhotonView>();
                photonView.ViewID = viewID;

                // 각 파이프 타일의 스크립트에 파이프 모양, 좌표와 회전 정보를 전해줌
                PipeTileScript pipeTileScript = pipeTile.GetComponent<PipeTileScript>();
                pipeTileScript.x = x;
                pipeTileScript.y = y;
                pipeTileScript.pipeShape = pipeShape;

                // pipeTile의 위치에 따라 Dave, Matthew가 클릭 가능한 영역을 구분
                if (x < 5)
                {
                    pipeTileScript.inactivePipeTile = "Dave";
                }
                else
                {
                    pipeTileScript.inactivePipeTile = "Matthew";
                }


                // currentRotation은 실제 각도 (-90, -180 등)가 아니라 0,1,2,3으로 간편히 구분하도록 함
                pipeTileScript.currentRotation = randomRotation / -90;

                // 각 pipeTileScript를 배열에 저장
                pipeTileScripts[x, y] = pipeTileScript;
                Debug.Log($"location : {x}, {y} / pipeshape : {pipeTileScript.pipeShape} , currentRotation : {pipeTileScript.currentRotation}");
            }
        }
        photonView.RPC("SetMasterPuzzleReady", RpcTarget.All, true);
        Debug.Log("SetMasterPuzzleReady 호출완료");
    }

    [PunRPC]
    public void SyncPuzzleState()
    {
        // 퍼즐 전체 타일 수 계산 (가로 x 세로)
        int pipeCount = gridWidth * gridHeight;

        // 각 타일의 데이터를 저장할 배열 초기화
        pipePositions = new Vector3[pipeCount];
        pipeRotations = new float[pipeCount];
        pipeShapesArray = new int[pipeCount];

        // 타일 정보를 배열에 저장
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int index = y * gridWidth + x;

                // 타일 위치, 회전, 모양 정보 저장
                pipePositions[index] = pipeTileScripts[x, y].transform.localPosition;
                pipeRotations[index] = pipeTileScripts[x, y].transform.localRotation.eulerAngles.z;
                pipeShapesArray[index] = pipeTileScripts[x, y].pipeShape;
            }
        }
        // 다른 클라이언트로 초기 생성된 퍼즐 상태 전송
        photonView.RPC("ApplyPuzzleState", RpcTarget.OthersBuffered, pipeShapesArray, pipePositions, pipeRotations);

        Debug.Log("MasterClient에서 파이프 타일 위치 / 회전 / 모양 정보 전송 완료");
    }



    [PunRPC]
    public void ApplyPuzzleState(int[] shapes, Vector3[] positions, float[] rotations)
    {
        // 파이프 상태 배열 초기화
        pipeTileScripts = new PipeTileScript[gridWidth, gridHeight];
        // 전달받은 위치와 회전 정보를 사용하여 퍼즐 상태를 동기화
        if (positions != null && rotations != null && shapes != null)
        {
            Debug.Log("MasterClient가 아닌 쪽에서 파이프 타일 위치 / 회전 / 모양 정보 수신 완료");
        }
        else
        {
            Debug.Log("누락된 정보 발생");
            return;
        }

        // 전달받은 모양, 위치, 회전 정보를 사용하여 퍼즐 상태를 동기화
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int index = y * gridWidth + x;
                int viewID = x + y * gridWidth + 100;

                // 파이프 모양에 맞는 프리팹 생성
                GameObject pipeTile = Instantiate(pipeShapes[shapes[index]], pipeGrid.transform);

                // 타일 위치 및 회전 적용
                pipeTile.transform.localPosition = positions[index];
                pipeTile.transform.localRotation = Quaternion.Euler(0, 0, rotations[index]);

                // PhotonView 설정
                PhotonView photonView = pipeTile.GetComponent<PhotonView>();
                photonView.ViewID = viewID;

                // 프리팹에 속성 할당
                PipeTileScript pipeTileScript = pipeTile.GetComponent<PipeTileScript>();
                pipeTileScript.x = x; // 현재 타일의 x 좌표
                pipeTileScript.y = y; // 현재 타일의 y 좌표
                pipeTileScript.pipeShape = shapes[index]; // 파이프 모양
                pipeTileScript.currentRotation = (int)(rotations[index] / -90); // 회전 정보 저장

                // pipeTile의 위치에 따라 Dave, Matthew가 클릭 가능한 영역을 구분
                if (x < 5)
                {
                    pipeTileScript.inactivePipeTile = "Dave";
                }
                else
                {
                    pipeTileScript.inactivePipeTile = "Matthew";
                }

                // 생성된 타일을 배열에 저장
                pipeTileScripts[x, y] = pipeTileScript;
            }
        }
        photonView.RPC("SetElsePuzzleReady", RpcTarget.All, true);
        // 퍼즐 동기화 완료
        Debug.Log("퍼즐 상태 동기화 완료");
    }

    [PunRPC]
    public void SetMasterPuzzleReady(bool readyState)
    {
        isMasterPuzzleReady = readyState;
    }

    [PunRPC]
    public void SetElsePuzzleReady(bool readyState)
    {
        isElsePuzzleReady = readyState;
    }

    [PunRPC]
    public void RequestPuzzleState()
    {
        // 상태 요청 처리 (MasterClient가 호출)
        if (PhotonNetwork.IsMasterClient)
        {
            SyncPuzzleState();
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

    [PunRPC]
    // 퍼즐 성공 시 호출되는 함수
    public void PuzzleSuccess()
    {
        Debug.Log("성공, 씬이 종료됩니다.");
        PuzzleManager.instance.PuzzleSuccess();
        photonView.RPC("ClosePuzzleScene", RpcTarget.All);
    }

    [PunRPC]
    public IEnumerator PuzzleSolveCheck()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // MasterClient에서 연결 확인
            if (IsPathConnectedToEnd())
            {
                Debug.Log("퍼즐 완성 확인!");

                // 성공 이미지 표시
                photonView.RPC("ShowImageTrigger", RpcTarget.All);
            }
        }
        yield return null;
    }

    [PunRPC]
    public void PuzzleSolved()
    {
        Debug.Log("퍼즐 성공!");
        puzzleSolved = true;
    }

    [PunRPC]
    public void ShowImageTrigger()
    {
        StartCoroutine(ShowImage());
    }

    public IEnumerator ShowImage()
    {
        Debug.Log("성공 이미지가 표시되었습니다.");
        // 이미지 표시
        oImage.gameObject.SetActive(true);
        // 0.5초 대기(코루틴이 매개변수 시간만큼 일시정지됨)
        yield return new WaitForSeconds(0.5f);
        
        // 이미지 숨김
        oImage.gameObject.SetActive(false);
        // 퍼즐 성공 상태 전파
        photonView.RPC("PuzzleSolved", RpcTarget.All);
    }

    public void Update()
    {
        if (isMasterPuzzleReady && PhotonNetwork.IsMasterClient && !isElsePuzzleReady)
        {
            photonView.RPC("SetMasterPuzzleReady", RpcTarget.All, true);
        }
        // 퍼즐 상태가 준비된 경우, 마스터 클라이언트에 상태 요청
        if (isMasterPuzzleReady && !PhotonNetwork.IsMasterClient && !isElsePuzzleReady)
        {
            Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString() + "가 퍼즐 달라고 요청 중");
            // 퍼즐 상태 요청
            photonView.RPC("RequestPuzzleState", RpcTarget.MasterClient);
            isElsePuzzleReady = true;
        }
        // 퍼즐이 해결됐다면
        if (puzzleSolved)
        {
            photonView.RPC("PuzzleSuccess", RpcTarget.All);
        }
    }

    public void OnClosePuzzleButtonClicked()
    {
        PuzzleManager.instance.ClickPuzzleCloseButton();
        // 한쪽에서 닫으면 둘 다 닫히도록 설정
        photonView.RPC("ClosePuzzleScene", RpcTarget.All);
    }

    [PunRPC]
    // 씬 닫기 함수
    void ClosePuzzleScene()
    {
        // 현재 씬 닫기
        SceneManager.UnloadSceneAsync("PrisonPipePuzzleScene");
        Debug.Log("씬이 닫혔습니다.");
    }
}