using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerSystem : MonoBehaviour
{
    private bool isMainMenu = false;

    // Start is called before the first frame update
    void Start()
    {
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
}
