using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBlockGridScript : MonoBehaviour
{
    // 상단 좌표 (맨 윗값)
    float xStart = -38.65f;
    float yStart = -43.28f;

    // 하단 좌표 (맨 밑값)
    float xEnd = -45.37f;
    float yEnd = -56.73f;

    // 가로, 세로 개수
    int cols = 5;
    int rows = 9;

    // 체크 지점 프리팹 변수
    public GameObject checkPrefab;
    void Start()
    {
        // 각 칸의 너비와 높이 계산
        float xStep = (xEnd - xStart) / (cols - 1);
        float yStep = (yEnd - yStart) / (rows - 1);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                float x = xStart + j * xStep;
                float y = yStart + i * yStep;
                
                // 좌표 출력 (또는 원하는 작업 수행)
                Instantiate(checkPrefab, new Vector3(x,y,0), Quaternion.identity);
            }
        }
    }
}
