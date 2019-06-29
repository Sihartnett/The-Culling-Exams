using System.Collections;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerMovement : MonoBehaviour
{

    public float mouseSensitivityX = 5f;
    public float mouseSensitivityY = 4f;
    
    public float moveSpeed = 3f;

    private readonly float gridSize = 1f;
    private readonly float factor = 1f;

    private float mouseX = 0.0f;
    private float mouseY = 0.0f;
    
    private Transform centerPoint;

    private Vector2 input;
    [HideInInspector] public bool isMoving = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float t;
    
    [HideInInspector] public PostProcessingBehaviour postBehavior;
    [HideInInspector] public VignetteModel.Settings vigMod;

    private bool isDead = false;

    //Scene Manager
    private SceneManagerSystem SMS;
    private Animator anim;

    private void Start()
    {
        isDead = false;
        SMS = FindObjectOfType<SceneManagerSystem>();

        anim = GetComponentInChildren<Animator>();
        postBehavior = GetComponentInChildren<PostProcessingBehaviour>();
        vigMod = postBehavior.profile.vignette.settings;
        vigMod.intensity = 0;
        postBehavior.profile.vignette.settings = vigMod;
    }

    private void Awake()
    {
        centerPoint = GameObject.Find("Cylinder").transform;
    }

    // Update is Empty Because all Update functionality was moved to the PlayManager
    // This was done to make sure all keyboard imput is in the right place
    private void Update()
    {
    }
    
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

                TileComponent tile = tileManager.allTiles[row, column].GetComponent<TileComponent>();

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
                    
                    // If you make it to this part of the code you are moving
                    
                    var moveToMe = new Vector3(tile.Tile.CenterPoint.x, 0.1f, tile.Tile.CenterPoint.z);
                    tileManager.fatigue -= 1;

                    tileManager.currentTile.x = row;
                    tileManager.currentTile.y = column;

                    if (tileManager.fatigue <= 0)
                    {
                        StartCoroutine(Death());

                        CrateScript.Deselect();
                        SMS.GameOver();
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

                    StartCoroutine(move(tileManager, transform, moveToMe, moveV, moveH));
                }
            }
        }        
    }

    public IEnumerator Death()
    {
        anim.SetTrigger("Death");
        isDead = true;
        yield return new WaitForSeconds(1.0f);
        yield return null;
    }

    public IEnumerator move(TilePlayManager tileManager, Transform transform, Vector3 endPoint, float vert, float horiz)
    {
        //CrateScript.ClearSelection(tileManager);
        //CrateScript.ClearHighlighted(tileManager);

        isMoving = true;
        startPosition = transform.position;
        endPosition = endPoint;
        t = 0;

        anim.SetFloat("Horizontal", horiz * 4);
        anim.SetFloat("Vertical", vert * 5);
        
        if ( horiz < 0 )
            anim.SetBool("moveLeft", true);
        if (horiz > 0)
            anim.SetBool("moveRight", true);
        if (vert < 0)
            anim.SetBool("moveBack", true);
        if (vert > 0)
            anim.SetBool("isMoving", true);

        while (t < 1f)
        {
            t += Time.deltaTime * (moveSpeed / gridSize) * factor;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        isMoving = false;

        anim.SetFloat("Horizontal", 0);
        anim.SetFloat("Vertical", 0);

        anim.SetBool("moveRight", false);
        anim.SetBool("moveLeft", false);
        anim.SetBool("moveBack", false);
        anim.SetBool("isMoving", false);
        
        yield return 0;
    }
}


