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
    public Image player1Image;
    public Image player2Image;

    // 흑백 이미지
    public Sprite daveBWImage;
    public Sprite matthewBWImage;

    // 컬러 이미지
    public Sprite daveColorImage;
    public Sprite matthewColorImage;

    // 플레이어 텍스트
    public Text player1Text;
    public Text player2Text;

    // "You" 텍스트를 위한 변수
    public Text player1Indicator;
    public Text player2Indicator;

    // 레디 상태 표시 텍스트
    public Text player1ReadyText;
    public Text player2ReadyText;

    // 레디 버튼
    public Button player1ReadyButton;
    public Button player2ReadyButton;

    // 준비 상태
    private bool player1Ready = false;
    private bool player2Ready = false;

    // 게임 시작 코루틴
    private Coroutine startGameCoroutine;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            // 방에 입장 시 방 코드 표시
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RoomCode"))
            {
                roomCodeText.text = "입장 코드 : " + PhotonNetwork.CurrentRoom.CustomProperties["RoomCode"].ToString();
            }

            // 초기 텍스트 설정
            player1Text.text = "플레이어를 기다리는 중...";
            player2Text.text = "플레이어를 기다리는 중...";

            // 방에 입장 시 초기 이미지 설정
            player1Image.sprite = daveBWImage;
            player2Image.sprite = matthewBWImage;

            // 캐릭터 이미지 업데이트
            UpdateCharacterImages();

            // 플레이어별로 버튼 접근 권한 설정
            SetReadyButtonInteractivity();
        }
    }

    // 두 플레이어 모두 준비됐는지 확인하는 함수
    private void CheckAllPlayersReady()
    {
        bool allReady = true;

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
                startGameCoroutine = StartCoroutine(StartGameAfterDelay(5));
            }
        }
        else
        {
            // 한 명이라도 준비를 취소하면 코루틴을 중지
            if (startGameCoroutine != null)
            {
                StopCoroutine(startGameCoroutine);
                startGameCoroutine = null;
            }
        }
    }

    // 5초 후에 게임 시작
    private IEnumerator StartGameAfterDelay(float delay)
    {
        float checkTime = 0f;
        while(checkTime < delay)
        {
            foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                if(!player.CustomProperties.ContainsKey("Ready") || !(bool)player.CustomProperties["Ready"])
                {
                    yield break;
                }
            }
            checkTime += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene("PrisonScene");
    }

    public void ReadyUpPlayer1()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Character") && PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString() == "Dave")
        {
            // Player1의 준비 상태 전환
            player1Ready = !player1Ready;
            player1ReadyText.text = player1Ready ? "Ready!" : "Not Ready";

            // Player1의 준비 상태를 네트워크에 동기화
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Ready", player1Ready } });

            // 모든 플레이어가 준비됐는지 확인
            CheckAllPlayersReady();
        }
    }

    public void ReadyUpPlayer2()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Character") && PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString() == "Matthew")
        {
            // Player2의 준비 상태 전환
            player2Ready = !player2Ready;
            player2ReadyText.text = player2Ready ? "Ready!" : "Not Ready";

            // Player2의 준비 상태를 네트워크에 동기화
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Ready", player2Ready } });

            // 모든 플레이어가 준비됐는지 확인
            CheckAllPlayersReady();
        }
    }

    // 플레이어별 레디 버튼을 자신의 캐릭터에 맞게 활성화 또는 비활성화
    private void SetReadyButtonInteractivity()
    {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Character"))
            {
                string character = player.CustomProperties["Character"].ToString();
                if(player.IsLocal)
                {
                    if (character == "Dave")
                    {
                        player1ReadyButton.interactable = true;
                        player2ReadyButton.interactable = false;
                    }
                    else if(character == "Matthew"){
                        player1ReadyButton.interactable = false;
                        player2ReadyButton.interactable = true;
                    }
                }
            }
        }
    }

    // 플레이어의 커스텀 프로퍼티가 업데이트될 때 호출되는 함수
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (changedProps.ContainsKey("Ready"))
        {
            CheckAllPlayersReady();
        }
        // 캐릭터 속성이 변경되었을 때 이미지 업데이트
        UpdateCharacterImages();
    }

    // 플레이어의 캐릭터 속성에 따라 이미지를 업데이트하는 함수
    public void UpdateCharacterImages()
    {
        // Player1과 Player2의 캐릭터 속성을 확인하여 정보 업데이트
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Character"))
            {
                string character = player.CustomProperties["Character"].ToString();
                if (character == "Dave")
                {
                    // Player1이 Dave일 경우 이미지 컬러로 변경 및 텍스트 업데이트
                    player1Image.sprite = daveColorImage;
                    player1Text.text = "Player1";
                    player1ReadyText.text = player1Ready ? "Ready!" : "Not Ready";
                    player1Indicator.text = player.IsLocal ? "You" : "";
                }
                else if (character == "Matthew")
                {
                    // Player2가 Matthew일 경우 이미지 컬러로 변경 및 텍스트 업데이트
                    player2Image.sprite = matthewColorImage;
                    player2Text.text = "Player2";
                    player2ReadyText.text = player2Ready ? "Ready!" : "Not Ready";
                    player2Indicator.text = player.IsLocal ? "You" : "";
                }
            }
        }

        // 추가: 만약 스위칭 로직에서 호출되면 즉시 "You" 텍스트 업데이트
        UpdatePlayerIndicator();
    }

    private void UpdatePlayerIndicator()
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
                        player1Indicator.text = "You";
                        player2Indicator.text = "";
                    }
                    else if (character == "Matthew")
                    {
                        player1Indicator.text = "";
                        player2Indicator.text = "You";
                    }
                }
            }
        }
    }

    // 방을 나간 플레이어의 캐릭터 이미지를 흑백으로 변경하는 메서드
    private void SetCharacterImagesToBlackAndWhite()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Character"))
            {
                string character = player.CustomProperties["Character"].ToString();

                if (character == "Matthew")
                {
                    return;
                }
            }
        }
        player2Image.sprite = matthewBWImage;
    }

    // 플레이어가 방에서 나갈 때 호출
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (otherPlayer.CustomProperties["Character"].ToString() == "Dave")
        {
            Photon.Realtime.Player remainingPlayer;
            if(PhotonNetwork.PlayerList[0] == otherPlayer)
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
                if(remainingPlayer.CustomProperties.ContainsKey(key))
                {
                    newProperties[key] = otherPlayer.CustomProperties[key];
                }
            }
            remainingPlayer.SetCustomProperties(newProperties);
        }
        // 매튜를 흑백처리
        player2Image.sprite = matthewBWImage;
        // You 
        UpdatePlayerIndicator();
        // 버튼 조작권한
        SetReadyButtonInteractivity();
        if(PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Ready"))
        {
            player1ReadyText.text = (bool)PhotonNetwork.LocalPlayer.CustomProperties["Ready"] ? "Ready!" : "";
        }
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