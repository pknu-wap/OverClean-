using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenEvent : MonoBehaviour
{
    public void SceneMove()
    {
        // GameLobby 씬으로 이동
        SceneManager.LoadScene("GameLobby");
    }

    public void Exit()
    {
        // #if 키워드를 사용하여 플랫폼별로 다르게 실행하는 함수.
        // 에디터와 프로그램 실행을 구분.
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // 어플리케이션 종료
            Application.Quit();
        #endif
    }
}