using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrisonPipePuzzleScript : MonoBehaviour
{
    // Grid Layout Group을 포함한 빈 오브젝트
    public GameObject pipeGrid;
    // 파이프 모양 배열 (일자, L자, T자)
    public GameObject[] pipeShapes;
    
    private int gridWidth = 10;
    private int gridHeight = 5;

