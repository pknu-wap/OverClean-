using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class HouseFuseBoxPuzzleScript : MonoBehaviourPun
{
    // 돌아가는 퓨즈 오브젝트 리스트
    public List<GameObject> fuseList = new List<GameObject>();
    // 초기 퓨즈 각도 저장용 리스트
    public List<float> fuseAngleList = new List<float>();
    // 클리어 조건 확인 버튼
    public Button clearCheckButton;
    // o,x 이미지
    public Image oImage;
    public Image xImage;
    // 마스터 클라이언트 퍼즐 초기화 완료 상태
    public bool isMasterPuzzleReady = false;
    // 나머지 클라이언트 퍼즐 초기화 완료 상태
    public bool isElsePuzzleReady = false;
    void Start()
    {
        // O,X 이미지를 초기에는 숨김
        oImage.gameObject.SetActive(false);
        xImage.gameObject.SetActive(false);
        // 마스터 클라이언트만 퍼즐 생성
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["Character"].ToString() + "가 MasterClient로써 퍼즐 생성.");
            foreach(GameObject fuse in fuseList)
            {
                // 초기 회전 각도 랜덤하게 설정
                float targetAngle = Random.Range(0f, 360f);
                photonView.RPC("SetRotate", RpcTarget.All, fuse, targetAngle);
            }
        }
    }

    [PunRPC]
    void SetRotate(GameObject fuse, float angle)
    {
        fuse.GetComponent<HouseFuseScript>().GetAndSetState(angle);
    }

    // 코루틴 시작 함수(버튼과 연결)
    public void StartCheckClear()
    {
        StartCoroutine(CheckClear());
    }

    IEnumerator CheckClear()
    {
        // 전부 연결됐는지 확인하는 함수
        bool allConnected = true;
        for(int i = 0; i < fuseList.Count; i++)
        {
            if(!fuseList[i].GetComponent<HouseFuseScript>().isConnect)
            {
                allConnected = false;
                break;
            }
        }
        if (allConnected)
        {
            photonView.RPC("ShowImageTrigger", RpcTarget.All, oImage);
            photonView.RPC("PuzzleSuccess", RpcTarget.All);
        }
        else
        {
            photonView.RPC("ShowImageTrigger", RpcTarget.All, xImage);
        }
        yield return null;
    }

    [PunRPC]
    // 퍼즐 성공 시 호출되는 함수
    public void PuzzleSuccess()
    {
        Debug.Log("성공, 씬이 종료됩니다.");
        PuzzleManager.instance.PuzzleSuccess();
        photonView.RPC("ClosePuzzleScene", RpcTarget.All);
    }

    [PunRPC]
    // 씬 닫기 함수
    void ClosePuzzleScene()
    {
        // 현재 씬 닫기
        SceneManager.UnloadSceneAsync("HouseFuseBoxPuzzleScene");
        Debug.Log("씬이 닫혔습니다.");
    }

    [PunRPC]
    public void ShowImageTrigger(Image image)
    {
        StartCoroutine(ShowImage(image));
    }

    // 정답, 오답 유무에 따른 이미지 표시 코루틴
    IEnumerator ShowImage(Image image)
    {
        // 이미지 표시
        image.gameObject.SetActive(true);
        // 0.5초 대기(코루틴이 매개변수 시간만큼 일시정지됨)
        yield return new WaitForSeconds(0.5f); 
        // 이미지 숨김
        image.gameObject.SetActive(false); 
    }
}
