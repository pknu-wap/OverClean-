using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonDustLeafDestinationAreaScript : MonoBehaviour
{
    // 먼지, 낙엽 총 개수를 참조해서 가져오기 위한 변수
    public PrisonDustLeafPuzzleScript dustLeafPuzzleScript;
    // 먼지, 낙엽 총 개수
    private int dustLeafCount;
    // 지금까지 구역에 들어온 먼지, 낙엽 총 개수 변수
    private int curDustLeafCount = 0;

    void Awake()
    {
        // 개수 참조를 위한 스크립트 할당
        dustLeafPuzzleScript = FindObjectOfType<PrisonDustLeafPuzzleScript>();
    }

    void Start()
    {
        // 개수 참조
        dustLeafCount = dustLeafPuzzleScript.ansCount;
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 구역에 들어온다면 카운트 증가
        curDustLeafCount++;
        // 전부 다 구역에 들어왔는지 체크
        puzzleSolveCheck();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 구역에서 나간다면 카운트 감소
        curDustLeafCount--;
    }

    private void puzzleSolveCheck()
    {
        // 먼지, 낙엽 총 수가 구역에 들어온 먼지, 낙엽 수와 같다면
        if(curDustLeafCount == dustLeafCount)
        {
            // 퍼즐 해결 신호 전달
            dustLeafPuzzleScript.puzzleSolved = true;
        }
    }
}
