using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseShelfPuzzleDustScript : MonoBehaviour
{
    // HouseShelfScript 참조(destroyPrefabCount 참조를 위함)
    private HouseShelfPuzzleScript ShelfPuzzleScript;

    void Awake()
    {
        // ShelfPuzzleScript를 찾아 참조
        ShelfPuzzleScript = FindObjectOfType<HouseShelfPuzzleScript>();
    }

    // 열쇠와 충돌 감지
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌체 태그로 수건인지 확인
        if (other.CompareTag("Towel"))
        {
            // 퍼즐 매니저 스크립트의 파괴된 프리팹 개수 증가
            ShelfPuzzleScript.destroyPrefabCount++;
            // 이 프리팹 파괴
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
}
