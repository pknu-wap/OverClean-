using UnityEngine;
using UnityEngine.UI;  
using UnityEngine.SceneManagement;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public RectTransform tutorialPanel;
    public RectTransform pausePanel;
    public RectTransform titleExitPanel;
    public RectTransform roomCodeInputPanel;
    public bool tutorialPanelOpen = true;
    public bool titleExitPanelOpen = false;
    public bool roomCodeInputPanelOpen = false;
    public bool pause = false;

     private void Awake()
    {
        // 싱글톤 패턴 구현: UIManager가 중복되지 않도록 설정
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            // 씬 전환 시에도 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
    }
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
