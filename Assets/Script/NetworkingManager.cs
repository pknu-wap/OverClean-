using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    // 싱글톤 인스턴스
    public static NetworkingManager Instance;

    // 게임 버전 설정
    private string gameVersion = "1";
    
    // 기존 방 코드를 저장할 HashSet
    private HashSet<string> existingRoomCodes = new HashSet<string>();

    private void Awake()
    {
        // 싱글톤 패턴 구현: NetworkingManager가 중복되지 않도록 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // 씬 전환 시에도 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // 게임 시작 시 서버 연결
        ConnectToPhotonServer();
    }

    // Photon 서버에 연결하는 함수
    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            // 씬 자동 동기화
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = gameVersion;
            // Photon 서버 연결
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Start 버튼 클릭 시 GameLobby 씬으로 이동
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("GameLobby");
    }

    // Exit 버튼 클릭 시 게임 종료
    public void OnExitButtonClicked()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }

    // 방 코드 생성 함수
    private string GenerateRoomCode()
    {
        string roomCode;
        // 랜덤 코드 생성하고 중복 확인하기
        do
        {
            roomCode = UnityEngine.Random.Range(1000000, 9999999).ToString();
        } while (existingRoomCodes.Contains(roomCode));

        // 생성된 코드는 여기 저장
        existingRoomCodes.Add(roomCode); 
        return roomCode;
    }

    // 방 생성 요청 함수
    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            // 방 코드 생성
            string roomCode = GenerateRoomCode();
            // 방 설정(최대 플레이어 수만 설정함)
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;

            // 방에 저장할 커스텀 프로퍼티(방 코드) 설정
            ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
            roomProps.Add("RoomCode", roomCode);
            roomOptions.CustomRoomProperties = roomProps;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "RoomCode" };

            // 방 이름을 방 코드로 설정하여 방 생성
            PhotonNetwork.CreateRoom(roomCode, roomOptions);
            Debug.Log("생성된 방 코드: " + roomCode);
        }
        else
        {
            Debug.LogWarning("Photon 서버에 아직 연결되지 않았습니다.");
        }
    }

    // 방 입장 요청 함수 (방 코드로 입장)
    public void JoinRoom(string roomCode)
    {
        // 방 코드를 방 이름으로 사용하여 입장 시도
        PhotonNetwork.JoinRoom(roomCode);
    }

    // 방 입장 시 캐릭터 할당
    private void AssignCharacterToPlayer(Photon.Realtime.Player player)
    {
        if (!player.CustomProperties.ContainsKey("Character"))
        {
            // 플레이어가 처음 입장한 경우에만 캐릭터를 할당
            string assignedCharacter = PhotonNetwork.PlayerList.Length == 1 ? "Dave" : "Matthew";
            ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
            playerProps.Add("Character", assignedCharacter);
            player.SetCustomProperties(playerProps);

            Debug.Log(player.NickName + "에게 캐릭터 " + assignedCharacter + "가 할당되었습니다.");
        }
    }

    // Photon 서버에 연결 성공 시 호출
    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon 서버에 연결되었습니다.");
        // 로비 접속
        PhotonNetwork.JoinLobby();
    }

    // 로비에 접속 성공 시 호출
    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 접속했습니다.");
        // 로비에 접속된 플레이어 수 출력
        // 전체 연결된 플레이어 수
        int playerCount = PhotonNetwork.CountOfPlayers;
        Debug.Log("현재 로비에 접속된 플레이어 수: " + playerCount);
    }

    // 방 입장 성공 시 호출
    public override void OnJoinedRoom()
    {
        // 방 코드 가져오기
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RoomCode", out object roomCode))
        {
            Debug.Log("방 입장 성공: 방 코드 " + roomCode);
        }
        else
        {
            Debug.Log("방 입장 성공: 방 코드를 찾을 수 없습니다.");
        }

        // 플레이어에게 캐릭터 할당
        AssignCharacterToPlayer(PhotonNetwork.LocalPlayer);

        // 방에 입장 성공하면 씬 전환
        SceneManager.LoadScene("Room");
    }

    // 방 입장 실패 시 호출 (방 코드로 입장 실패 시)
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("방 입장 실패: " + message);
    }

    // 방 생성 실패 시 호출
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("방 생성 실패: " + message);
    }

    // 플레이어가 방에서 나갈 때 호출
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "이(가) 방을 나갔습니다.");

        // 남아 있는 플레이어의 캐릭터가 유지되도록 처리
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Character"))
            {
                string character = player.CustomProperties["Character"].ToString();
                Debug.Log("남은 플레이어 " + player.NickName + "의 캐릭터는 " + character + "입니다.");
            }
        }
    }

    // 방에서 나가는 경우에 호출
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
