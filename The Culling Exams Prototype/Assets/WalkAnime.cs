using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAnime : MonoBehaviour
{
    static Animator anim;
    public float speed = 3f;
    void Start()
    {
        anim = GetComponent<Animator>();
        //Animator anim = GetComponent<Animator>();
        //anim.SetBool("isMoving", true);
    }

    // Update is called once per frame
   public void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        translation *= Time.deltaTime;
        //transform.Translate(0, 0, translation);

        if (translation != 0)
        {
            WalkOn();
        }
        else
        {
            WalkOff();
        }
    }
    public static void WalkOn()
    {
        anim.SetBool("isMoving", true);
    }
    public static void WalkOff()
    {
        anim.SetBool("isMoving", false);
    }
}
