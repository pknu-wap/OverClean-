using UnityEngine;
using UnityEngine.UI;  
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public Button switchButton;
    public RectTransform tutorialPanel;
    public bool tutorialPanelOpen = true;

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

    public void onTutorialOpenButtonClicked()
    {
        if(tutorialPanelOpen == false)
        {
            tutorialPanelOpen = true;
            tutorialPanel.gameObject.SetActive(tutorialPanelOpen);    
        }
        else if(tutorialPanelOpen == true)
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
}
