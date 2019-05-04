using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private void Start()
    {
    }

    private void Awake()
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

    private void LateUpdate()
    {
        CameraOrbit();
    }

    public float mouseSensitivityX = 5f, mouseSensitivityY = 4f, walkSpeed = 5f, turnSpeed = 5f;

    private Transform centerPoint, player;
    private float mouseX, mouseY, moveV, moveH;

    //Simple camera orbiting around player using a pivot point (center point)
    void CameraOrbit()
    {
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivityX;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivityY;

        mouseY = Mathf.Clamp(mouseY, -60f, 0f);

        centerPoint.rotation = Quaternion.Euler(mouseY, mouseX, 0f);
        transform.rotation = Quaternion.Euler(0f, mouseX, 0f);

    }

    //Player movement and camera rotation, so that the player moves where the camera is pointing
   public  void Movement()
    {
        if (!isMoving)
        {
            moveH = Input.GetAxisRaw("Horizontal") * walkSpeed;
            moveV = Input.GetAxisRaw("Vertical") * walkSpeed;
            
            TilePlayManager tileManager = transform.parent.GetComponent<TilePlayManager>();

            Vector2 currentTile = tileManager.currentTile;

            float playerRotationDegree = player.transform.eulerAngles.y;

            float row = 0;
            float column = 0;

            if (moveV != 0 || moveH != 0)
            {
                while (0 < playerRotationDegree && playerRotationDegree >= 360)
                {
                    if (0 < playerRotationDegree)
                        playerRotationDegree += 360;
                    if (playerRotationDegree >= 360)
                        playerRotationDegree -= 360;
                }
            }

            if (moveV > 0)
            {
                // Move forward
                if (45 <= playerRotationDegree && playerRotationDegree <= 135)
                {
                    row = currentTile.x + 1;
                    column = currentTile.y;
                }
                else if (135 <= playerRotationDegree && playerRotationDegree <= 225)
                {
                    row = currentTile.x;
                    column = currentTile.y - 1;
                }
                else if (225 <= playerRotationDegree && playerRotationDegree <= 315)
                {
                    row = currentTile.x - 1;
                    column = currentTile.y;
                }
                else if (315 <= playerRotationDegree && playerRotationDegree <= 360 || 0 <= playerRotationDegree && playerRotationDegree <= 45)
                {
                    row = currentTile.x;
                    column = currentTile.y + 1;
                }
            }
            else if (moveV < 0)
            {
                // Move back
                if (45 <= playerRotationDegree && playerRotationDegree <= 135)
                {
                    row = currentTile.x - 1;
                    column = currentTile.y;
                }
                else if (135 <= playerRotationDegree && playerRotationDegree <= 225)
                {
                    row = currentTile.x;
                    column = currentTile.y + 1;
                }
                else if (225 <= playerRotationDegree && playerRotationDegree <= 315)
                {
                    row = currentTile.x + 1;
                    column = currentTile.y;
                }
                else if (315 <= playerRotationDegree && playerRotationDegree <= 360 || 0 <= playerRotationDegree && playerRotationDegree <= 45)
                {
                    row = currentTile.x;
                    column = currentTile.y - 1;
                }
            }
            else if (moveH < 0)
            {
                // Move left
                if (45 <= playerRotationDegree && playerRotationDegree <= 135)
                {
                    row = currentTile.x;
                    column = currentTile.y + 1;
                }
                else if (135 <= playerRotationDegree && playerRotationDegree <= 225)
                {
                    row = currentTile.x + 1;
                    column = currentTile.y;
                }
                else if (225 <= playerRotationDegree && playerRotationDegree <= 315)
                {
                    row = currentTile.x;
                    column = currentTile.y - 1;
                }
                else if (315 <= playerRotationDegree && playerRotationDegree <= 360 || 0 <= playerRotationDegree && playerRotationDegree <= 45)
                {
                    row = currentTile.x - 1;
                    column = currentTile.y;
                }
            }
            else if (moveH > 0)
            {
                // Move right
                if (45 <= playerRotationDegree && playerRotationDegree <= 135)
                {
                    row = currentTile.x;
                    column = currentTile.y - 1;
                }
                else if (135 <= playerRotationDegree && playerRotationDegree <= 225)
                {
                    row = currentTile.x - 1;
                    column = currentTile.y;
                }
                else if (225 <= playerRotationDegree && playerRotationDegree <= 315)
                {
                    row = currentTile.x;
                    column = currentTile.y + 1;
                }
                else if (315 <= playerRotationDegree && playerRotationDegree <= 360 || 0 <= playerRotationDegree && playerRotationDegree <= 45)
                {
                    row = currentTile.x + 1;
                    column = currentTile.y;
                }
            }

            TileComponent tile = null;

            tile = tileManager.allTiles[(int)row, (int)column].GetComponent<TileComponent>();

            if (moveV != 0 || moveH != 0)
            {
                // This is a valid move
                if ((tile.Tile.tileType == TileType.basic ||
                    tile.Tile.tileType == TileType.moveable ||
                    tile.Tile.tileType == TileType.blueTile ||
                    tile.Tile.tileType == TileType.redTile ||
                    tile.Tile.tileType == TileType.brownTile ||
                    tile.Tile.tileType == TileType.purpleTile ||
                    tile.Tile.tileType == TileType.start ||
                    tile.Tile.tileType == TileType.finish) &&
                    tile.Tile.crateType == CrateType.none)
                {
                    var moveToMe = new Vector3(tile.Tile.CenterPoint.x, 0.1f, tile.Tile.CenterPoint.z);
                    tileManager.fatigue -= 1;
                    
                    StartCoroutine(move(transform, moveToMe));
                }
            }
        }

        return;


        Vector3 movement = new Vector3(moveH, 0, moveV);

        movement = transform.rotation * movement;

        transform.Translate(movement * Time.deltaTime);

        if (Input.GetAxis("Vertical") != 0)
        {
            Quaternion turnAngle = Quaternion.Euler(0, centerPoint.eulerAngles.y, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, turnAngle, Time.deltaTime * turnSpeed);
        }

    }

    private float moveSpeed = 3f;
    private float gridSize = 1f;
    private float factor = 1f;

    private Vector2 input;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;

    public IEnumerator move(Transform transform, Vector3 endPoint)
    {
        isMoving = true;
        startPosition = transform.position;
        endPosition = endPoint;
        t = 0;

        //Animator anim = GetComponent<Animator>();

        while (t < 1f)
        {
            t += Time.deltaTime * (moveSpeed / gridSize) * factor;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            
            yield return null;
        }

        isMoving = false;

        yield return 0;
    }
}


