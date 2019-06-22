using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerSystem : MonoBehaviour
{
    #region variables

    [SerializeField] AudioClip WinSound, LostSound, SelectSound, DeselectSound, SelectingSound;

    public bool LoadOneTime = true;

    private AudioSource audioPlayer;
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
            Invoke("LoadNextLevel", 0.5f);
            LoadOneTime = false;
        }
    }

    public void LoadNextLevel()
    {
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
        // SMS.LostGame();
    }

    #endregion

    #region Sound Function

    public void LostGame()
    {
        if (LoadOneTime)
        {
            audioPlayer.PlayOneShot(LostSound);
            Invoke("LoadLostGame", 0.5f);
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
