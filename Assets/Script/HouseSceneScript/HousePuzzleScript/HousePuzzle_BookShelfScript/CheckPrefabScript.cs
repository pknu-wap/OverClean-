using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPrefabScript : MonoBehaviour
{
    // 이 프리팹의 위치에 있어야 할 책의 이름
    public string thisLocationBook;
    // 퍼즐 매니저 스크립트 참조
    // public HouseBookPuzzleScript houseBookPuzzleScript;
    // 올바른 책이 닿여있는지 확인하는 변수
    public bool isCorrectBook = false;
    // Start is called before the first frame update
    /*void Start()
    {
        houseBookPuzzleScript = FindObjectOfType<HouseBookPuzzleScript>();
    }*/

    // Update is called once per frame
    void Update()
    {

    }

    // 책이 들어왔을 때 체크하는 함수
    void OnTriggerEnter2D(Collider2D other)
    {
        // 먼저 BookPrefabScript 컴포넌트를 가져옵니다.
        BookPrefabScript bookScript = other.GetComponent<BookPrefabScript>();

        // null 체크를 통해 안전하게 처리합니다.
        if (bookScript != null)
        {
            // BookPrefabScript가 존재할 경우 비교 로직 실행
            if (bookScript.thisBookColor == thisLocationBook)
            {
                isCorrectBook = true;
            }
        }
    }

    // 책이 나갔을 때 체크하는 함수
    void OnTriggerExit2D(Collider2D other)
    {
        // 먼저 BookPrefabScript 컴포넌트를 가져옵니다.
        BookPrefabScript bookScript = other.GetComponent<BookPrefabScript>();

        // null 체크를 통해 안전하게 처리합니다.
        if (bookScript != null)
        {
            // BookPrefabScript가 존재할 경우 비교 로직 실행
            if (bookScript.thisBookColor == thisLocationBook)
            {
                isCorrectBook = false;
            }
        }
    }
}
