using System.Collections;
// UI 요소를 다루기 위해 필요
using UnityEngine.UI; 
using UnityEngine;
// Dotween 네임스페이스 사용
using DG.Tweening; 

public class InGameUIManager : MonoBehaviour
{
    // UI 요소 연결하기
    public RectTransform TutorialGuideMessage;
    public Button TutorialGuideShowButton;
    public Button TutorialCloseButton;

    private Vector3 originalPosition;

    void Start()
    {
        // 모달 창의 원래 위치를 저장합니다.
        originalPosition = TutorialGuideMessage.anchoredPosition;

        // 모달 창을 처음에는 비활성화
        TutorialGuideMessage.gameObject.SetActive(false);

        // 버튼 클릭 이벤트 설정
        TutorialGuideShowButton.onClick.AddListener(ShowTutorialGuideMessage);
        TutorialCloseButton.onClick.AddListener(CloseTutorialGuideMessage);
    }

    // 모달 창 열기
    public void ShowTutorialGuideMessage()
    {
        Debug.Log("튜토리얼 창 오픈");
        TutorialGuideMessage.gameObject.SetActive(true);  // 모달 활성화
        
        // 시작 위치를 화면 바깥쪽으로 설정
        TutorialGuideMessage.anchoredPosition = new Vector2(-Screen.width, Screen.height);
        Debug.Log("시작 위치를 화면 바깥쪽으로 설정");

        // DOTween으로 부드럽게 원래 위치로 이동
        TutorialGuideMessage.DOAnchorPos(originalPosition, 0.6f)
            .SetEase(Ease.OutBack);  // 팡 하고 나타나는 느낌
        Debug.Log("DOTween으로 부드럽게 원래 위치로 이동");
    }

    // 모달 창 닫기
    private void CloseTutorialGuideMessage()
    {
        // DOTween으로 모달을 화면 바깥쪽으로 이동
        TutorialGuideMessage.DOAnchorPos(new Vector2(-Screen.width, Screen.height), 0.6f)
            .SetEase(Ease.InBack)  // 부드럽게 사라지는 느낌
            .OnComplete(() => TutorialGuideMessage.gameObject.SetActive(false));  // 이동 후 비활성화
    }
    void Update()
    {
        // 모달이 활성화된 상태를 매 프레임 모니터링
        if (TutorialGuideMessage.gameObject.activeSelf)
        {
            Debug.Log("모달 창이 열려 있습니다.");
        }
        
        // 버튼 클릭 가능 여부를 매 프레임 확인
        if (TutorialGuideShowButton.IsActive() && !TutorialGuideShowButton.interactable)
        {
            Debug.LogWarning("TutorialGuideShowButton이 비활성화되어 클릭할 수 없습니다.");
        }

    }
}
