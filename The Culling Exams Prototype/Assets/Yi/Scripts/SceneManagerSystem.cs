using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerSystem : MonoBehaviour
{
    [SerializeField] AudioClip WinSound, LostSound, SelectSound, DeselectSound, SelectingSound;

    public bool LoadOneTime = true;

    private bool isMainMenu = false;
    private AudioSource audioPlayer;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        if (SceneManager.GetActiveScene().buildIndex == 0)
            isMainMenu = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMainMenu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("Level1"); 
            }
        }
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LostGame()
    {
        if (LoadOneTime)
        {
            audioPlayer.PlayOneShot(LostSound);
            Invoke("LoadLostGame", 0.5f);
            LoadOneTime = false;   
        }
    }

    public void LoadLostGame()
    {
        SceneManager.LoadScene("Defeat Scene");
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
}
