using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryFunction : MonoBehaviour
{
   public void ReturnMain()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
