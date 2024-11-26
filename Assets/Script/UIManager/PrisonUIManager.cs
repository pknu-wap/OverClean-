using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;

public class PrisonUIManager : MonoBehaviour
{
    public StageManager stageManager;
    private PhotonView photonView;

    public GoalZoneScript goalZone;

    public GameObject tutorialPanel;
    public GameObject pausePanel;
    public GameObject pauseTextPanel;

    public bool tutorialPanelOpen = true;
    public bool isPaused = false;

    public int pausedByPlayerId = -1;

    private void Start()
    {
        stageManager = FindObjectOfType<StageManager>();
        goalZone = FindAnyObjectByType<GoalZoneScript>();
        photonView = GetComponent<PhotonView>();
        tutorialPanel.SetActive(tutorialPanelOpen);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z 눌려짐 - DestroyPlayers 호출");
            // 파괴할 프리팹 네트워킹매니저에 저장
            NetworkingManager.Instance.InsertDestroyPlayerPrefab();
            photonView.RPC("LoadHouseScene", RpcTarget.MasterClient);
        }

        if (tutorialPanelOpen && !isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTutorialPanel();
        }
        else if (!tutorialPanelOpen && !isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            pausedByPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
            photonView.RPC("UpdatePauseState", RpcTarget.All, pausedByPlayerId, true);
        }
        else if (!tutorialPanelOpen && isPaused && Input.GetKeyDown(KeyCode.Escape) && pausedByPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            pausedByPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
            photonView.RPC("UpdatePauseState", RpcTarget.All, pausedByPlayerId, false);
        }
    }

    // 하우스씬 로드(마스터 클라이언트만 실행하도록 PUNRPC 호출할것!)
    [PunRPC]
    public void LoadHouseScene()
    {
        // 씬 로드
        PhotonNetwork.LoadLevel("HouseScene");
    }

    public void OpenTutorialPanel()
    {
        if (!tutorialPanelOpen)
        {
            tutorialPanelOpen = true;
            tutorialPanel.gameObject.SetActive(tutorialPanelOpen);
        }
        else if (tutorialPanelOpen)
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

    public void ClosePausePanel()
    {
        photonView.RPC("UpdatePauseState", RpcTarget.All, pausedByPlayerId, false);
    }

    [PunRPC]
    void UpdatePauseState(int actorNumber, bool pauseState)
    {
        isPaused = pauseState;
        if (!goalZone.stageClear && pauseState)
        {
            stageManager.isPaused = true;
            stageManager.SetPlayerMovement(false);
            pausedByPlayerId = actorNumber;

            if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            {
                pausePanel.gameObject.SetActive(true);
                pauseTextPanel.gameObject.SetActive(false);
            }
            else
            {
                pausePanel.gameObject.SetActive(false);
                pauseTextPanel.gameObject.SetActive(true);
            }
        }
        else if (!goalZone.stageClear && !pauseState)
        {
            stageManager.isPaused = false;
            stageManager.SetPlayerMovement(true);
            pausedByPlayerId = -1;
            pausePanel.gameObject.SetActive(false);
            pauseTextPanel.gameObject.SetActive(false);
        }

    }

    public void LoadMapChooseScene()
    {
        PhotonNetwork.LoadLevel("MapChooseScene");
    }

    public void LoadLobbyScene()
    {
        photonView.RPC("ExitAndGotoLobbyScene", RpcTarget.All);
    }

    [PunRPC]
    public void ExitAndGotoLobbyScene()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
