using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCheckPrefabScript : MonoBehaviour
{
    // 퍼즐매니저 스크립트 참조
    HouseBlockPuzzleScript houseBlockPuzzleScript;   
    void Start()
    {
        // 퍼즐 매니저 할당
        if (houseBlockPuzzleScript == null)
        {
            houseBlockPuzzleScript = FindObjectOfType<HouseBlockPuzzleScript>();
        }
    }

    // 프리팹에 블럭이 덮이면
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            houseBlockPuzzleScript.currentCheckPrefabCount++;
        }
    }

    // 프리팹에서 블럭이 나가면
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            houseBlockPuzzleScript.currentCheckPrefabCount--;
        }
    }
}
