﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FatigueGroup : MonoBehaviour
{
    PauseMenuManager PMM;

    // Start is called before the first frame update
    void Start()
    {
        PMM = PauseMenuManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponentInChildren<TextMeshPro>().text = "Fatigue: " + PMM.fatigueCopy.ToString();
    }
}
