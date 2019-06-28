using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerSystem : MonoBehaviour
{
    #region variables

    [SerializeField] AudioClip WinSound, LostSound, SelectSound, DeselectSound, SelectingSound;

    public GameObject LoadingBG;
    public GameObject lostScreen_storymode;
    public GameObject lostScreen_storyfail;
    public GameObject lostScreen_levelselect;

    public bool LoadOneTime = true;

    private AudioSource audioPlayer;
    public AudioSource BGMPlayer;
    public AudioClip MMT;
    public AudioClip[] BGMs;

    private int lives = 3;
    private int gamemode = 0;

    public int Gamemode { get => gamemode; set => gamemode = value; }
    public int Lives { get => lives; set => lives = value; }
    #endregion

    #region Pause Menu Load Function

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    public void NextLevel()
    {
        if (LoadOneTime)
        {
            audioPlayer.PlayOneShot(WinSound);
            Invoke("LoadNextLevel", 0.7f);
            LoadOneTime = false;
        }
    }

    public void LoadNextLevel()

    {
        ////
        //BGMPlayer.Stop();
        //int RNG = UnityEngine.Random.Range(0, 3);
        //BGMPlayer.clip = BGMs[RNG];
        //BGMPlayer.Play();
        ////
        LoadingBG.SetActive(true);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void ReturnToMainMenu()
    {
        gamemode = 0;
        lives = 3;
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void ReturnToNarrativeScene()
    {
        SceneManager.LoadSceneAsync("PreLevel");
    }

    public void ResetLevel()
    {
        string scene;
        scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(scene);
    }

    // YI TODO:  I really like a lot of your code and you are on the way to becoming a great programmer.
    // Finish this class Currently the functionality of checking death condition is in the Fatigue Counter
    // I have moved a conditional to the play manager that does it now and that will fix the bug where you die and still win the level

    public void GameOver ()
    {
        LostGame();
    }

    #endregion

    #region Sound Function

    public void LostGame()
    {
        if (LoadOneTime)
        {
            audioPlayer.PlayOneShot(LostSound);
            if(gamemode == 1)
            {
                lives--;
                if(lives <= 0)
                    StartCoroutine(EnableLostScreen(lostScreen_storyfail));
                else if(lives > 0)
                    StartCoroutine(EnableLostScreen(lostScreen_storymode));
            }
            if(gamemode == 2)
                StartCoroutine(EnableLostScreen(lostScreen_levelselect));
            LoadOneTime = false;   
        }
    }

    public IEnumerator EnableLostScreen(GameObject lost)
    {
        yield return new WaitForSeconds(1.75f);
        lost.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Menus.PauseMenuManager.Instance.canPause = false;
        yield return null;
    }

    public void SelectCrate()
    {
        audioPlayer.PlayOneShot(SelectSound);
        audioPlayer.clip = SelectingSound;
        audioPlayer.Play();
        audioPlayer.loop = true;
    }

    public void DeSelectCrate()
    {
        audioPlayer.Pause();
        audioPlayer.loop = false;
        audioPlayer.PlayOneShot(DeselectSound);
    }

    public void SelectingCrate()
    {
        audioPlayer.PlayOneShot(SelectingSound);
    }
    
    #endregion
}
