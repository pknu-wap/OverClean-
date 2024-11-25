using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    // UI 요소 연결하기
    public Button QuickMatchButton;
    public Button MakeRoomButton;
    public Button EnterRoomButton;
    // 방 코드 입력 필드
    public InputField roomCodeInputField;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            NetworkingManager.Instance.LeaveRoom();
        }
        // 버튼 클릭 이벤트 설정
        QuickMatchButton.onClick.AddListener(OnQuickMatch);
        MakeRoomButton.onClick.AddListener(OnMakeRoom);
         // 방 입장 버튼 클릭 이벤트 설정
        EnterRoomButton.onClick.AddListener(JoinRoomWithCode); // EnterRoomButton 추가
    }

    // QuickMatch 버튼 클릭 시 호출
    public void OnQuickMatch()
    {
        // 랜덤 매칭
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("랜덤 방 찾기 시도 중...");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.LogWarning("Photon 서버에 연결되지 않았습니다.");
        }
    }

    // 방 생성 버튼 클릭 시 호출
    public void OnMakeRoom()
    {
        // NetworkingManager를 통해 방 생성 요청하기
        NetworkingManager.Instance.CreateRoom();
    }

    // 코드를 입력해서 룸 입장하기(EnterRoomModal panel에서)
    public void JoinRoomWithCode()
    {
        string enteredCode = roomCodeInputField.text;
        Debug.Log("입장 시도: " + enteredCode);

        if (!string.IsNullOrEmpty(enteredCode))
        {
            Debug.Log("현재 서버 상태 : " + PhotonNetwork.IsConnectedAndReady);
            if (PhotonNetwork.IsConnectedAndReady) // 상태 확인
            {
                NetworkingManager.Instance.JoinRoom(enteredCode);
            }
            else
            {
                Debug.LogWarning("Photon 네트워크가 준비되지 않았습니다. 방에 입장할 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("입장 코드를 입력하세요.");
        }
    }
}