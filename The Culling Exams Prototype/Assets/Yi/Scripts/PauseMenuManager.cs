using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{

    public static PauseMenuManager instance = null;
    [SerializeField] GameObject PauseMenu;
    private bool PauseorNot = false;  

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)

            instance = this;   

        else if (instance != this)   
            
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseorNot = !PauseorNot;

        if (PauseorNot)
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void Resume()
    {
        PauseorNot = !PauseorNot;
    }

    public void Quit2()
    {
        Application.Quit();
    }
}
