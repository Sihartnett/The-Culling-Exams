using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{

    public static PauseMenuManager instance = null;

    [SerializeField] GameObject PauseMenu;
    [SerializeField] TextMeshProUGUI fatigueCounter;
    [SerializeField] Material skybox;
    [SerializeField] SceneManagerSystem SMS;
    [SerializeField] GameObject FatigueGroup;
    [SerializeField] GameObject lostScreen;
    [SerializeField] GameObject managerGroup;

    private TilePlayManager TPM;
    private PlayerMovement TPUS;
    private bool canPause = true;
    private bool PauseorNot = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        //TPM = FindObjectOfType<TilePlayManager>();
        //TPUS = FindObjectOfType<PlayerMovement>();
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        //RenderSettings.skybox = skybox; 
    }

    // Update is called once per frame
    void Update()
    {
        if (canPause)
        {
            if (TPM.fatigue >= 0)
            {
                fatigueCounter.text = "Fatigue: " + TPM.fatigue.ToString();
                FatigueGroup.GetComponentInChildren<TextMeshPro>().text = "Fatigue: " + TPM.fatigue.ToString();
            }
            if (TPM.fatigue <= 0)
            {
                //SMS.LostGame();
                lostScreen.SetActive(true);
                canPause = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
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
    }

    public void Resume()
    {
        PauseorNot = !PauseorNot;
    }

    public void Quit2()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //RenderSettings.skybox = skybox;

        if(scene.name == "Victory Scene" || scene.name == "Defeat Scene" || scene.name == "Main Menu")
        {
            managerGroup.SetActive(false);
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
            PauseorNot = false;
            lostScreen.SetActive(false);
            SMS.LoadOneTime = true;
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
