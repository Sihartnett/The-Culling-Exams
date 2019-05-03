using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementCounter : MonoBehaviour
{
   static Text fCount;

   // public static int _counter = PlayerGridMovement.counter;
    void Start()
    {
        fCount = GetComponent<Text>();

    }

    void Update()
    {
        
        FatigueCount();

    }
    public static void FatigueCount()
    {
      //  fCount.text= "Fatigue: " + _counter;
        
    }
}
