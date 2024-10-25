using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; set; }
    public GameObject popupWin;
    public GameObject popupLose;
    public Button deleteBtn;
    private void Start()
    {
        Instance = this;
    }
    public void ResetMap()
    {
        LevelManager.Instance.CreateLevel();
    }

    public void OnClickNextLevel()
    {
        popupWin.SetActive(false);
        LevelManager.Instance.CreateLevelData();
        LevelManager.Instance.SetTextLevel();
        LevelManager.Instance.CreateLevel();
    }

    public void OnClickTryAgain()
    {
        popupLose.SetActive(false);

    }
}
