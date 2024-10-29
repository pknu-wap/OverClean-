using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

using DG.Tweening; 

public class InGame : MonoBehaviour
{
    // 모달 창의 RectTransform
    public RectTransform TutorialMessage;  

    // 상단바 아이콘 버튼
    public Button TutorialGuideShowButton;        

    // 튜토리얼 모달창 내 뒤로가기 버튼
    public Button TutorialGudeCloseButton;

    // 시작 위치 (화면 밖)
    private Vector2 hiddenPosition = new Vector2(-500, 500);  

    // 최종 위치 (화면 안)
    private Vector2 shownPosition = new Vector2(470, 0);     

     // 초기 모달 상태   
    private bool isModalVisible = false; 

    private void Start()
    {
        // 모달 창을 숨긴 위치로 초기화
        TutorialMessage.anchoredPosition = hiddenPosition;

        // 아이콘 버튼에 클릭 이벤트 연결
        TutorialGuideShowButton.onClick.AddListener(ShowTutorialGuideMessage);
        TutorialGudeCloseButton.onClick.AddListener(CloseTutorialGuideMessage);
        
    }

    public void ShowTutorialGuideMessage()
    {
        // 모달 숨기기 애니메이션
        TutorialMessage.DOAnchorPos(hiddenPosition, 0.75f).SetEase(Ease.InBack);

        // 모달 상태 반전
        isModalVisible = !isModalVisible;
    }

    public void CloseTutorialGuideMessage()
    {
        // 모달 숨기기 애니메이션
        TutorialMessage.DOAnchorPos(hiddenPosition, 0.75f).SetEase(Ease.InBack);

        // 모달 상태 반전
        isModalVisible = !isModalVisible;
    }
}
