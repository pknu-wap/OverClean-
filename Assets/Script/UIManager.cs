using UnityEngine;
using UnityEngine.UI;  
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Button switchButton;
    public RectTransform tutorialPanel;
    public RectTransform pausePanel;
    public bool tutorialPanelOpen = true;
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
        // TutorialPanel이 열려있고, Pause가 비활성화 되어있을때, ESC 눌렀을때 TutorialPanel 닫히게 하고 싶음.
        if(tutorialPanelOpen && !pause && Input.GetKeyDown(KeyCode.Escape))
        {
            onTutorialCloseButtonClicked();
        }
        // tutorialPanel이 닫혀있고, Pause가 비활성화 되었있을때, ESC를 누르면 일시정지 창이 나옴.
        else if(!tutorialPanelOpen && !pause && Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscButtonClicked();
        }
        // tutorialPanel이 닫혀있고, Pause가 활성화 되어있을때, ESC를 누르면 일시정지 창이 닫힘.
        else if(!tutorialPanelOpen && pause && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePausePanel();
        }
        
    }
    public void onTutorialOpenButtonClicked()
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

    public void onTutorialCloseButtonClicked()
    {
        tutorialPanelOpen = false;
        tutorialPanel.gameObject.SetActive(tutorialPanelOpen);
    }

        public void ClosePausePanel()
    {
        pause = false;
        Time.timeScale = 1;
        pausePanel.gameObject.SetActive(false);
    }

    public void OnEscButtonClicked()
    {
        pause = true;
        Time.timeScale = 0;
        pausePanel.gameObject.SetActive(true);
    }
    
}
