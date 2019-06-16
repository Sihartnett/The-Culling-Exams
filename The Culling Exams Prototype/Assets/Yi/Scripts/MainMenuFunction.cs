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

    public void enableLevelSelect()
    {
        GetComponentInChildren<Animator>().SetTrigger("PlayTitle");
        Invoke("showLevelSelect", 1f);
    }

    void showLevelSelect()
    {
        title.SetActive(false);
        levelselect.SetActive(true);
        CreateLevelButton();
    }

    void CreateLevelButton()
    {
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
