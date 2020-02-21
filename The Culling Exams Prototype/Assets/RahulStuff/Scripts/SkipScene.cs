using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SkipScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SceneManager.LoadScene("Level1_Backup");
        }   
    }

    public void Skip()
    {
        SceneManager.LoadScene("Level1_Backup");
    }
}
