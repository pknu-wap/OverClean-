using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HouseFuseBoxPuzzleScript : MonoBehaviour
{
    // 돌아가는 퓨즈 오브젝트 리스트
    public List<GameObject> fuseList = new List<GameObject>();
    // 클리어 조건 확인 버튼
    public Button clearCheckButton;
    // o,x 이미지
    public Image oImage;
    public Image xImage;
    void Start()
    {
        // O,X 이미지를 초기에는 숨김
        oImage.gameObject.SetActive(false);
        xImage.gameObject.SetActive(false);
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
            // 체크 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(oImage));
            // puzzlesuccess 호출
            PuzzleManager.instance.PuzzleSuccess();
            // 제거할 프리팹 없으니 바로 씬 닫기
            SceneManager.UnloadSceneAsync("HouseFuseBoxPuzzleScene");
        }
        else
        {
            // 오답인 경우 X 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(xImage));
        }
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
