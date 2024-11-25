using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HouseUIManager : MonoBehaviour
{
    public StageManager stageManager;
    private PhotonView photonView;

    public GameObject tutorialPanel;
    public GameObject pausePanel;
    public GameObject pauseTextPanel;

    public bool tutorialPanelOpen = true;
    public bool isPaused = false;

    public int pausedByPlayerId = -1;   

    private void Start()
    {
        stageManager = FindObjectOfType<StageManager>();
        photonView = GetComponent<PhotonView>();
        tutorialPanel.SetActive(tutorialPanelOpen);
    }

    private void Update()
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

    public void ClosePausePanel()
    {
        photonView.RPC("UpdatePauseState", RpcTarget.All, pausedByPlayerId, false);
    }

    [PunRPC]
    void UpdatePauseState(int actorNumber, bool pauseState)
    {
        isPaused = pauseState;
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
        else if(!pauseState)
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