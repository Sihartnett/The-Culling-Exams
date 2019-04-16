using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOn : MonoBehaviour
{
    [SerializeField]
    public Material red;
    [SerializeField]
    public Material green;

    private MeshRenderer myRend;
    [HideInInspector]
    public bool currentlySelected = false;
    void Start()
    {
        myRend = GetComponent<MeshRenderer>();
    }
    
    public void Clickme()
    {
        if(currentlySelected == false)
        {
            myRend.material = red;
        }
        else
        {
            myRend.material = green;

        }
    }
    
}
