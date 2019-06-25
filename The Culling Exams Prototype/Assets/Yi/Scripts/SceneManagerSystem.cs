using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerSystem : MonoBehaviour
{
    #region variables

    [SerializeField] AudioClip WinSound, LostSound, SelectSound, DeselectSound, SelectingSound;

    public GameObject lostScreen;

    public bool LoadOneTime = true;

    private AudioSource audioPlayer;
    public AudioSource BGMPlayer;
    public AudioClip MMT;
    public AudioClip[] BGMs;
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

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public IEnumerator EnableLostScreen()
    {
        yield return new WaitForSeconds(1.75f);
        lostScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Menus.PauseMenuManager.Instance.canPause = false;
        yield return null;
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
        SceneManager.LoadSceneAsync("Main Menu");
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
            StartCoroutine(EnableLostScreen());
            LoadOneTime = false;   
        }
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
