using UnityEngine;
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
        if (!goalZone.stageClear && pauseState)
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
        if(BGMManager.instance != null)
        {
            BGMManager.instance.SetMusicForScene("MapChooseScene"); 
        }
        Debug.Log("BGMManager == null");
        PhotonNetwork.LoadLevel("MapChooseScene");
    }

    public void LoadLobbyScene()
    {
        if(BGMManager.instance != null)
        {
            BGMManager.instance.SetMusicForScene("MapChooseScene"); 
        }
        Debug.Log("BGMManager == null");
        photonView.RPC("ExitAndGotoLobbyScene", RpcTarget.All);
    }

    [PunRPC]
    public void ExitAndGotoLobbyScene()
    {
        if(BGMManager.instance != null)
        {
            BGMManager.instance.SetMusicForScene("MapChooseScene"); 
        }
        Debug.Log("BGMManager == null");
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
