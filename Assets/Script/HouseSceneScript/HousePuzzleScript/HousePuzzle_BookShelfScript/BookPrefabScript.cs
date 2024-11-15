using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookPrefabScript : MonoBehaviour
{
    // 책 색상 변수
    public string thisBookColor;
    // 스프라이트 렌더러 컴포넌트 참조
    private SpriteRenderer sr;
    void Start()
    {
        // SpriteRenderer 컴포넌트 가져오기
        sr = GetComponent<SpriteRenderer>();

        // 책 색상에 따라 스프라이트 색상 설정
        UpdateBookColor();
    }

    // 책 색상에 따라 스프라이트 색상 변경
    private void UpdateBookColor()
    {
        switch (thisBookColor.ToLower()) // 대소문자 구분 없이 처리
        {
            case "빨강":
                sr.color = Color.red;
                break;
            case "주황":
                sr.color = new Color(1.0f, 0.5f, 0.0f); // RGB 값으로 오렌지 색상
                break;
            case "노랑":
                sr.color = Color.yellow;
                break;
            case "초록":
                sr.color = Color.green;
                break;
            case "파랑":
                sr.color = Color.blue;
                break;
            case "보라":
                sr.color = new Color(0.5f, 0.0f, 1.0f); // RGB 값으로 보라색
                break;
            case "검정":
                sr.color = Color.black;
                break;
            default:
                sr.color = Color.white; // 기본값: 흰색
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
