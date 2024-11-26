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
        isPaused = PauseManager.Instance.isPaused;
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z 눌려짐 - DestroyPlayers 호출");
            // 파괴할 프리팹 네트워킹매니저에 저장
            NetworkingManager.Instance.InsertDestroyPlayerPrefab();
            photonView.RPC("LoadHouseScene", RpcTarget.MasterClient);
        }

        if(PauseManager.Instance.isTransitioningPauseState)
        {
            return;
        }

        if (tutorialPanelOpen && !isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTutorialPanel();
        }
        else if (!tutorialPanelOpen && !isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            pausedByPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
            PauseManager.Instance.isTransitioningPauseState = true;
            photonView.RPC("UpdatePauseState", RpcTarget.All, pausedByPlayerId, true);
        }
        else if (!tutorialPanelOpen && isPaused && Input.GetKeyDown(KeyCode.Escape) && pausedByPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            pausedByPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
            PauseManager.Instance.isTransitioningPauseState = true;
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
        if (!goalZone.stageClear)
        {
        isPaused = pauseState;
        PauseManager.Instance.isPaused = pauseState;
        if (pauseState)
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
        else if (!pauseState)
        {
            stageManager.isPaused = false;
            stageManager.SetPlayerMovement(true);
            pausedByPlayerId = -1;
            pausePanel.gameObject.SetActive(false);
            pauseTextPanel.gameObject.SetActive(false);
        }
        StartCoroutine(ResetTransitionState());
        }

    }
    private IEnumerator ResetTransitionState()
    {
        yield return new WaitForSeconds(0.1f); // 딜레이 추가
        PauseManager.Instance.isTransitioningPauseState = false; // 상태 전환 완료
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
