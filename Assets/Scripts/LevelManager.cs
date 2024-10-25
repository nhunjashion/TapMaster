using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [Header("Level Detail")]
    [SerializeField] private int currentLevel;
    public int CurrentLevel { get => currentLevel; }
    [SerializeField] int levelTarget;
    public int LevelTarget { get => levelTarget; set => levelTarget = value; }

    [SerializeField] private int countX;
    public int CountX { get => countX; }
    [SerializeField] private int countY;
    public int CountY { get => countY; }
    [SerializeField] int countZ;
    public int CountZ { get => countZ; }
    public TextMeshProUGUI levelTxt;

    [SerializeField] bool isWin = false;

    private void Start()
    {
        Instance = this;
        currentLevel = 0;
        SetTextLevel();
        CreateLevel();
    }
    public void SetTextLevel()
    {
        int level = currentLevel;
        levelTxt.text = "Level : " + level.ToString();
    }
    public void CreateLevel()
    {
        CreateLevelData();
        CubeCtrl.Instance.SpawnCube();
    }

    public void CreateLevelData()
    {
        if(currentLevel == 0)
        {
            countX = 6;
            countY = 6;
            countZ = 6;
        }
        else if(currentLevel <= 10)
        {
            countX = RandomValue(1, 3);
            countY = RandomValue(1, 3);
            countZ = RandomValue(1, 3);
        }
        else
        {
            countX = RandomValue(3, 6);
            countY = RandomValue(3, 6);
            countZ = RandomValue(3, 6);
        }

        levelTarget = countX * countY * countZ; 
    }

    public int RandomValue(int start, int end)
    {
        return Random.Range(start, end);
    }


    public void CheckWin()
    {
        if(levelTarget <= 0)
            isWin = true;
        if (isWin)
        {
            GameSceneManager.Instance.popupWin.SetActive(true);
            currentLevel++;
            isWin = false;
        }
        else return;
    }


}
