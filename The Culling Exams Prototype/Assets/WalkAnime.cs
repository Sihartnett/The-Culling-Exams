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
    void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        translation *= Time.deltaTime;
        //transform.Translate(0, 0, translation);

        if (translation != 0)
        {
            anim.SetBool("isMoving",true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }
}
