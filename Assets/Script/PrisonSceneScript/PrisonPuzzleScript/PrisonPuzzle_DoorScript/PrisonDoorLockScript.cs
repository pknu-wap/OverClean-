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
    // 타임아웃 상태 플래그(행동제한을 제어하기 위한 변수)
    public bool isTimeout = false; 
    // 맞았을 때 체크 이미지를 표시하기 위한 UI 이미지
    public Image oImage;
    // 틀렸을 때 X 이미지를 표시하기 위한 UI 이미지
    public Image xImage;

    void Awake()
    {
        // 퍼즐 매니저 객체를 씬에서 찾아서 참조
        doorPuzzleManager = FindObjectOfType<PrisonDoorPuzzleScript>();
        // "TimeBar"라는 이름을 가진 오브젝트의 Slider 컴포넌트를 참조
        timeBar = GameObject.Find("TimeBar").GetComponent<Slider>();
        // O,X 이미지 참조
        oImage = GameObject.Find("CorrectAnswerImage").GetComponent<Image>();
        xImage = GameObject.Find("WrongAnswerImage").GetComponent<Image>();
        
    }

    void Start()
    {
        // 타임바의 초기값을 최대값(1.0)으로 설정
        timeBar.value = 1.0f;
        // 타임바 UI를 비활성화하여 화면에 보이지 않게 설정
        timeBar.gameObject.SetActive(false);
        // O,X 이미지를 초기에는 숨김
        oImage.gameObject.SetActive(false);
        xImage.gameObject.SetActive(false);
    }

    // 열쇠와의 충돌을 감지하는 메서드
    private void OnTriggerEnter2D(Collider2D other)
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if(audioManager != null)
        {
            audioManager.PlayRandomPrisonDoorPuzzleKeySound();
        }
        // 타임바를 최대값(1.0)으로 초기화
        timeBar.value = 1.0f;
        // 타임바 UI를 활성화하여 화면에 표시
        timeBar.gameObject.SetActive(true);
        // 체크(타임아웃) 시작
        isTimeout = true;

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
            // 프레임마다 경과 시간에 deltaTime을 추가하여 checkTime보다 작은 시점에만 while문 작동
            elapsedTime += Time.deltaTime;
            // 타임바의 value를 (1 - 경과 시간 / 체크 시간)으로 설정하여 줄어드는 효과 구현
            timeBar.value = 1 - (elapsedTime / checkTime);
            // 다음 프레임까지 대기(코루틴 문법, while과 같이 쓰면 void Update()랑 비슷한 효과)
            yield return null;
        }

        // 열쇠가 정답인지 확인
        if (other.GetComponent<PrisonDoorKeyScript>().isAnsKey)
        {
            // 정답인 경우, 체크 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(oImage));
            // 퍼즐 매니저에 퍼즐 성공 상태 전달
            doorPuzzleManager.puzzleSolved = true;
        }
        else
        {
            // 오답인 경우, 자물쇠 진동 코루틴 실행 및 X 이미지 표시 코루틴 실행
            StartCoroutine(ShakeLock());
            yield return StartCoroutine(ShowImage(xImage));
        }

        // 타임아웃 종료
        isTimeout = false; 
        // 체크 타임아웃이 끝나도 드래그가 유지되던 버그 해결(키 스크립트에서 명시적으로 드래그 해제)
        other.GetComponent<PrisonDoorKeyScript>().isDragging = false;
        // 타임바 UI를 비활성화하여 화면에서 사라지게 설정
        timeBar.gameObject.SetActive(false);
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

    // 자물쇠를 진동시키는 코루틴
    IEnumerator ShakeLock()
    {
        // 자물쇠의 원래 위치 저장
        Vector3 originalPos = transform.position; 
        // 진동 지속 시간 고정값
        float shakeDuration = 0.5f; 
        // 진동 강도
        float shakeMagnitude = 0.1f; 
        // 진동 지속시간 변수
        float elapsed = 0f;
        // 고정된 시간만큼 진동(while - yield return null은 void update()와 비슷한 효과)
        while (elapsed < shakeDuration)
        {
            // 진동 지속시간 변수에 deltaTime을 더하면서 shakeDuration보다 작은 시점까지만 지속
            elapsed += Time.deltaTime;
            // 진동 강도 * -1 ~ 1 값으로 x축 추가값 결정
            float xPosAddVal = Random.Range(-1, 1) * shakeMagnitude;
            // 원래 위치값에 x축 추가값을 더해 새로운 위치 설정
            transform.position = originalPos + new Vector3(xPosAddVal, 0, 0);
            yield return null;
        }
        // 원래 위치로 복구
        transform.position = originalPos; 
    }
}
