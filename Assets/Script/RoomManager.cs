using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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

    // 준비 상태
    private bool player1Ready = false;
    private bool player2Ready = false;

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

            // 초기화 시 캐릭터 이미지를 흑백으로 설정
            SetCharacterImagesToBlackAndWhite();
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
            StartCoroutine(StartGameAfterDelay(5));
        }
    }

    // 5초 후에 게임 시작
    private IEnumerator StartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("PrisonScene");
    }

    public void ReadyUp()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Character"))
        {
            string character = PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString();

            if (character == "Dave")
            {
                // 준비 상태 전환
                player1Ready = !player1Ready;
                player1ReadyText.text = player1Ready ? "Ready!" : "";
                
                // 플레이어의 준비 상태를 네트워크에 동기화
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Ready", player1Ready } });
            }
            else if (character == "Matthew")
            {
                player2Ready = !player2Ready;
                player2ReadyText.text = player2Ready ? "Ready!" : "";
                
                // 플레이어의 준비 상태를 네트워크에 동기화
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Ready", player2Ready } });
            }

            // 모든 플레이어가 준비됐는지 확인
            CheckAllPlayersReady();
        }
    }

    // 플레이어가 룸에 입장했을 때 호출되는 콜백 (자신 포함)
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "이(가) 방에 들어왔습니다.");

        // 플레이어 텍스트 및 이미지 업데이트
        UpdatePlayerTexts();
        UpdateCharacterImages();
    }

    // 플레이어의 커스텀 프로퍼티가 업데이트될 때 호출되는 함수
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

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
                    // 준비 상태 표시
                    player1ReadyText.text = player1Ready ? "Ready!" : "";
                    // 로컬 플레이어가 Player1이면 "You" 표시
                    player1Indicator.text = player.IsLocal ? "You" : "";
                }
                else if (character == "Matthew")
                {
                    // Player2가 Matthew일 경우 이미지 컬러로 변경 및 텍스트 업데이트
                    player2Image.sprite = matthewColorImage;
                    player2Text.text = "Player2";
                    // 준비 상태 표시
                    player2ReadyText.text = player2Ready ? "Ready!" : "";
                    // 로컬 플레이어가 Player2이면 "You" 표시
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

    // 플레이어 텍스트 업데이트 함수
    private void UpdatePlayerTexts()
    {
        // 플레이어의 수에 따라 텍스트 업데이트
        int playerCount = PhotonNetwork.PlayerList.Length;

        if (playerCount == 0)
        {
            player1Text.text = "플레이어를 기다리는 중...";
            player2Text.text = "플레이어를 기다리는 중...";
        }
        else if (playerCount == 1)
        {
            player1Text.text = "Player1";
            player2Text.text = "플레이어를 기다리는 중...";
        }
        else if (playerCount == 2)
        {
            player1Text.text = "Player1";
            player2Text.text = "Player2";
        }
    }

    // 방을 나간 플레이어의 캐릭터 이미지를 흑백으로 변경하는 메서드
    private void SetCharacterImagesToBlackAndWhite()
    {
        // 모든 플레이어의 캐릭터 정보 확인
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Character"))
            {
                string character = player.CustomProperties["Character"].ToString();

                // 해당 캐릭터 이미지 흑백으로 변경
                if (character == "Dave")
                {
                    player1Image.sprite = daveBWImage;
                }
                else if (character == "Matthew")
                {
                    player2Image.sprite = matthewBWImage;
                }
            }
        }
    }

    // 플레이어가 방에서 나갈 때 호출
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "이(가) 방을 나갔습니다.");

        // 나간 플레이어의 캐릭터 이미지 업데이트
        SetCharacterImagesToBlackAndWhite();

        // Player1이 나간 경우
        if (otherPlayer.ActorNumber == 1)
        {
            // 남아 있는 플레이어가 있을 경우 Player2를 Player1으로 할당
            if (PhotonNetwork.PlayerList.Length > 1)
            {
                Photon.Realtime.Player newPlayer1 = PhotonNetwork.PlayerList[1]; // Player2를 새로운 Player1으로 설정
                newPlayer1.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Character", "Matthew" } });

                // Player2 이미지 업데이트
                player1Image.sprite = matthewColorImage;
                player1Text.text = "Player1";

                // Player2의 이미지 흑백으로 설정
                player2Image.sprite = matthewBWImage;
                player2Text.text = "플레이어를 기다리는 중...";
            }
        }
        else
        {
            // 나간 플레이어가 Player2인 경우
            UpdateCharacterImages();
        }

        // 플레이어 텍스트 업데이트
        UpdatePlayerTexts();
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
