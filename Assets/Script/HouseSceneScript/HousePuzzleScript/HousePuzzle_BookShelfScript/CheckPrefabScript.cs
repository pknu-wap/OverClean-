using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPrefabScript : MonoBehaviour
{
    // 이 프리팹의 위치에 있어야 할 책의 이름
    public string thisLocationBook;
    // 퍼즐 매니저 스크립트 참조
    public HouseBookPuzzleScript houseBookPuzzleScript;
    // 올바른 책이 닿여있는지 확인하는 변수
    public bool isCorrectBook = false;
    // Start is called before the first frame update
    void Start()
    {
        houseBookPuzzleScript = FindObjectOfType<HouseBookPuzzleScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 플레이어가 구역에 들어왔을 때 처리
    void OnTriggerEnter2D(Collider2D other)
    {
    }
}
