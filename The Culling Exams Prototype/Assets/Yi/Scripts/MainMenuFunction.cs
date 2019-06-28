using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

    public void enableButtons()
    {
        GetComponentInChildren<Animator>().SetTrigger("PlayButtons");
        buttonGroup.SetActive(true);
        TouchBG.SetActive(false);
    }

    public void enableStoryMode()
    {
        LoadingBG.SetActive(true);
        SceneManager.LoadSceneAsync("PreLevel");
    }

    public void enableLevelSelect()
    {
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

}
