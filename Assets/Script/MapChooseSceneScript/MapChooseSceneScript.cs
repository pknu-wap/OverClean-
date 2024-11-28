using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MapChooseSceneScript : MonoBehaviour
{
    public float player1StartX;
    public float player1StartY;
    public float player1StartZ;
    public float player2StartX;
    public float player2StartY;
    public float player2StartZ;

    void Awake()
    {
        // 기존에 있던 프리팹 삭제
        NetworkingManager.Instance.DestroyPlayerPrefabList();
        // 플레이어 생성 및 할당
        if (PhotonNetwork.IsConnected)
        {
            // 데이브 할당 / 생성
            if (PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString() == "Dave")
            {
                PhotonNetwork.Instantiate("Player1", new Vector3(player1StartX, player1StartY, player1StartZ), Quaternion.identity);
            }
            // 매튜 할당 / 생성
            else
            {
                PhotonNetwork.Instantiate("Player2", new Vector3(player2StartX, player2StartY, player2StartZ), Quaternion.identity);
            }
        }
    }
}
