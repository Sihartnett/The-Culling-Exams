using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkScript : MonoBehaviour
{
    private float speed = 1f;
    public float wSpeed = 1f;
    public float rotSpeed; 
    static Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isIdle", false);
            anim.SetBool("isIdleLeft", false);
            anim.SetBool("isidleright", false);
        }

        else if (Input.GetKey(KeyCode.S))
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isIdle", true);
            anim.SetBool("isIdleLeft", false);
            anim.SetBool("isidleright", false);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isIdle", true);
            anim.SetBool("isIdleLeft", true);
            anim.SetBool("isidleright", false);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isIdle", false);
            anim.SetBool("isIdleLeft", false);
            anim.SetBool("isidleright", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
            
            anim.SetBool("isIdle", true);
            anim.SetBool("isIdleLeft", false);
            anim.SetBool("isidleright", true);

        }

        var z = Input.GetAxis("Vertical") * speed;
        var y = Input.GetAxis("Horizontal") * rotSpeed;
        transform.Translate(0, 0, z);
        transform.Translate(0, y, 0);


    }
}
