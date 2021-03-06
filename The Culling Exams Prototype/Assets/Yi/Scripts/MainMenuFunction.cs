﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MainMenuFunction : MonoBehaviour
{
    [SerializeField] string[] LevelName;
    [SerializeField] Button levelbutton;
    [SerializeField] GameObject title;
    [SerializeField] GameObject levelselect;
    [SerializeField] Transform buttonParent;
    [SerializeField] GameObject Parent;
    [SerializeField] GameObject TouchBG;
    [SerializeField] GameObject buttonGroup;
    [SerializeField] GameObject LoadingBG;

    private SceneManagerSystem SMS;

    private void Awake()
    {
        SMS = FindObjectOfType<SceneManagerSystem>();
    }

    public void enableButtons()
    {
        GetComponentInChildren<Animator>().SetTrigger("PlayButtons");
        buttonGroup.SetActive(true);
        TouchBG.SetActive(false);
    }

    public void enableStoryMode()
    {
        StartCoroutine(StartStory());
    }

    public void enableLevelSelect()
    {
        SMS.Gamemode = 2;
        GetComponentInChildren<Animator>().SetTrigger("PlayTitle");
        Invoke("showLevelSelect", 0.7f);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    void showLevelSelect()
    {
        levelselect.SetActive(true);
        CreateLevelButton();
    }

    void CreateLevelButton()
    {
        foreach (Transform child in Parent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < LevelName.Length; i++)
        {
            Button lb = Instantiate(levelbutton, buttonParent);
            lb.GetComponentInChildren<TextMeshProUGUI>().text = "Level" + (i + 1);
            int tempint = i;
            lb.onClick.AddListener(() => LoadSelectedLevel(tempint));
        }
    }

    void LoadSelectedLevel(int scenenum)
    {
        LoadingBG.SetActive(true);
        SceneManager.LoadSceneAsync(LevelName[scenenum]);
    }

    IEnumerator StartStory()
    {
        yield return new WaitForSeconds(1.75f);
        SMS.Gamemode = 1;
        LoadingBG.SetActive(true);
        SceneManager.LoadSceneAsync("ChooseScene");
    }

}
