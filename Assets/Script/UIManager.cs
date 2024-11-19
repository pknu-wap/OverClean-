using UnityEngine;
using UnityEngine.UI;  
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public Button switchButton;
    public RectTransform TutorialPanel;

    private void Start()
    {
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
        TutorialPanel.gameObject.SetActive(true);   
    }

    public void onTutorialCloseButtonClicked()
    {
        TutorialPanel.gameObject.SetActive(false);
    }
}
