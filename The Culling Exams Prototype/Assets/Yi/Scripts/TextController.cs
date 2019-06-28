using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TextController : MonoBehaviour
{
    [SerializeField] GameObject Loading;

    [SerializeField] TextAsset dialogue;
    [SerializeField] TextMeshProUGUI outputText;

    [SerializeField] string Character1;
    [SerializeField] string Character2;

    [SerializeField] RawImage C1;
    [SerializeField] RawImage C2;

    private string[] text;

    private int counter = -1;

    // Start is called before the first frame update
    void Start()
    {
        InitializeText();
        NextLineOfDialogue();
    }

    void InitializeText()
    {        
        text = dialogue.text.Split('\n');
    }

    public void NextLineOfDialogue()
    {
        if (counter < text.Length - 1)
        {
            counter++;
            outputText.text = text[counter];
            SetUpPortrait(counter);
        }
        else
        {
            Debug.Log("No text!");
            Loading.SetActive(true);
            SceneManager.LoadSceneAsync("Level1_Backup");
        }
    }

    void SetUpPortrait(int count)
    {
        if (text[count].Contains(Character1)) {
            Debug.Log(text[count]);
            C1.color = new Color32(255, 255, 255, 255);
            C2.color = new Color32(128, 128, 128, 255);
        }
        else if (text[count].Contains(Character2)) {
            C1.color = new Color32(128, 128, 128, 255);
            C2.color = new Color32(255, 255, 255, 255);
            Debug.Log(text[count]);
        }
        }
}
