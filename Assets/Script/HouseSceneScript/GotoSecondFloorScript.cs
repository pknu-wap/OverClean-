using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoSecondFloorScript : MonoBehaviour
{
    // 플레이어가 구역에 들어온다면 true, 나가면 false (태그로 구분)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.Contains("Player"))
        {
            Vector3 secondFloor = new Vector3(-12.82f,28.79f,-2);
            other.transform.position = secondFloor;
        }
    }
}
