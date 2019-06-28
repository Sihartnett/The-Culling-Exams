using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TextController : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] GameObject Loading;

    [SerializeField] TextAsset dialogue_Eliza;
    [SerializeField] TextAsset dialogue_Isabel;
    [SerializeField] TextAsset dialogue_Ian;

    [HideInInspector]
    [SerializeField] TextMeshProUGUI outputText;

    private string LabMonitorName = "Lab Monitor";
    private string ProtagonistName;

    [Tooltip("Art for lab monitor!")]
    [SerializeField] Sprite LabMonitor;
    [Tooltip("Drag art assets into these slots.")]
    [SerializeField] Sprite Eliza, Isabel, Ian;

    [HideInInspector]
    [SerializeField] Image labmonitorpic;
    [HideInInspector]
    [SerializeField] Image protagonistpic;

    private string[] text;

    private int counter = -1;

    private SceneManagerSystem SMS;

    private void Awake()
    {
        SMS = FindObjectOfType<SceneManagerSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeText();
        NextLineOfDialogue();
    }

    void InitializeText()
    {
        labmonitorpic.sprite = LabMonitor;

        if (SMS.Lives == 3)
        {
            protagonistpic.sprite = Eliza;
            ProtagonistName = "Eliza";
            text = dialogue_Eliza.text.Split('\n');
        }
        else if (SMS.Lives == 2)
        {
            protagonistpic.sprite = Isabel;
            ProtagonistName = "Isabel";
            text = dialogue_Isabel.text.Split('\n');
        }
        else if (SMS.Lives == 1)
        {
            protagonistpic.sprite = Ian;
            ProtagonistName = "Ian";
            text = dialogue_Ian.text.Split('\n');
        }
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
        if (text[count].Contains(LabMonitorName)) {
            //Debug.Log(text[count]);
            labmonitorpic.color = new Color32(255, 255, 255, 255);
            protagonistpic.color = new Color32(128, 128, 128, 255);
        }
        else if (text[count].Contains(ProtagonistName)) {
            labmonitorpic.color = new Color32(128, 128, 128, 255);
            protagonistpic.color = new Color32(255, 255, 255, 255);
            //Debug.Log(text[count]);
        }
        }
}
