using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HouseFinalDoorPuzzleScript : MonoBehaviour
{
    // 문 비밀번호 변수
    public int password;
    // 비밀번호 UI 텍스트 참조
    public TMP_Text passwordUi;
    // 입력받는 UI 참조
    public TMP_InputField inputField;
    // o,x 이미지
    public Image oImage;
    public Image xImage;
    void Start()
    {
        // 패스워드 초기화
        password = Random.Range(1000,9999);
        // 패스워드 ui 텍스트를 패스워드로 갱신
        passwordUi.text = password.ToString();
        // 두꺼비집 퍼즐 해결 여부에 따른 ui 활/비활성화
        if(StageManager.fusePuzzleSolved)
        {
            passwordUi.gameObject.SetActive(true);
        }
        else
        {
            passwordUi.gameObject.SetActive(false);
        }
        // UI 이미지 비활성화
        oImage.gameObject.SetActive(false);
        xImage.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        StartCoroutine(CheckPassword());
    }

    IEnumerator CheckPassword()
    {
        // 입력받은 값을 얻어옴
        string input = inputField.text;
        if (!string.IsNullOrEmpty(input) && password == int.Parse(input))
        {
            // 정답인 경우, 체크 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(oImage));
            // puzzlesuccess 호출
            PuzzleManager.instance.PuzzleSuccess();
            // 씬 닫기
            SceneManager.UnloadSceneAsync("HouseFinalDoorPuzzleScene");
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
