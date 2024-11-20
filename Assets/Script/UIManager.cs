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
    public bool tutorialPanelOpen = true;
    public bool titleExitPanelOpen = false;
    public bool pause = false;

    private void Start()
    {
        tutorialPanel.gameObject.SetActive(tutorialPanelOpen);
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
            // TutorialPanel이 열려있고, Pause가 비활성화 되어있을때, ESC 눌렀을때 TutorialPanel 닫히게 하고 싶음.
            if(tutorialPanelOpen && !pause && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseTutorialPanel();
            }
            // TutorialPanel이 닫혀있고, Pause가 비활성화 되었있을때, ESC를 누르면 일시정지 창이 나옴.
            else if(!tutorialPanelOpen && !pause && Input.GetKeyDown(KeyCode.Escape))
            {
                OpenEscPanel();
            }
            // TutorialPanel이 닫혀있고, Pause가 활성화 되어있을때, ESC를 누르면 일시정지 창이 닫힘.
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
        
    }
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
}
