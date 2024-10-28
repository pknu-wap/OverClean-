using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PrisonDoorLockScript : MonoBehaviour
{
    // 퍼즐 매니저를 참조하는 변수
    public PrisonDoorPuzzleScript doorPuzzleManager;
    // 타임바 UI 슬라이더
    public Slider timeBar; 
    // 체크에 걸리는 시간 (타임바가 감소하는 시간)
    private float checkTime = 1.5f;

    // 게임 시작 시 퍼즐 매니저와 타임바 초기화
    void Awake()
    {
        // 퍼즐 매니저 객체를 씬에서 찾아서 참조
        doorPuzzleManager = FindObjectOfType<PrisonDoorPuzzleScript>();
        // "TimeBar"라는 이름을 가진 오브젝트의 Slider 컴포넌트를 참조
        timeBar = GameObject.Find("TimeBar").GetComponent<Slider>();
    }

    // 타임바 초기값을 설정하고 비활성화
    void Start()
    {
        // 타임바의 초기값을 최대값(1.0)으로 설정
        timeBar.value = 1.0f;
        // 타임바 UI를 비활성화하여 화면에 보이지 않게 설정
        timeBar.gameObject.SetActive(false);
    }

    // 열쇠와의 충돌을 감지하는 메서드
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 타임바를 최대값(1.0)으로 초기화
        timeBar.value = 1.0f;
        // 타임바 UI를 활성화하여 화면에 표시
        timeBar.gameObject.SetActive(true);
        // CheckTime 코루틴을 시작하여 열쇠 확인 절차 실행
        StartCoroutine(CheckTime(other));
    }

    // 타임바를 줄이며 열쇠를 확인하는 코루틴
    IEnumerator CheckTime(Collider2D other)
    {
        // 경과 시간을 저장하는 변수 초기화
        float elapsedTime = 0f;

        // 1.5초 동안 반복하여 타임바를 줄임
        while (elapsedTime < checkTime)
        {
            // 프레임마다 경과 시간에 deltaTime을 추가
            elapsedTime += Time.deltaTime;
            // 타임바의 value를 (1 - 경과 시간 / 체크 시간)으로 설정하여 줄어드는 효과 구현
            timeBar.value = 1 - (elapsedTime / checkTime);
            // 다음 프레임까지 대기
            yield return null;
        }

        // 열쇠가 정답인지 확인
        if (other.GetComponent<PrisonDoorKeyScript>().isAnsKey)
        {
            // 정답 열쇠일 경우 퍼즐 성공 로그 출력
            Debug.Log("맞는 열쇠, 퍼즐 해결 성공");
            // 퍼즐 매니저에 퍼즐 성공 상태 전달
            doorPuzzleManager.puzzleSolved = true;
        }
        else
        {
            // 오답 열쇠일 경우 오답 로그 출력
            Debug.Log("틀린 열쇠");
        }

        // 타임바 UI를 비활성화하여 화면에서 사라지게 설정
        timeBar.gameObject.SetActive(false);
    }
}
