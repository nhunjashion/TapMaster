using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace TapMaster
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; set; }
        public GameObject popupWin;
        public GameObject popupLose;
        public Button deleteBtn;
        public GameObject SelectUI;

        [Header("Audio")]
        [SerializeField] AudioSource audioSource;
        [SerializeField] List<AudioClip> audioClips = new();

        public static event Action OnClickSpell;
        public static event Action OnClickCancelSpell;

        private void Start()
        {
            Instance = this;
            CubeCtrl.OnClickCube += PlaySound;
        }
        public void ResetMap()
        {
            LevelManager.Instance.ResetMap();
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

        public void OnClickBackBtn()
        {
            Application.Quit();
        }

        public void PlaySound()
        {
            Random random = new();
            SoundName name = (SoundName)random.Next(Enum.GetValues(typeof(SoundName)).Length);


            AudioClip sound = audioClips.Find(x => x.name == name.ToString());
            Debug.Log(sound);
            audioSource.PlayOneShot(sound,.5f);
        }


        public void OnClickDelete()
        {
            if (LevelManager.Instance.SpellAmount <= 0)
                deleteBtn.interactable = false;
            else
            {
                SelectUI.SetActive(true);
                OnClickSpell?.Invoke();
            }

        }

        public void OnClickCancel()
        {
            SelectUI.SetActive(false);
            OnClickCancelSpell?.Invoke();
        }
    }
}

public enum SoundName
{
    UI_Click_1,
    UI_Click_2,
    UI_Click_3,
    UI_Click_4,
}