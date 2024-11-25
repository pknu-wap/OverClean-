using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUIManager : MonoBehaviour
{
    public GameObject roomCodeInputPanel;
    public bool roomCodeInputPanelOpen = false;
    private void Update()
    {
        if(roomCodeInputPanelOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseRoomCodeInputPanel();
        }
        else if(!roomCodeInputPanelOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    // 방 코드 입력 버튼 클릭
    public void OpenRoomCodeInputPanel()
    {
        roomCodeInputPanelOpen = true;
        roomCodeInputPanel.SetActive(roomCodeInputPanelOpen);
    }

    public void CloseRoomCodeInputPanel()
    {
        roomCodeInputPanelOpen = false;
        roomCodeInputPanel.SetActive(roomCodeInputPanelOpen);
    }
    
    // Quick 버튼 클릭 
    public void OnQuickMatch()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }
    
    // 방 생성 버튼 클릭
    public void OnMakeRoom()
    {
        NetworkingManager.Instance.CreateRoom();
    }
}
