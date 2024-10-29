using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // UI 요소를 다루기 위해 필요
using UnityEngine;
// Dotween 네임스페이스 사용
using DG.Tweening; 

public class InGameUIManager : MonoBehaviour
{
    public RectTransform TutorialGuideMessage;
    public Button TutorialCloseButton;

    void Start()
    {
        TutorialCloseButton.onClick.AddListener(CloseTutorialGuideMessage);
    }

    private void CloseTutorialGuideMessage(){

    }
    
    void Update()
    {
        
    }
}
