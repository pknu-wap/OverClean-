using UnityEngine;
using Photon.Pun;

public class CameraManager : MonoBehaviourPunCallbacks
{
    public Camera player1Camera;
    public Camera player2Camera;
    public GameObject player1;
    public GameObject player2;

    void Start()
    {
        AssignCamera();
    }

    void AssignCamera()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 로컬 플레이어가 플레이어 1인 경우
            if (player1.GetComponent<PhotonView>().IsMine)
            {
                player1Camera.gameObject.SetActive(true);
                player2Camera.gameObject.SetActive(false);
            }
        }
        else
        {
            // 로컬 플레이어가 플레이어 2인 경우
            if (player2.GetComponent<PhotonView>().IsMine)
            {
                player1Camera.gameObject.SetActive(false);
                player2Camera.gameObject.SetActive(true);
            }
        }
    }
}
