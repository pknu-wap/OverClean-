using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    // UI 요소 연결하기
    public Button EnterRoomButton;
    // 방 코드 입력 필드
    public InputField roomCodeInputField;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            NetworkingManager.Instance.LeaveRoom();
        }
        // 방 입장 버튼 클릭 이벤트 설정
        EnterRoomButton.onClick.AddListener(JoinRoomWithCode); 
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