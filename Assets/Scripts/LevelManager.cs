using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TapMaster
{
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

        [SerializeField] int spellAmount;
        public int SpellAmount { get => spellAmount; set => spellAmount= value; }


        public TextMeshProUGUI levelTxt;
        public TextMeshProUGUI spellAmountTxt;


        [SerializeField] bool isWin = false;

        private void Start()
        {
            Instance = this;
            currentLevel = 0;
            SetTextLevel();
            CreateLevel();
            SetTextSpellAmount();
        }
        public void SetTextLevel()
        {
            int level = currentLevel;
            levelTxt.text = "Level : " + level.ToString();
        }

        public void SetTextSpellAmount()
        {
            int amount = spellAmount;
            spellAmountTxt.text = "Remaining: " + amount.ToString();
        }

        public void CreateLevel()
        {
            CreateLevelData();
            CubeCtrl.Instance.SpawnCube();
            SetTextSpellAmount();
        }

        public void ResetMap()
        {
            levelTarget = countX * countY * countZ;

            if (levelTarget >= 343)
                spellAmount = 4;
            else if (levelTarget >= 216)
                spellAmount = 3;
            else if (levelTarget >= 150)
                spellAmount = 2;
            else spellAmount = 1;
            GameSceneManager.Instance.deleteBtn.interactable = true;
            CubeCtrl.Instance.ResetMap();
            SetTextSpellAmount();
        }

        public void CreateLevelData()
        {
            if(currentLevel == 0)
            {
                countX = 2;
                countY = 1;
                countZ = 2;
            }
            else if(currentLevel <= 10)
            {
                countX = RandomValue(2, 3);
                countY = RandomValue(2, 3);
                countZ = RandomValue(2, 4);
            }
            else if(currentLevel <= 30)
            {
                countX = RandomValue(3, 6);
                countY = RandomValue(3, 6);
                countZ = RandomValue(3, 6);
            }
            else if(currentLevel <= 70)
            {
                countX = RandomValue(5, 6);
                countY = RandomValue(5, 6);
                countZ = RandomValue(5, 6);
            }
            else
            {
                countX = RandomValue(6, 8);
                countY = RandomValue(6, 8);
                countZ = RandomValue(6, 8);
            }

            levelTarget = countX * countY * countZ;

            if (levelTarget >= 343)
                spellAmount = 4;
            else if (levelTarget >= 216)
                spellAmount = 3;
            else if (levelTarget >= 150)
                spellAmount = 2;
            else spellAmount = 1;
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

}
