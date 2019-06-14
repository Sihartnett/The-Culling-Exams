using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuFunction : MonoBehaviour
{
    [SerializeField] string[] LevelName;
    [SerializeField] GameObject levelbutton;
    [SerializeField] GameObject title;
    [SerializeField] GameObject levelselect;
    [SerializeField] Transform buttonParent;

    public void enableLevelSelect()
    {
        title.SetActive(false);
        levelselect.SetActive(true);
        CreateLevelButton();
    }

    void CreateLevelButton()
    {
        for (int i = 0; i < LevelName.Length; i++)
        {
            Instantiate(levelbutton, buttonParent);
        }
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
