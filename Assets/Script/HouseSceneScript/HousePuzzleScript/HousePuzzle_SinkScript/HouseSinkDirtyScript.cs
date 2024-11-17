using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSinkDirtyScript : MonoBehaviour
{
    // HouseSinkScript 참조(destroyPrefabCount 참조를 위함)
    private HouseSinkPuzzleScript SinkPuzzleScript;
    // Start is called before the first frame update
    void Start()
    {
        // SinkPuzzleScript를 찾아 참조
        SinkPuzzleScript = FindObjectOfType<HouseSinkPuzzleScript>();
    }

    // 열쇠와의 충돌을 감지하는 메서드
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌체 태그로 스펀지인지 확인
        if (other.CompareTag("Sponge"))
        {
            // 퍼즐 매니저 스크립트의 파괴된 프리팹 개수 증가
            SinkPuzzleScript.destroyPrefabCount++;
            // 이 프리팹 파괴
            Destroy(gameObject);
        }
    }
}
