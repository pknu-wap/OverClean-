using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    public GameObject titleExitPanel;
    public bool titleExitPanelOpen = false;

    private void Update()
    {
        if(titleExitPanelOpen == true && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTitleExitPanel();
        }
    }
    
    public void OpenTitleExitPanel()
    {
        titleExitPanelOpen = true;
        titleExitPanel.SetActive(titleExitPanelOpen);
    }

    public void CloseTitleExitPanel()
    {
        titleExitPanelOpen = false;
        titleExitPanel.SetActive(titleExitPanelOpen);
    }

    public void OnExitButtonClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}