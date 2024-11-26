using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.SocialPlatforms;
using Photon.Pun.UtilityScripts;

public class RoomManager : MonoBehaviourPunCallbacks
{
    // 방 코드 표시 텍스트
    public Text roomCodeText;

    // 플레이어 슬롯을 위한 변수
    public Image daveImage;
    public Image matthewImage;

    // 흑백 이미지
    public Sprite daveBWImage;
    public Sprite matthewBWImage;

    // 컬러 이미지
    public Sprite daveColorImage;
    public Sprite matthewColorImage;

    // 플레이어 텍스트
    public Text daveText;
    public Text matthewText;

    // "You" 텍스트를 위한 변수
    public Text daveIndicator;
    public Text matthewIndicator;

    // 레디 상태 표시 텍스트
    public Text daveReadyText;
    public Text matthewReadyText;

    // 레디 버튼
    public Button daveReadyButton;
    public Button matthewReadyButton;

    // 준비 상태
    public bool daveReady = false;
    public bool matthewReady = false;

    // 스위칭 버튼
    public Button switchButton;

    // 게임 시작 코루틴
    private Coroutine startGameCoroutine;
    // 카운트다운 텍스트 변수

    public Text countdownText; 

    // CountDownPanel UI 컴포넌트를 연결할 변수
    public RectTransform CountDownPanel;
    private void Start()
    {
        // 스위칭 버튼 로직 할당
        switchButton.onClick.AddListener(NetworkingManager.Instance.SwitchPlayers);
        if (PhotonNetwork.InRoom)
        {
            // 방에 입장 시 방 코드 표시
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RoomCode"))
            {
                roomCodeText.text = "입장 코드 : " + PhotonNetwork.CurrentRoom.CustomProperties["RoomCode"].ToString();
            }

            // 초기 텍스트 설정
            daveText.text = "플레이어를 기다리는 중...";
            matthewText.text = "플레이어를 기다리는 중...";

            // 방에 입장 시 초기 이미지 설정
            daveImage.sprite = daveBWImage;
            matthewImage.sprite = matthewBWImage;

            // 캐릭터 이미지 업데이트
            UpdateCharacterImages();

            // 플레이어별로 버튼 접근 권한 설정
            // SetReadyButtonInteractivity();
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                if(player.IsLocal)
                {
                        Debug.Log("할당 값 : 캐릭터에는 " + (string)player.CustomProperties["Character"]);
                        Debug.Log("할당 값 : 레디에는 " + ((bool)player.CustomProperties["Ready"] ? "true" : "false"));
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            LeaveRoom();
        }
    }

    // 두 플레이어 모두 준비됐는지 확인하는 함수
    private void CheckAllPlayersReady()
    {
        bool allReady = true;
        // 현재 룸의 플레이어 카운트가 MaxPlayers(2)가 아니라면 return
        if(PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            return;
        }

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Ready"))
            {
                bool isReady = (bool)player.CustomProperties["Ready"];
                if (!isReady)
                {
                    allReady = false;
                    break;
                }
            }
            else
            {
                allReady = false;
                break;
            }
        }

        if (allReady)
        {
            // 모두 준비 상태일 때 5초 후에 게임 시작
            if (startGameCoroutine == null)
            {
                CountDownPanel.gameObject.SetActive(true);
                startGameCoroutine = StartCoroutine(StartGameAfterDelay(5));
            }
        }
        else
        {
            // 한 명이라도 준비를 취소하면 코루틴을 중지
            if (startGameCoroutine != null)
            {
                CountDownPanel.gameObject.SetActive(false);
                StopCoroutine(startGameCoroutine);
                startGameCoroutine = null;
            }
        }
    }

    // 5초 후에 게임 시작
    private IEnumerator StartGameAfterDelay(float delay)
    {
        float checkTime = 0f;
        while (checkTime < delay)
        {
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                if (!player.CustomProperties.ContainsKey("Ready") || !(bool)player.CustomProperties["Ready"])
                {
                    // 준비 상태가 아닌 플레이어가 있으면 카운트다운 취소
                    countdownText.text = ""; // 카운트다운 텍스트 지우기
                    yield break;
                }
            }
             // 카운트다운 텍스트 업데이트
            countdownText.text = $"{Mathf.CeilToInt(delay - checkTime)}";
        
            checkTime += Time.deltaTime;
            yield return null;
        }

        // 카운트다운 종료 시 텍스트 초기화 및 게임 씬 로드
        countdownText.text = "";
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("PrisonScene");
        }
    }

    public void ReadyUpDave()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Character") && PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString() == "Dave")
        {
            // Dave의 준비 상태 전환
            daveReady = !daveReady;
            daveReadyText.text = daveReady ? "Ready!" : "Not Ready";

            // Dave의 준비 상태를 네트워크에 동기화
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Ready", daveReady } });

            // 모든 플레이어가 준비됐는지 확인
            CheckAllPlayersReady();
        }
    }

    public void ReadyUpMatthew()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Character") && PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString() == "Matthew")
        {
            // matthew의 준비 상태 전환
            matthewReady = !matthewReady;
            matthewReadyText.text = matthewReady ? "Ready!" : "Not Ready";

            
            // matthew의 준비 상태를 네트워크에 동기화
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Ready", matthewReady } });

            // 모든 플레이어가 준비됐는지 확인
            CheckAllPlayersReady();
        }
    }

    // 플레이어의 커스텀 프로퍼티가 업데이트될 때 호출되는 함수
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (changedProps.ContainsKey("Ready"))
        {
            Debug.Log("레디 변경 감지 작동");
            bool isReady = (bool)targetPlayer.CustomProperties["Ready"];
            if (targetPlayer.CustomProperties["Character"].ToString() == "Dave")
            {
                Debug.Log(targetPlayer.CustomProperties["Character"].ToString() + "의 상태는 " + (isReady ? "Ready!" : "Not Ready"));
                daveReadyText.text = isReady ? "Ready!" : "Not Ready";
            }
            else if (targetPlayer.CustomProperties["Character"].ToString() == "Matthew")
            {
                Debug.Log(targetPlayer.CustomProperties["Character"].ToString() + "의 상태는 " + (isReady ? "Ready!" : "Not Ready"));
                matthewReadyText.text = isReady ? "Ready!" : "Not Ready";
            }
            CheckAllPlayersReady();
        }
        // 캐릭터 속성이 변경되었을 때 이미지 업데이트
        UpdateCharacterImages();


    }


    public void UpdateCharacterImages()
{
    // Dave와 Matthew의 캐릭터 속성을 확인하여 정보 업데이트
    foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
    {
        if (player.CustomProperties.ContainsKey("Character"))
        {
            string character = player.CustomProperties["Character"].ToString();
            if (character == "Dave")
            {
                // Dave가 룸에 있을 경우 이미지 컬러로 변경 및 텍스트 업데이트
                daveImage.sprite = daveColorImage;
                daveText.text = "Player1";
                if (player.IsLocal)
                {
                    daveReadyButton.interactable = true;
                    matthewReadyButton.interactable = false;
                }

                // 레디 상태 업데이트
                if (player.CustomProperties.ContainsKey("Ready"))
                {
                    bool isReady = (bool)player.CustomProperties["Ready"];
                    daveReadyText.text = isReady ? "Ready!" : "Not Ready";
                }
            }
            else if (character == "Matthew")
            {
                // Matthew가 룸에 있을 경우 이미지 컬러로 변경 및 텍스트 업데이트
                matthewImage.sprite = matthewColorImage;
                matthewText.text = "Player2";
                if (player.IsLocal)
                {
                    daveReadyButton.interactable = false;
                    matthewReadyButton.interactable = true;
                }

                // 레디 상태 업데이트
                if (player.CustomProperties.ContainsKey("Ready"))
                {
                    bool isReady = (bool)player.CustomProperties["Ready"];
                    matthewReadyText.text = isReady ? "Ready!" : "Not Ready";
                }
            }
        }
    }

    // 추가: 만약 스위칭 로직에서 호출되면 즉시 "You" 텍스트 업데이트
    UpdatePlayerIndicator();
}


    public void UpdatePlayerIndicator()
    {
        // 플레이어 전환 시 "You" 텍스트를 알맞게 업데이트
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Character"))
            {
                string character = player.CustomProperties["Character"].ToString();
                if (player.IsLocal)
                {
                    if (character == "Dave")
                    {
                        daveIndicator.text = "You";
                        matthewIndicator.text = "";
                    }
                    else if (character == "Matthew")
                    {
                        daveIndicator.text = "";
                        matthewIndicator.text = "You";
                    }
                }
                
            }
        }
    }

    // 플레이어가 방에서 나갈 때 호출
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (otherPlayer.CustomProperties["Character"].ToString() == "Dave")
        {
            Photon.Realtime.Player remainingPlayer;
            if (PhotonNetwork.PlayerList[0] == otherPlayer)
            {
                remainingPlayer = PhotonNetwork.PlayerList[1];
            }
            else
            {
                remainingPlayer = PhotonNetwork.PlayerList[0];
            }

            var newProperties = new ExitGames.Client.Photon.Hashtable();
            foreach (var key in otherPlayer.CustomProperties.Keys)
            {
                if (remainingPlayer.CustomProperties.ContainsKey(key))
                {
                    newProperties[key] = otherPlayer.CustomProperties[key];
                }
            }
            remainingPlayer.SetCustomProperties(newProperties);
            // You 
            UpdatePlayerIndicator();
            // 버튼 조작권한
            //SetReadyButtonInteractivity();
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Ready"))
            {
                daveReadyText.text = (bool)PhotonNetwork.LocalPlayer.CustomProperties["Ready"] ? "Ready!" : "";
            }
        }
        // 전부 레디 - 시작 후 5초 대기 중일 때 누군가 방을 나간다면
        if (startGameCoroutine != null)
        {
            // 게임시작 코루틴 중지
            CountDownPanel.gameObject.SetActive(false);
            StopCoroutine(startGameCoroutine);
            startGameCoroutine = null;
        }
        // 매튜를 흑백처리
        matthewImage.sprite = matthewBWImage;
        // 매튜가 레디한 채로 나가면 not ready 메세지로 갱신되지 않음. 따로 처리
        matthewReadyText.text = "Not Ready";
    }

    // Back 버튼 클릭 시 호출되는 함수
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("방을 나가는 중입니다...");
    }

    // 방을 나가면 호출되는 콜백
    public override void OnLeftRoom()
    {
        Debug.Log("방을 떠났습니다.");
        SceneManager.LoadScene("LobbyScene");
    }
}