﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator_Controller : MonoBehaviour
{
    private void Awake()
    {
        centerPoint = GameObject.Find("Cylinder").transform;
        player = GameObject.Find("Jasper").transform;
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
    public void Movement()
    {
        if (!isMoving)
        {
            moveH = Input.GetAxisRaw("Horizontal") * walkSpeed;
            moveV = Input.GetAxisRaw("Vertical") * walkSpeed;

            TilePlayManager tileManager = transform.parent.GetComponent<TilePlayManager>();

            TileComponent currentTile = tileManager.allTiles[(int)tileManager.currentTile.x, (int)tileManager.currentTile.y].GetComponent<TileComponent>();

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
                    row = currentTile.Row + 1;
                    column = currentTile.Column;
                }
                else if (135 <= playerRotationDegree && playerRotationDegree <= 225)
                {
                    row = currentTile.Row;
                    column = currentTile.Column - 1;
                }
                else if (225 <= playerRotationDegree && playerRotationDegree <= 315)
                {
                    row = currentTile.Row - 1;
                    column = currentTile.Column;
                }
                else if (315 <= playerRotationDegree && playerRotationDegree <= 360 || 0 <= playerRotationDegree && playerRotationDegree <= 45)
                {
                    row = currentTile.Row;
                    column = currentTile.Column + 1;
                }
            }
            else if (moveV < 0)
            {
                // Move back
                if (45 <= playerRotationDegree && playerRotationDegree <= 135)
                {
                    row = currentTile.Row - 1;
                    column = currentTile.Column;
                }
                else if (135 <= playerRotationDegree && playerRotationDegree <= 225)
                {
                    row = currentTile.Row;
                    column = currentTile.Column + 1;
                }
                else if (225 <= playerRotationDegree && playerRotationDegree <= 315)
                {
                    row = currentTile.Row + 1;
                    column = currentTile.Column;
                }
                else if (315 <= playerRotationDegree && playerRotationDegree <= 360 || 0 <= playerRotationDegree && playerRotationDegree <= 45)
                {
                    row = currentTile.Row;
                    column = currentTile.Column - 1;
                }
            }
            else if (moveH < 0)
            {
                // Move left
                if (45 <= playerRotationDegree && playerRotationDegree <= 135)
                {
                    row = currentTile.Row;
                    column = currentTile.Column + 1;
                }
                else if (135 <= playerRotationDegree && playerRotationDegree <= 225)
                {
                    row = currentTile.Row + 1;
                    column = currentTile.Column;
                }
                else if (225 <= playerRotationDegree && playerRotationDegree <= 315)
                {
                    row = currentTile.Row;
                    column = currentTile.Column - 1;
                }
                else if (315 <= playerRotationDegree && playerRotationDegree <= 360 || 0 <= playerRotationDegree && playerRotationDegree <= 45)
                {
                    row = currentTile.Row - 1;
                    column = currentTile.Column;
                }
            }
            else if (moveH > 0)
            {
                // Move right
                if (45 <= playerRotationDegree && playerRotationDegree <= 135)
                {
                    row = currentTile.Row;
                    column = currentTile.Column - 1;
                }
                else if (135 <= playerRotationDegree && playerRotationDegree <= 225)
                {
                    row = currentTile.Row - 1;
                    column = currentTile.Column;
                }
                else if (225 <= playerRotationDegree && playerRotationDegree <= 315)
                {
                    row = currentTile.Row;
                    column = currentTile.Column + 1;
                }
                else if (315 <= playerRotationDegree && playerRotationDegree <= 360 || 0 <= playerRotationDegree && playerRotationDegree <= 45)
                {
                    row = currentTile.Row + 1;
                    column = currentTile.Column;
                }
            }

            TileComponent tile = null;

            tile = tileManager.allTiles[(int)row, (int)column].GetComponent<TileComponent>();

            if (moveV != 0 || moveH != 0)
            {
                // if the target tile is a tile you cannot move return out of function
                //if (tile.Tile.tileType == TileType.barrier
                    //|| tile.Tile.tileType == TileType.fall
                    //|| tile.Tile.tileType == TileType.moveableBarrier
                    //|| tile.Tile.crateType != CrateType.none)
                    return;

                // now check for all the impassible wall tiles in yourself and in your target 
                if (currentTile.Row < row
                    &&
                    (currentTile.Tile.northWallType == WallType.blueDoor
                    || currentTile.Tile.northWallType == WallType.brownDoor
                    || currentTile.Tile.northWallType == WallType.purpleDoor
                    || currentTile.Tile.northWallType == WallType.redDoor
                    || currentTile.Tile.northWallType == WallType.wall
                    || currentTile.Tile.northWallType == WallType.window
                    || tile.Tile.southWallType == WallType.blueDoor
                    || tile.Tile.southWallType == WallType.brownDoor
                    || tile.Tile.southWallType == WallType.purpleDoor
                    || tile.Tile.southWallType == WallType.redDoor
                    || tile.Tile.southWallType == WallType.wall
                    || tile.Tile.southWallType == WallType.window))
                    return;

                if (currentTile.Row > row
                    &&
                    (currentTile.Tile.southWallType == WallType.blueDoor
                    || currentTile.Tile.southWallType == WallType.brownDoor
                    || currentTile.Tile.southWallType == WallType.purpleDoor
                    || currentTile.Tile.southWallType == WallType.redDoor
                    || currentTile.Tile.southWallType == WallType.wall
                    || currentTile.Tile.southWallType == WallType.window
                    || tile.Tile.northWallType == WallType.blueDoor
                    || tile.Tile.northWallType == WallType.brownDoor
                    || tile.Tile.northWallType == WallType.purpleDoor
                    || tile.Tile.northWallType == WallType.redDoor
                    || tile.Tile.northWallType == WallType.wall
                    || tile.Tile.northWallType == WallType.window))
                    return;

                if (currentTile.Column < column
                    &&
                    (currentTile.Tile.westWallType == WallType.blueDoor
                    || currentTile.Tile.westWallType == WallType.brownDoor
                    || currentTile.Tile.westWallType == WallType.purpleDoor
                    || currentTile.Tile.westWallType == WallType.redDoor
                    || currentTile.Tile.westWallType == WallType.wall
                    || currentTile.Tile.westWallType == WallType.window
                    || tile.Tile.eastWallType == WallType.blueDoor
                    || tile.Tile.eastWallType == WallType.brownDoor
                    || tile.Tile.eastWallType == WallType.purpleDoor
                    || tile.Tile.eastWallType == WallType.redDoor
                    || tile.Tile.eastWallType == WallType.wall
                    || tile.Tile.eastWallType == WallType.window))
                    return;

                if (currentTile.Column > column
                    &&
                    (currentTile.Tile.eastWallType == WallType.blueDoor
                    || currentTile.Tile.eastWallType == WallType.brownDoor
                    || currentTile.Tile.eastWallType == WallType.purpleDoor
                    || currentTile.Tile.eastWallType == WallType.redDoor
                    || currentTile.Tile.eastWallType == WallType.wall
                    || currentTile.Tile.eastWallType == WallType.window
                    || tile.Tile.westWallType == WallType.blueDoor
                    || tile.Tile.westWallType == WallType.brownDoor
                    || tile.Tile.westWallType == WallType.purpleDoor
                    || tile.Tile.westWallType == WallType.redDoor
                    || tile.Tile.westWallType == WallType.wall
                    || tile.Tile.westWallType == WallType.window))
                    return;

                var moveToMe = new Vector3(tile.Tile.CenterPoint.x, 0.1f, tile.Tile.CenterPoint.z);
                tileManager.fatigue -= 1;

                StartCoroutine(move(transform, moveToMe));
            }
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
