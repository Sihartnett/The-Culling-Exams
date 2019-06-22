using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    private float gridSize = 1f;
    private float factor = 1f;

    private Vector2 input;
    private bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;

    private bool isDead = false;

    //Scene Manager
    private SceneManagerSystem SMS;
    private Animator anim;

    private void Start()
    {
        isDead = false;
        SMS = FindObjectOfType<SceneManagerSystem>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        centerPoint = GameObject.Find("Cylinder").transform;
        //animator = player.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
    }
    
    public float mouseSensitivityX = 5f, mouseSensitivityY = 4f;

    private Transform centerPoint;

    float mouseX = 0.0f, mouseY = 0.0f;

    //Simple camera orbiting around player using a pivot point (center point)
    public void CameraOrbit(float verticle, float horizontal)
    {
        mouseX += verticle * mouseSensitivityX;
        mouseY -= horizontal * mouseSensitivityY;

        mouseY = Mathf.Clamp(mouseY, -60f, 0f);

        centerPoint.rotation = Quaternion.Euler(mouseY, mouseX, 0f);
        transform.rotation = Quaternion.Euler(0f, mouseX, 0f);
    }

    //Player movement and camera rotation, so that the player moves where the camera is pointing
    public void Movement(float moveH, float moveV, Transform player)
    {
        if (!isDead)
        {
            if (!isMoving)
            {
                TilePlayManager tileManager = transform.parent.GetComponent<TilePlayManager>();

                TileComponent currentTile = tileManager.allTiles[(int)tileManager.currentTile.x, (int)tileManager.currentTile.y].GetComponent<TileComponent>();

                float playerRotationDegree = player.transform.eulerAngles.y;

                int row = 0;
                int column = 0;

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



                anim.SetBool("isMoving", false);

                if (moveV > 0)
                {
                    anim.SetBool("isMoving", true);
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
                    anim.SetBool("moveBack", true);
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
                    anim.SetBool("moveLeft", true);
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
                    anim.SetBool("moveRight", true);
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
                else
                {
                    anim.SetBool("moveRight", false);
                    anim.SetBool("moveLeft", false);
                    anim.SetBool("moveBack", false);
                    anim.SetBool("isMoving", false);
                }

                TileComponent tile = null;

                tile = tileManager.allTiles[row, column].GetComponent<TileComponent>();

                if (moveV != 0 || moveH != 0)
                {
                    // if the target tile is a tile you cannot move return out of function
                    if (tile.Tile.tileType == TileType.barrierTile
                        || tile.Tile.tileType == TileType.fallTile
                        || tile.Tile.tileType == TileType.moveableBarrierTile
                        || tile.Tile.crateType != CrateType.none)
                        return;

                    // now check for all the impassible wall tiles in yourself and in your target 
                    if (currentTile.Row < row
                        &&
                        (currentTile.Tile.northWallType == WallType.blueDoor
                        || currentTile.Tile.northWallType == WallType.brownDoor
                        || currentTile.Tile.northWallType == WallType.purpleDoor
                        || currentTile.Tile.northWallType == WallType.redDoor
                        || currentTile.Tile.northWallType == WallType.orangeDoor
                        || currentTile.Tile.northWallType == WallType.lightBlueDoor
                        || currentTile.Tile.northWallType == WallType.wall
                        || currentTile.Tile.northWallType == WallType.window
                        || tile.Tile.southWallType == WallType.blueDoor
                        || tile.Tile.southWallType == WallType.brownDoor
                        || tile.Tile.southWallType == WallType.purpleDoor
                        || tile.Tile.southWallType == WallType.redDoor
                        || tile.Tile.southWallType == WallType.wall
                        || tile.Tile.southWallType == WallType.window
                        || tile.Tile.southWallType == WallType.orangeDoor
                        || tile.Tile.southWallType == WallType.lightBlueDoor
                        ))
                        return;

                    if (currentTile.Row > row
                        &&
                        (currentTile.Tile.southWallType == WallType.blueDoor
                        || currentTile.Tile.southWallType == WallType.brownDoor
                        || currentTile.Tile.southWallType == WallType.purpleDoor
                        || currentTile.Tile.southWallType == WallType.redDoor
                        || currentTile.Tile.southWallType == WallType.wall
                        || currentTile.Tile.southWallType == WallType.window
                        || currentTile.Tile.southWallType == WallType.orangeDoor
                        || currentTile.Tile.southWallType == WallType.lightBlueDoor
                        || tile.Tile.northWallType == WallType.blueDoor
                        || tile.Tile.northWallType == WallType.brownDoor
                        || tile.Tile.northWallType == WallType.purpleDoor
                        || tile.Tile.northWallType == WallType.redDoor
                        || tile.Tile.northWallType == WallType.wall
                        || tile.Tile.northWallType == WallType.window
                        || tile.Tile.northWallType == WallType.orangeDoor
                        || tile.Tile.northWallType == WallType.lightBlueDoor))
                        return;

                    if (currentTile.Column < column
                        &&
                        (currentTile.Tile.westWallType == WallType.blueDoor
                        || currentTile.Tile.westWallType == WallType.brownDoor
                        || currentTile.Tile.westWallType == WallType.purpleDoor
                        || currentTile.Tile.westWallType == WallType.redDoor
                        || currentTile.Tile.westWallType == WallType.wall
                        || currentTile.Tile.westWallType == WallType.window
                        || currentTile.Tile.westWallType == WallType.orangeDoor
                        || currentTile.Tile.westWallType == WallType.lightBlueDoor
                        || tile.Tile.eastWallType == WallType.blueDoor
                        || tile.Tile.eastWallType == WallType.brownDoor
                        || tile.Tile.eastWallType == WallType.purpleDoor
                        || tile.Tile.eastWallType == WallType.redDoor
                        || tile.Tile.eastWallType == WallType.wall
                        || tile.Tile.eastWallType == WallType.window
                        || tile.Tile.eastWallType == WallType.orangeDoor
                        || tile.Tile.eastWallType == WallType.lightBlueDoor
                        ))
                        return;

                    if (currentTile.Column > column
                        &&
                        (currentTile.Tile.eastWallType == WallType.blueDoor
                        || currentTile.Tile.eastWallType == WallType.brownDoor
                        || currentTile.Tile.eastWallType == WallType.purpleDoor
                        || currentTile.Tile.eastWallType == WallType.redDoor
                        || currentTile.Tile.eastWallType == WallType.wall
                        || currentTile.Tile.eastWallType == WallType.window
                        || currentTile.Tile.eastWallType == WallType.orangeDoor
                        || currentTile.Tile.eastWallType == WallType.lightBlueDoor
                        || tile.Tile.westWallType == WallType.blueDoor
                        || tile.Tile.westWallType == WallType.brownDoor
                        || tile.Tile.westWallType == WallType.purpleDoor
                        || tile.Tile.westWallType == WallType.redDoor
                        || tile.Tile.westWallType == WallType.wall
                        || tile.Tile.westWallType == WallType.window
                        || tile.Tile.westWallType == WallType.orangeDoor
                        || tile.Tile.westWallType == WallType.lightBlueDoor
                        ))
                        return;

                    var moveToMe = new Vector3(tile.Tile.CenterPoint.x, 0.1f, tile.Tile.CenterPoint.z);
                    tileManager.fatigue -= 1;

                    tileManager.currentTile.x = row;
                    tileManager.currentTile.y = column;

                    if (tileManager.fatigue <= 0)
                    {
                        StartCoroutine(Death());
                    }

                    if (tile.Tile.pickupType == PickupType.fatiguePickup)
                    {
                        tileManager.fatigue += (int)tile.Tile.pickupCount;
                        tileManager.SetPickupType(row, column, PickupType.none, 0.0f);
                    }

                    if (tile.Tile.tileType == TileType.finishTile)
                    {
                        CrateScript.Deselect();
                        SMS.NextLevel();
                    }

                    StartCoroutine(move(transform, moveToMe, moveV, moveH));
                }
            }
        }        
    }

    public IEnumerator Death()
    {
        anim.SetTrigger("Death");
        isDead = true;
        yield return new WaitForSeconds(1.0f);
        CrateScript.Deselect();
        SMS.GameOver();
        yield return null;
    }

    public IEnumerator move(Transform transform, Vector3 endPoint, float vert, float horiz)
    {
        isMoving = true;
        startPosition = transform.position;
        endPosition = endPoint;
        t = 0;

        //animator?.SetFloat("Vertical", vert);
        //animator?.SetFloat("Horizontal", vert);
        //animator?.SetBool("isMoving", true);

        while (t < 1f)
        {
            t += Time.deltaTime * (moveSpeed / gridSize) * factor;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        isMoving = false;

        //animator?.SetFloat("Verticle", 0);
        //animator?.SetFloat("Horizontal", 0);
        //animator?.SetBool("isMoving", false);

        yield return 0;
    }
}


