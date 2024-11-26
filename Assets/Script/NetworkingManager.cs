using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
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

    // 씬 전환시 파괴할 플레이어 프리팹 저장 리스트
    public List<GameObject> prefabsToDestroy = new List<GameObject>();

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
        // Photon 서버에 연결된 상태가 아닌 경우에만 연결 시도
        if (!PhotonNetwork.IsConnected)
        {
            ConnectToPhotonServer();
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            int playerCount = PhotonNetwork.CountOfPlayers;
            Debug.Log("현재 로비에 접속된 플레이어 수: " + playerCount);
        }
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

    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon Master 서버에 연결되었습니다.");
        PhotonNetwork.JoinLobby();
    }
    // 방 코드 생성 함수
    private string GenerateRoomCode()
    {
        string roomCode;
        // 랜덤 코드 생성하고 중복 확인하기
        do
        {
            roomCode = Random.Range(1000000, 9999999).ToString();
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
    private void AssignCharacterToPlayer(Player player)
    {
        string assignedCharacter;
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            assignedCharacter = "Dave"; // 첫 번째 플레이어는 Dave
        }
        else
        {
            assignedCharacter = "Matthew"; // 두 번째 플레이어는 Matthew
        }

        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps.Add("Character", assignedCharacter);
        playerProps.Add("Ready", false);
        player.SetCustomProperties(playerProps);

        Debug.Log(player.NickName + "에게 캐릭터 " + assignedCharacter + "가 할당되었습니다.");
        //Debug.Log("초기 레디값은" + ((bool)player.CustomProperties["Ready"] ? "true" : "false") + "입니다");
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
        SceneManager.LoadScene("RoomScene");
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
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "이(가) 방을 나갔습니다.");
        // 추가 로직이 필요한 경우 여기에 구현
    }

    // 방에서 나가는 경우에 호출
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    // 플레이어 전환을 처리하는 함수
    public void SwitchPlayers()
    {
        Debug.Log("스위칭 로직 시작");
        var localPlayer = PhotonNetwork.LocalPlayer;
        string localCharacter = localPlayer.CustomProperties["Character"].ToString();

        Photon.Realtime.Player otherPlayer = null;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.IsLocal)
            {
                otherPlayer = player;
                break;
            }
        }

        if (otherPlayer == null) return;

        string otherCharacter = otherPlayer.CustomProperties["Character"].ToString();

        ExitGames.Client.Photon.Hashtable localPlayerProperties = new ExitGames.Client.Photon.Hashtable
    {
        { "Character", otherCharacter },
    };
        localPlayer.SetCustomProperties(localPlayerProperties);

        ExitGames.Client.Photon.Hashtable otherPlayerProperties = new ExitGames.Client.Photon.Hashtable
    {
        { "Character", localCharacter },
    };
        otherPlayer.SetCustomProperties(otherPlayerProperties);

        // UI에 Ready 상태 즉시 반영
        RoomManager roomManager = FindObjectOfType<RoomManager>();
        if (roomManager != null)
        {
            roomManager.UpdateCharacterImages();
        }
    }
    
    // 씬 전환시 파괴할 플레이어 프리팹 저장용 함수
    public void InsertDestroyPlayerPrefab()
    {
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (var photonView in photonViews)
        {
            if (photonView.gameObject.name.Contains("Player"))
            {
                Debug.Log(photonView.gameObject.name + "할당됨");
                prefabsToDestroy.Add(photonView.gameObject);
            }
        }
    }

    // 저장된 플레이어 프리팹 파괴 함수
    public void DestroyPlayerPrefabList()
    {
        foreach (GameObject prefab in prefabsToDestroy)
        {
            PhotonView.Destroy(prefab);
        }
        prefabsToDestroy.Clear();
    }
}