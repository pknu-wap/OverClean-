using UnityEngine;
using UnityEngine.UI;  
using UnityEngine.SceneManagement;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Button switchButton;
    public RectTransform tutorialPanel;
    public RectTransform pausePanel;
    public RectTransform titleExitPanel;
    public RectTransform roomCodeInputPanel;
    public bool tutorialPanelOpen = true;
    public bool titleExitPanelOpen = false;
    public bool roomCodeInputPanelOpen = false;
    public bool pause = false;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "PrisonScene" || SceneManager.GetActiveScene().name == "HouseScene")
        {
            tutorialPanel.gameObject.SetActive(tutorialPanelOpen);
        }
        else 
        {
            return;
        }
        // NetworkingManager가 로드될 때까지 기다린 후, 버튼 이벤트 연결
        if (PhotonNetwork.IsConnected)
        {
            if (NetworkingManager.Instance != null)
            {
                switchButton.onClick.AddListener(NetworkingManager.Instance.SwitchPlayers);
            }
        }
        else
        {
            // Photon 서버에 연결되어 있지 않을 때의 처리 로직
            Debug.LogWarning("Photon 서버에 연결되어 있지 않음.");
        }
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "PrisonScene" || SceneManager.GetActiveScene().name == "HouseScene")
        {
            if(tutorialPanelOpen && !pause && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseTutorialPanel();
            }
            else if(!tutorialPanelOpen && !pause && Input.GetKeyDown(KeyCode.Escape))
            {
                OpenEscPanel();
            }
            else if(!tutorialPanelOpen && pause && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseEscPanel();
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

    public void OpenEscPanel()
    {
        pause = true;
        Time.timeScale = 0;
        pausePanel.gameObject.SetActive(true);
    }
    
    public void CloseEscPanel()
    {
        pause = false;
        Time.timeScale = 1;
        pausePanel.gameObject.SetActive(false);
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
        SceneManager.LoadScene("LobbyScene");
    } 

}
