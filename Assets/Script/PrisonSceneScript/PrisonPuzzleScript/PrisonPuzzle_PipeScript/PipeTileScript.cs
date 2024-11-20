using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PipeTileScript : MonoBehaviour
{
    // currentRotation, pipeShape는 타일이 생성될 때 PrisonPipePuzzleScript.cs에서 정보를 받아 초기화됨
    public int currentRotation;
    public int pipeShape;
    public int x, y;
    public string inactivePipeTile;
    // 방향 정보 설정
    public enum Direction
    {
        Top, Right, Bottom, Left
    }
    // 연결 가능한 방향 정보 저장
    public Dictionary<Direction, bool> connectableDirections = new Dictionary<Direction, bool>
    {
        { Direction.Top, false },
        { Direction.Bottom, false },
        { Direction.Left, false },
        { Direction.Right, false }
    };
    // 스프라이트 렌더러
    private SpriteRenderer spriteRenderer;
    public Camera puzzleCamera;
    public PrisonPipePuzzleScript pipePuzzleScript;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // PrisonPipePuzzleScript 참조 설정
        pipePuzzleScript = FindObjectOfType<PrisonPipePuzzleScript>();
        UpdateConnectableDirections();

        // 파이프 타일에 마우스 클릭이 가능하도록 함
        if (puzzleCamera == null)
        {
            puzzleCamera = GameObject.FindGameObjectWithTag("PuzzleCamera").GetComponent<Camera>();
        }
    }

    public void RotatePipe()
    {
        // 파이프 타일이 기존 회전 각도를 유지한 상태에서 90도 회전
        transform.Rotate(0, 0, -90);
        // 0,1,2,3 으로 회전 정보 구분
        currentRotation = (currentRotation + 1) % 4;
        // 회전 시 마다 연결 정보 변경
        UpdateConnectableDirections();
    }

    private void UpdateConnectableDirections()
    {
        // 파이프 모양에 따른 연결 설정
        switch (pipeShape)
        {
            // 일자 모양
            case 0:
                connectableDirections[(Direction)currentRotation] = true;
                connectableDirections[(Direction)((currentRotation + 1) % 4)] = false;
                connectableDirections[(Direction)((currentRotation + 2) % 4)] = true;
                connectableDirections[(Direction)((currentRotation + 3) % 4)] = false;
                break;
            // L자 모양
            case 1:
                connectableDirections[(Direction)currentRotation] = true;
                connectableDirections[(Direction)((currentRotation + 1) % 4)] = true;
                connectableDirections[(Direction)((currentRotation + 2) % 4)] = false;
                connectableDirections[(Direction)((currentRotation + 3) % 4)] = false;
                break;
            // T자 모양
            case 2:
                connectableDirections[(Direction)currentRotation] = true;
                connectableDirections[(Direction)((currentRotation + 1) % 4)] = true;
                connectableDirections[(Direction)((currentRotation + 2) % 4)] = true;
                connectableDirections[(Direction)((currentRotation + 3) % 4)] = false;
                break;
            default:
                break;
        }
    }


    // 인접한 파이프와 인자로 받은 방향으로 연결이 가능한지 확인
    // 인접한 파이프의 스크립트를 받아와 connectableDirections에 접근
    public bool IsConnectedTo(PipeTileScript otherPipe, Direction direction)
    {
        Direction oppositeDirection = GetOppositeDirection(direction);
        return connectableDirections[direction] && otherPipe.connectableDirections[oppositeDirection];    
    }

    // 인자로 받은 방향의 반대 방향을 반환하는 함수 
    private Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Top: return Direction.Bottom;
            case Direction.Bottom: return Direction.Top;
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
            default: return direction;
        }
    }

    private void OnMouseDownHandler()
    {
        // 마우스 위치를 월드 좌표로 변환 (2D 평면에서의 위치만 사용)
        Vector3 mousePosition = puzzleCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -puzzleCamera.transform.position.z));
        // 2D 평면에서의 위치만 사용
        mousePosition.z = 0f;

        // 현재 오브젝트에 마우스 클릭이 감지되었는지 확인
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject == gameObject && PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString() == inactivePipeTile)
        {
            RotatePipe();
            StartCoroutine(pipePuzzleScript.puzzleSolveCheck());
            Debug.Log($"경로 연결 성공 여부 : {pipePuzzleScript.puzzleSolved}");
            Debug.Log($"x : {x}, y : {y}, pipeshape : {pipeShape} , currentRotation : {currentRotation}");
            Debug.Log($"connectableDirections : {connectableDirections[Direction.Top]}, {connectableDirections[Direction.Right]}, {connectableDirections[Direction.Bottom]}, {connectableDirections[Direction.Left]}");
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDownHandler();
        }
    }
}
