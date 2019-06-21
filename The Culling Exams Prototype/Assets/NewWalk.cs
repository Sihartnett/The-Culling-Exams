using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWalk : MonoBehaviour
{
    Animator kidController;
    float speed = 10.0f;
    Rigidbody playerRB;

    void Start()
    {

        playerRB = GetComponent<Rigidbody>();

        kidController = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)
             || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            //Animate Walk
            kidController.SetBool("Walk", true);
            kidController.SetBool("Stop", false);

        }

        //Stop  Animation
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A)
             || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            kidController.SetBool("Walk", false);
            kidController.SetBool("Stop", true);
        }



    }

    void FixedUpdate()
    {
        //Move Code

        //Move Player Forward
        if (Input.GetKey(KeyCode.W))
        {
            playerRB.AddForce(new Vector3(0, 0, 5), ForceMode.VelocityChange);

            playerRB.rotation = Quaternion.LookRotation(Vector3.forward);
        }

        //Move Player Back
        if (Input.GetKey(KeyCode.S))
        {
            playerRB.AddForce(new Vector3(0, 0, -5), ForceMode.VelocityChange);

            playerRB.rotation = Quaternion.LookRotation(Vector3.back);

        }

        //Move Player Left
        if (Input.GetKey(KeyCode.A))
        {
            //Move Player
            playerRB.AddForce(new Vector3(-5, 0, 0), ForceMode.VelocityChange);

            playerRB.rotation = Quaternion.LookRotation(Vector3.left);
        }

        //Move Player Right
        if (Input.GetKey(KeyCode.D))
        {
            //Move Player
            playerRB.AddForce(new Vector3(5, 0, 0), ForceMode.VelocityChange);

            playerRB.rotation = Quaternion.LookRotation(Vector3.right);
        }
    }
}