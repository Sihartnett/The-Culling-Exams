using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{

    public static PauseMenuManager instance = null;

    [SerializeField] GameObject PauseMenu;
    [SerializeField] TextMeshProUGUI fatigueCounter;
    [SerializeField] Material skybox;

    private TilePlayManager TPM;
    private SceneManagerSystem SMS;
    private PlayerMovement TPUS;
    public bool PauseorNot = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        TPM = FindObjectOfType<TilePlayManager>();
        SMS = FindObjectOfType<SceneManagerSystem>();
        TPUS = FindObjectOfType<PlayerMovement>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        RenderSettings.skybox = skybox; 
    }

    // Update is called once per frame
    void Update()
    {
        if (TPM.fatigue >= 0)
            fatigueCounter.text = "Fatigue: " + TPM.fatigue.ToString();
        else if (TPM.fatigue <= 0)
        {
            SMS.LostGame();
            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            PauseorNot = !PauseorNot;

        if (PauseorNot)
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

    public void Resume()
    {
        PauseorNot = !PauseorNot;
    }

    public void Quit2()
    {
        Application.Quit();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RenderSettings.skybox = skybox;
        TPM = FindObjectOfType<TilePlayManager>();
        SMS = FindObjectOfType<SceneManagerSystem>();
        TPUS = FindObjectOfType<PlayerMovement>();
        SMS.LoadOneTime = true;
        if(scene.name == "Victory Scene" || scene.name == "Defeat Scene" || scene.name == "Main Menu")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
