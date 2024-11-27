using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoFirstFloorScript : MonoBehaviour
{
    // 플레이어가 구역에 들어온다면 true, 나가면 false (태그로 구분)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.Contains("Player"))
        {
            Vector3 firstFloor = new Vector3(-1.47f,-4.37f,-2);
            other.transform.position = firstFloor;
        }
    }
}
