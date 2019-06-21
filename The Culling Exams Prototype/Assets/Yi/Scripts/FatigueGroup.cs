using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FatigueGroup : MonoBehaviour
{
    Menus.PauseMenuManager PMM;

    // Start is called before the first frame update
    void Start()
    {
        PMM = Menus.PauseMenuManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (PMM != null)
            GetComponentInChildren<TextMeshPro>().text = "Fatigue: " + PMM.fatigueCopy.ToString();
    }
}
