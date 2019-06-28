using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Menus
{
    public class PauseMenuManager : MonoBehaviour
    {
        #region Variables

        [SerializeField] GameObject PauseMenu;
        [SerializeField] TextMeshProUGUI fatigueCounter;
        [SerializeField] Material skybox;
        [SerializeField] SceneManagerSystem SMS;

        [SerializeField] GameObject managerGroup;

        public int fatigueCopy = 0;

        private TilePlayManager TPM;
        private PlayerMovement TPUS;
        public bool canPause = true;
        private bool canFatigue = true;
        public bool Paused = false;

        #region Singleton Implimentation

        private static PauseMenuManager _instance = null;
        public static PauseMenuManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    PauseMenuManager[] managers = FindObjectsOfType(typeof(PauseMenuManager)) as PauseMenuManager[];
                    if (managers.Length > 1)
                    {
                        Debug.Log("Too many active PauseMenuManagers scripts in the scene");
                    }
                    if (managers.Length == 0)
                    {
                        Debug.Log("Need one active PauseMenuManager in the scene");
                    }
                    else
                    {
                        _instance = managers[0];
                        DontDestroyOnLoad(_instance);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            Instance.Initialize();
        }

        public void Initialize()
        {

        }
        #endregion

        #endregion

        #region Pause Function

  

        // Update is called once per frame
        void Update()
        {
            if (canFatigue)
            {
                if (TPM.fatigue >= 0)
                {
                    fatigueCounter.text = "Fatigue: " + TPM.fatigue.ToString();
                    fatigueCopy = TPM.fatigue;
                }
                if (TPM.fatigue <= 0)
                {
                    //SMS.LostGame();
                    //StartCoroutine(EnableLostScreen());
                    
                }
            }
            if (canPause)
            {

                if (Input.GetKeyDown(KeyCode.Escape))
                    Paused = !Paused;

                if (Paused)
                {
                    PauseMenu.SetActive(true);
                    TPUS.enabled = false;
                    Time.timeScale = 0;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    PauseMenu.SetActive(false);
                    TPUS.enabled = true;
                    Time.timeScale = 1;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }

        public void Resume()
        {
            Paused = !Paused;
        }
        #endregion

        #region OnLoaded
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //RenderSettings.skybox = skybox;
            SMS.LoadingBG.SetActive(false);

            if (scene.name == "Main Menu")
            {
                SMS.BGMPlayer.Stop();
                SMS.BGMPlayer.clip = SMS.MMT;
                SMS.BGMPlayer.Play();
            }

            else
            {
                //
                SMS.BGMPlayer.Stop();
                int RNG = UnityEngine.Random.Range(0, 3);
                SMS.BGMPlayer.clip = SMS.BGMs[RNG];
                SMS.BGMPlayer.Play();
                // 
            }

            if (scene.name == "Victory Scene" || scene.name == "Defeat Scene" || scene.name == "Main Menu" || scene.name == "PreLevel")
            {
                managerGroup.SetActive(false);
                PauseMenu.SetActive(false);
                Time.timeScale = 1;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SMS.lostScreen_storyfail.SetActive(false);
                SMS.lostScreen_storymode.SetActive(false);
                canFatigue = false;
                canPause = false;
            }
            else
            {
                TPM = FindObjectOfType<TilePlayManager>();
                TPUS = FindObjectOfType<PlayerMovement>();
                managerGroup.SetActive(true);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                canPause = true;
                canFatigue = true;
                Paused = false;
                SMS.lostScreen_levelselect.SetActive(false);
                SMS.LoadOneTime = true;
            }
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        #endregion
    }
}


