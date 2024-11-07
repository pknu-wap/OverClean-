using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrisonDustLeafDestinationAreaScript : MonoBehaviour
{
    // 먼지, 낙엽 총 개수를 참조해서 가져오기 위한 변수
    public PrisonDustLeafPuzzleScript dustLeafPuzzleScript;
    // 먼지, 낙엽 총 개수
    private int dustLeafCount;
    // 지금까지 구역에 들어온 먼지, 낙엽 총 개수 변수
    private int curDustLeafCount = 0;
    // 체크 이미지를 표시하기 위한 UI 이미지
    public Image oImage;

    void Awake()
    {
        // 개수 참조를 위한 스크립트 할당
        dustLeafPuzzleScript = FindObjectOfType<PrisonDustLeafPuzzleScript>();
        // 초기에는 성공 이미지 숨김
        oImage.gameObject.SetActive(false); 
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
        StartCoroutine(puzzleSolveCheck());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 구역에서 나간다면 카운트 감소
        curDustLeafCount--;
    }

    IEnumerator puzzleSolveCheck()
    {
        // 먼지, 낙엽 총 수가 구역에 들어온 먼지, 낙엽 수와 같다면
        if(curDustLeafCount == dustLeafCount)
        {
            // 체크 이미지 표시 코루틴 실행
            yield return StartCoroutine(ShowImage(oImage));
            // 퍼즐 해결 신호 전달
            dustLeafPuzzleScript.puzzleSolved = true;
        }
    }

    IEnumerator ShowImage(Image image)
    {
        // 이미지 표시
        image.gameObject.SetActive(true);
        // 0.5초 대기(코루틴이 매개변수 시간만큼 일시정지됨)
        yield return new WaitForSeconds(0.5f); 
        // 이미지 숨김
        image.gameObject.SetActive(false); 
    }
}
