using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private void Start()
        {
        }

        private void Awake ()
        {
            centerPoint = GameObject.Find("Cylinder").transform;
            player = GameObject.Find("Ethan").transform;
        }

        private void Update()
        {

        }
        
        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            Movement();
        }

        private void LateUpdate ()
        {
            CameraOrbit();
        }

        public float mouseSensitivityX = 5f, mouseSensitivityY = 4f, walkSpeed = 5f, turnSpeed = 5f;

        private Transform centerPoint, player;
        private float mouseX, mouseY, moveV, moveH;
        
        //Simple camera orbiting around player using a pivot point (center point)
        void CameraOrbit ()
        {

            mouseX += Input.GetAxis("Mouse X") * mouseSensitivityX;
            mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivityY;

            mouseY = Mathf.Clamp(mouseY, -60f, 0f);

            centerPoint.rotation = Quaternion.Euler(mouseY, mouseX, 0f);
            player.rotation = Quaternion.Euler(0f, mouseX, 0f);

        }

        //Player movement and camera rotation, so that the player moves where the camera is pointing
        void Movement ()
        {
            moveH = Input.GetAxisRaw("Horizontal") * walkSpeed;
            moveV = Input.GetAxisRaw("Vertical") * walkSpeed;

            Vector3 movement = new Vector3(moveH, 0, moveV);

            movement = transform.rotation * movement;

            transform.Translate(movement * Time.deltaTime);

            if (Input.GetAxis("Vertical") != 0)
            {
                Quaternion turnAngle = Quaternion.Euler(0, centerPoint.eulerAngles.y, 0);

                transform.rotation = Quaternion.Slerp(transform.rotation, turnAngle, Time.deltaTime * turnSpeed);
            }
        }
    }
}
