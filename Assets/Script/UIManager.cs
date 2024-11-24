using UnityEngine;
using UnityEngine.UI;  
using UnityEngine.SceneManagement;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public RectTransform tutorialPanel;
    
    // 버튼이 있는 일시정지 창 -> 로비로 돌아가기, 게임으로 돌아가기
    public RectTransform pausePanel;

    // 버튼이 없는 일시정지 창 "일시정지됨"
    public RectTransform pauseTextPanel;
    public RectTransform titleExitPanel;
    public RectTransform roomCodeInputPanel;
    public bool tutorialPanelOpen = true;
    public bool titleExitPanelOpen = false;
    public bool roomCodeInputPanelOpen = false;
    public bool isPaused = false;
    private PhotonView photonView;

    // 일시정지를 시작한 플레이어 ID 초기화 한거 같던데
    private int pausedByPlayerId = -1;   

    // 기존에 있던 싱글톤 패턴을 해제하고 씬마다의 UiManager에게 공통 스크립트를 부여, 해당 씬에 있는 버튼만 사용하게 합니다
    // 앞으로 추가되는 버튼들도 이곳에 추가 / 할당한 후 사용하되, 절대 그 씬에 존재하지 않는 버튼을 사용하는 로직을 추가하지 마세요! 바로 오류납니다
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if(SceneManager.GetActiveScene().name == "PrisonScene" || SceneManager.GetActiveScene().name == "HouseScene")
        {
            tutorialPanel.gameObject.SetActive(tutorialPanelOpen);
        }
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "PrisonScene" || SceneManager.GetActiveScene().name == "HouseScene")
        {
            if(tutorialPanelOpen && !isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseTutorialPanel();
            }
            else if(!tutorialPanelOpen && !isPaused && Input.GetKeyDown(KeyCode.Escape))
            {
                pausedByPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
                photonView.RPC("UpdatePauseState", RpcTarget.All, pausedByPlayerId, true);
            }
            else if(!tutorialPanelOpen && isPaused && Input.GetKeyDown(KeyCode.Escape) && pausedByPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                pausedByPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
                photonView.RPC("UpdatePauseState", RpcTarget.All, pausedByPlayerId, false);
            }
        }
        else if(SceneManager.GetActiveScene().name == "TitleScene")
        {
            if(titleExitPanelOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseTitleExitPanel();
            }
        }
        else if(SceneManager.GetActiveScene().name == "LobbyScene")
        {
            if(!roomCodeInputPanelOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("TitleScene");
            }
            else if(roomCodeInputPanelOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseRoomCodeInputPanel();
            }
            
        }
        
    }

    // Prison & House
    public void OpenTutorialPanel()
    {
        if(!tutorialPanelOpen)
        {
            tutorialPanelOpen = true;
            tutorialPanel.gameObject.SetActive(tutorialPanelOpen);    
        }
        else if(tutorialPanelOpen)
        {
            tutorialPanelOpen = false;
            tutorialPanel.gameObject.SetActive(tutorialPanelOpen);
        }
    }

    public void CloseTutorialPanel()
    {
        tutorialPanelOpen = false;
        tutorialPanel.gameObject.SetActive(tutorialPanelOpen);
    }

    [PunRPC]
    void UpdatePauseState(int actorNumber, bool pauseState)
    {
        // 얘가 지금 하고 있는건가? 
        isPaused = pauseState;
        // 
        if (pauseState)
        {
            // 모든 플레이어에서 시간 멈춤
            Time.timeScale = 0;
            // pausedByPlayerId 모든 클라이언트에서 재설정
            pausedByPlayerId = actorNumber;
            // 일시정지가 활성화된 경우
            if(PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            {
                // 자신이 일시정지를 시작한 경우
                pausePanel.gameObject.SetActive(true);
                pauseTextPanel.gameObject.SetActive(false);
            }
            if(PhotonNetwork.LocalPlayer.ActorNumber != actorNumber)
            {
                // 다른 플레이어가 일시정지를 시작한 경우
                pausePanel.gameObject.SetActive(false);
                pauseTextPanel.gameObject.SetActive(true);
            }
        }
        else
        {
            Time.timeScale = 1;
            if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            {
                // 자신이 일시정지를 시작한 경우
                pausePanel.gameObject.SetActive(false);
                pauseTextPanel.gameObject.SetActive(false);
            }
            if(PhotonNetwork.LocalPlayer.ActorNumber != actorNumber)
            {
                // 다른 플레이어가 일시정지를 시작한 경우
                pausePanel.gameObject.SetActive(false);
                pauseTextPanel.gameObject.SetActive(false);
            }
        }
        
    }

public void ClosePausePanel()
    {
        if(isPaused && pausedByPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            // 모든 클라이언트에 상태 동기화
            photonView.RPC("UpdatePauseState", RpcTarget.All, pausedByPlayerId, false);
        }
    }

    public void LoadMapChooseScene()
    {
        PhotonNetwork.LoadLevel("MapChooseScene"); 
    }

    // Title
    public void OpenTitleExitPanel()
    {
        titleExitPanelOpen = true;
        titleExitPanel.gameObject.SetActive(titleExitPanelOpen);
    }

    public void CloseTitleExitPanel()
    {
        titleExitPanelOpen = false;
        titleExitPanel.gameObject.SetActive(false);
    }

    public void OnExitButtonClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
            Debug.Log("게임 종료");
    }
    
    // Lobby
    public void OpenRoomCodeInputPanel()
    {
        roomCodeInputPanelOpen = true;
        roomCodeInputPanel.gameObject.SetActive(roomCodeInputPanelOpen);
    }

    public void CloseRoomCodeInputPanel()
    {
        roomCodeInputPanelOpen = false;
        roomCodeInputPanel.gameObject.SetActive(roomCodeInputPanelOpen);
    }
    

    // TitleScene -> StartButton
    // MapClearPanel -> BackToLobbyButton
    public void LoadLobbyScene()
    {
        PhotonNetwork.LoadLevel("LobbyScene"); 
    } 

}
