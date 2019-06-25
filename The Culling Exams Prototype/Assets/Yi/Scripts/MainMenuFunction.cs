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
    [SerializeField] GameObject storyCanvas;

    public void enableButtons()
    {

    }

    public void enableStoryMode()
    {
        GetComponentInChildren<Animator>().SetTrigger("PlayTitle");
        Invoke("disableTitle", 1f);
    }

    public void enableLevelSelect()
    {
        storyCanvas.SetActive(false);        
        Invoke("showLevelSelect", 0.1f);
    }

    void disableTitle()
    {
        title.SetActive(false);
        storyCanvas.SetActive(true);
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
        Debug.Log("clear");
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
        SceneManager.LoadSceneAsync(LevelName[scenenum]);
    }

}
