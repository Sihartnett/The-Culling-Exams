﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerSystem : MonoBehaviour
{
    [SerializeField] AudioClip WinSound, LostSound, SelectSound, DeselectSound, SelectingSound;

    public bool LoadOneTime = true;

    private AudioSource audioPlayer;

    
    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

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
        Application.Quit();
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
        SceneManager.LoadSceneAsync("Defeat Scene");
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
