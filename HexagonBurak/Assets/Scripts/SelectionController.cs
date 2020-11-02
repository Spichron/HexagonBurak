using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEditor.ShortcutManagement;
//using System.Reflection;
//using UnityEditor;

public class SelectionController : MonoBehaviour
{
    private Transform set1;
    private GameObject edge1;
    private SpriteRenderer sR1;
    private Transform set2;
    private GameObject edge2;
    private SpriteRenderer sR2;
    private Transform set3;
    private GameObject edge3;
    private SpriteRenderer sR3;
    private Transform set4;
    private GameObject edge4;
    private SpriteRenderer sR4;
    private Transform set5;
    private GameObject edge5;
    private SpriteRenderer sR5;
    private Transform set6;
    private GameObject edge6;
    private SpriteRenderer sR6;

    private RaycastHit2D preHit; //Checks mouse release is on the same object or not
    private RaycastHit2D hit; //For Selecting the First Hexagon
    private RaycastHit2D hit1; //For Selecting the One of the Neighbour Hexagons
    private RaycastHit2D hit2; //For Selecting the One of the Neighbour Hexagons
    private RaycastHit2D rotationHit; //For disabling mouse movements towards rotator
    private RaycastHit2D rotationReverseHit; //Looks for reverse direction

    public LayerMask hexagonLayer; //For Selecting Hexagons Only
    public LayerMask rotationLayer; //For Selecting Hexagons Only

    private Transform preSelect;
    private Transform select;
    private Transform select1;
    private Transform select2;
    private Transform choose;
    private Transform choose1;
    private Transform choose2;
    private Transform rotateSelect;

    private Vector2 mouse2D;
    private Vector2 direction1;
    private Vector2 direction2;
    private Vector2 mouseStart;
    private Vector2 mouseEnd;

    private Vector3 mousePosition;
    private Vector3 centerPoint;

    private Vector3 selectorPosTemp;
    private Vector3 selector1PosTemp;
    private Vector3 selector2PosTemp;

    private float selectAngle;
    private float mouseDistance;

    private int counter;
    private int clockWise;
    private int rotationClock = 0;
    private int bombCounter = 1000;

    private GameObject selector;
    private GameObject selector1;
    private GameObject selector2;
    private GameObject originPoint;
    private GameObject hexagonSample;
    private GameObject downBlock;
    private GameObject upBlock;
    private GameObject rightBlock;
    private GameObject leftBlock;

    private Text scoreText;

    private bool isRotating = false;
    private bool isMoving = false;
    private bool moveLeft;
    private bool moveUp;
    private bool moveRight;
    private bool moveDown;
    private bool rotationLock = false;
    private bool isMoveSet = false;
    private bool canRotate = true;
    private bool isDestroy = false;
    private bool resetParameters = false;
    private bool moveDone = false;
    private bool tripleCheck = false;

    //PUBLIC

    public bool globalRotaiton = false;
    public bool globalDestroy = false;
    public bool globalMoving = false;

    //POSITIONING AND REPOSITIONING

    public Sprite bombSprite;

    private Vector3[,] hexagonLocation;

    private Vector3 scorePosition;

    private Vector2 aChange;

    private GameObject[,] hexagonObject;

    private GameObject aObject;
    private GameObject bObject;
    private GameObject cObject;
    private GameObject aSelect;

    public Color[] hexagonColor;

    private float[] colorPicker;

    private float xPosition = 0.6f;
    private float yPosition = 1f;
    private float fColorCount;
    private float fC; //color counter

    public int line = 8;
    public int column = 9;
    private int c; //color count
    private int totalHex;
    private int i;
    private int j;
    private int turnCount = 1;
    private int scorePoint = 0;
    private int iSafe;
    private int jSafe;
    private int newTarget;

    private float a;
    private float b;
    private float fLine;
    private float fColumn;
    private float xCamera;
    private float yCamera;
    private float screenRatio;
    private float targetRatio;
    private float differenceInSize;

    public int colorCount = 5;
    public int randomColor;
    public int destroyCounter = 0;
    public int hexCount;
    public int bombReady = 0;
    public int safeCounter = 0;
    public int destroyCount = 0;

    private bool stopTurn = false;

    public bool checkDone = false;
    public bool preCheckDone = false;
    public bool gameOver = false;
    public bool globalMovement = true;

    private bool startState = true;

    private bool canDestroy = false;

    public bool turnEnd = false;

    private HexagonController hexCon;

    /* Alt + V For CLearing Console------------
    [Shortcut("Clear Console", KeyCode.V, ShortcutModifiers.Alt)]
    public static void ClearConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
    //-----------------------------------------*/
    private void Start()
    {
        Starter();
        CameraSet(); //To make camera get all scene in any condition
        RandomSet(); //In here I created random colored hexagons and then check that if they are matching. and while they are checking randomize their color. So at the begining there will be no matching at the beginin whatsoever. 
        RoutineCheck(); //This routine check checks that if any hexagon can become a match with rotation. If not game overs.
        startState = false; //BUT is it is in Start function, it ll just reset the scene. 
    }

    private void Update()
    {
        turnEnd = false;
        PauseGame();//TEMP
        MousePush();// When the mouse push, this situation is needed because player must choose its hexagon group when they release their finger.
        MouseOn();//checks mouse place and if it is gonna move more then a small radius
        MousePull();//so select the three pair of hexagons. Also it is important where you are touching to select the other pairs.
        TripleRotation();// If finger is moved on the screen it rotates due to the movement. In a small cap area it doesn't rotate. It is intentional.
        RemoveSelector(); //Selectors become invisible when there is matchin and reproducing 
        ResetHexagons(); //Make stage stable again. First checks every hexagons below and if there isnt, they start to fall to the exact points they meant to go. Then Checks all hexagons upper area, if there is not a hexagon at that place creates a new hexagon above there. ANd also if a column doesnt have any hexagons, by basically checks when i=0, add new hexagon to that column's beggining.
        DrawScore(); //after scene is stabilized updates the score and sets the time for the bombs
        if (gameOver) // Gameover condition.
        {
            Debug.Log("Game Over");
            SceneManager.LoadScene(3, LoadSceneMode.Single);
        }
    }

    //CREATION EVENT---------------------------

    private void Starter()
    {
        //SELECTION AND ROTATION
        selector = GameObject.Find("Selector");//Defining Main Selector
        selector1 = GameObject.Find("Selector1");//Defining First Auto Selector
        selector2 = GameObject.Find("Selector2");//Defining Second Auto Selector
        originPoint = GameObject.Find("Rotator");//Defining Rotation Point

        //hexagonLayer |= (1 << LayerMask.NameToLayer("Hexagon"));    //Defining hexagon layer, all hexagons are in this layer
        //rotationLayer |= (1 << LayerMask.NameToLayer("Selected"));  //Selected layer changes its layer due to make raycasts past itself for sake of choosing the other two hexagons. TEMPORARY

        //PRODUCTION
        hexagonSample = GameObject.Find("HexagonSample");//All hexagons are reproducing from this sample
        downBlock = GameObject.Find("DownBlock");//Arranges the canvas size (orthosize)
        upBlock = GameObject.Find("UpBlock"); //Arranges the canvas size TEMPORARY
        rightBlock = GameObject.Find("RightBlock");//Arranges the canvas size TEMPORARY
        leftBlock = GameObject.Find("LeftBlock");//Arranges the canvas size
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();//Defining Text to show Scores to the Player

        //DEFINING Hexagons 
        totalHex = line * column; //Top Hexagon Count
        hexCount = totalHex;
        hexagonLocation = new Vector3[line, column];//Stores Locations for all Hexagon PLaces
        hexagonObject = new GameObject[line, column];//Stores Data of all Hexagons
        hexagonColor = new Color[colorCount];//Stores all Hexagon Colors
        colorPicker = new float[colorCount];//Stores all Color HUEs
        a = xPosition;//for positioning all Hexagons x axises it takes the offset value 
        b = yPosition;//for positioning all Hexagons y axises it takes the offset value
        fColorCount = colorCount; // for making float HUE values

        //Selecting Colors
        for (c = 0; c < colorCount; c++)
        {
            fC = c; //so the result of the division will be a float value
            colorPicker[c] = fC / fColorCount;//It selects the HUEs in fixed range, they can also be selected manually
            //Debug.Log("ColorHue:" + colorPicker[c]);
            hexagonColor[c] = Color.HSVToRGB(colorPicker[c], 0.7f, 1f);// this color array holds the defined colors
        }

        newTarget = bombCounter;
    }

    private void CameraSet()
    {
        fLine = line; //holds line data as a float
        fColumn = column; // holds column data as a float
        xCamera = 0.6f + 0.45f * (fColumn - 1f); // for geting canvas size.y, based on hexagon size
        yCamera = 0.25f + (fLine / 2f);          // for geting canvas size.y, based on hexagon size

        //POSITIONING MAIN CAMERA
        Camera.main.transform.position = new Vector3(xCamera, yCamera, Camera.main.transform.position.z); //Camera position assigment due to hexagon count, it will be at center.

        //BLOCK POSITIONING
        downBlock.transform.position = new Vector3(xCamera, -0.5f, downBlock.transform.position.z); //this box represents the canvas size.x
        leftBlock.transform.position = new Vector3(-0.5f, yCamera, leftBlock.transform.position.z); //this box represents the canvas size.y
        upBlock.transform.position = new Vector3(xCamera, 1f + fLine, upBlock.transform.position.z); //TEMPORARY
        rightBlock.transform.position = new Vector3(1.7f + 0.9f * (fColumn - 1f), yCamera, rightBlock.transform.position.z); //TEMPORARY

        //BLOCK SIZING
        downBlock.transform.localScale = new Vector3(1.7f + 0.9f * (fColumn - 1f), downBlock.transform.localScale.y, downBlock.transform.localScale.z); //its sclae is arranged
        upBlock.transform.localScale = new Vector3(1.7f + 0.9f * (fColumn - 1f), upBlock.transform.localScale.y, upBlock.transform.localScale.z);//TEMPORARY
        rightBlock.transform.localScale = new Vector3(rightBlock.transform.localScale.x, 1f + fLine, rightBlock.transform.localScale.z);//TEMPORARY
        leftBlock.transform.localScale = new Vector3(leftBlock.transform.localScale.x, (1f + fLine) * 1.5f, leftBlock.transform.localScale.z); // its scale is arranged

        //ASPECT RATIO CALCULATIONS
        screenRatio = (float)Screen.width / (float)Screen.height; //Represents the aspect ratio that about to change
        targetRatio = downBlock.GetComponent<SpriteRenderer>().bounds.size.x / leftBlock.GetComponent<SpriteRenderer>().bounds.size.y; // Represents an aspect ratio done by the blocks

        //CAMERA SIZING
        if (screenRatio >= targetRatio)//So zooms in
        {
            Camera.main.orthographicSize = leftBlock.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        }
        else//Else zooms out
        {
            differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = leftBlock.GetComponent<SpriteRenderer>().bounds.size.y / 2 * differenceInSize;
        }

        scorePosition = Camera.main.WorldToScreenPoint(new Vector3(downBlock.transform.position.x, leftBlock.GetComponent<SpriteRenderer>().bounds.size.y * 0.8f, -5)); //For Positioning score text
        scoreText.transform.position = scorePosition; //Positions the score text
    }

    private void RandomSet()
    {
        //Debug.Log("Here");
        //RANDOM HEXAGON PLACEMENT
        for (i = 0; i < line; i++)
        {
            for (j = 0; j < column; j++)
            {
                //Debug.Log("j= " + j);
                if (j % 2 == 1) //For Column Position Shifting in Odd numbers it shifts
                {
                    b -= 0.5f;
                }
                randomColor = (int)(System.Math.Round(Random.value / 2.5f, 1) * 10);
                //Debug.Log(System.Math.Round((Random.value)/2.5f,1)*10); //RANDOM RANGE uses time to create randomness, so in loops, it wont randomize
                hexagonLocation[i, j] = new Vector3(a, b, 0f);
                //Debug.Log("HexagonLocations: " + hexagonLocation[i, j]);
                hexagonObject[i, j] = Instantiate(hexagonSample, hexagonLocation[i, j], Quaternion.identity) as GameObject;
                hexagonObject[i, j].GetComponent<SpriteRenderer>().color = hexagonColor[randomColor];
                hexagonObject[i, j].GetComponent<HexagonController>().enabled = true; //So the sample cant become a problem by the chance
                if (j % 2 == 1)
                {
                    b += 0.5f;
                }
                a += 0.9f;
            }
            a = xPosition;
            b++;
        }
        b = yPosition;

        //Change colors until not Matches for starting point
        for (i = 0; i < line; i++)
        {
            for (j = 0; j < column; j++)
            {
                isDestroy = true;
                while (isDestroy)
                {
                    isDestroy = false;
                    CheckHits();
                    if (isDestroy)
                    {
                        randomColor = (int)(System.Math.Round(Random.value / 2.5f, 1) * 10);
                        hexagonObject[i, j].GetComponent<SpriteRenderer>().color = hexagonColor[randomColor];
                    }
                }
            }
        }
    }

    //TEMPORARY--------------------------------

    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }

        }
    }

    //MOUSE CONDITION CHECK EVENTS-------------
    private void MousePush()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MousePosition();
            mouseStart = mouse2D;
            preHit = Physics2D.Raycast(mouse2D, Vector2.zero, hexagonLayer);
            if (preHit)
            {
                preSelect = preHit.transform;
            }
        }
    }

    private void MouseOn()
    {
        if (Input.GetMouseButton(0)) //While Clicking with Left Mouse Button
        {
            MousePosition();
            if (choose && canRotate)
            {
                MouseMove();
            }
        }
    }

    private void MousePosition()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Screen to World Point Change
        mouse2D = new Vector2(mousePosition.x, mousePosition.y);
    }

    //ROTATION PROCESS-------------------------
    private void MouseMove()
    {
        mouseDistance = Mathf.Sqrt(Mathf.Pow(mouse2D.x - mouseStart.x, 2) + Mathf.Pow(mouse2D.y - mouseStart.y, 2));
        if (mouseDistance >= 0.5)
        {
            if (mouse2D.x >= mouseStart.x)
            {
                moveRight = true;
            }
            else
            {
                moveLeft = true;
            }

            if (mouse2D.y >= mouseStart.y)
            {
                moveUp = true;
            }
            else
            {
                moveDown = true;
            }
            mouseEnd = mouse2D; //that position becomes static
            isMoveSet = true;
        }
    }

    private void TripleRotation()
    {
        if (choose && choose1 && choose2)
        {
            //Debug.Log("Here");
            if (!rotationLock && isMoveSet)
            {
                RotationWay();
            }

            if (isMoving)
            {
                //ROTATE
                if (!isRotating)
                {
                    isRotating = true;
                    selectorPosTemp = selector.transform.position;
                    selector1PosTemp = selector1.transform.position;
                    selector2PosTemp = selector2.transform.position;
                    choose.transform.SetParent(originPoint.transform);
                    choose1.transform.SetParent(originPoint.transform);
                    choose2.transform.SetParent(originPoint.transform);
                    selector.transform.SetParent(originPoint.transform);
                    selector1.transform.SetParent(originPoint.transform);
                    selector2.transform.SetParent(originPoint.transform);
                }

                if (isRotating && turnCount < 4 && !stopTurn)
                {
                    if (rotationClock == 1)
                    {
                        if ((originPoint.transform.eulerAngles.z < 120 * turnCount && turnCount < 3) || (originPoint.transform.eulerAngles.z > 240 && turnCount == 3))
                        {
                            originPoint.transform.eulerAngles += new Vector3(0, 0, 360) * Time.deltaTime * rotationClock;
                        }
                        else
                        {
                            stopTurn = false;
                            ResetTransform();
                            turnCount++;
                        }
                    }
                    else
                    {
                        if (originPoint.transform.eulerAngles.z == 0) //a Little push
                        {
                            originPoint.transform.eulerAngles = new Vector3(0, 0, 359);
                        }
                        if ((originPoint.transform.eulerAngles.z > 360 - 120 * turnCount && turnCount < 3) || (originPoint.transform.eulerAngles.z < 120 && turnCount == 3))
                        {
                            originPoint.transform.eulerAngles += new Vector3(0, 0, 360) * Time.deltaTime * rotationClock;
                        }
                        else
                        {
                            stopTurn = false;
                            ResetTransform();
                            turnCount++;
                        }
                    }

                }

                //Debug.Log("Turns: " + turnCount);
                //Debug.Log("ANGLE: " + originPoint.transform.eulerAngles.z);
                if (turnCount == 4)
                {
                    ResetBack();
                }
            }
        }
    }

    private void RotationWay()
    {
        rotationHit = Physics2D.Raycast(mouseStart, mouseEnd - mouseStart, 2000f, rotationLayer);
        rotateSelect = rotationHit.transform;
        if (!rotateSelect) //Do Reverse
        {
            rotationHit = Physics2D.Raycast(mouseEnd, mouseStart - mouseEnd, 2000f, rotationLayer);
            rotateSelect = rotationReverseHit.transform;
        }

        if (rotateSelect)
        {
            rotateSelect.GetComponent<CircleCollider2D>().enabled = true;
        }

        if (mouseEnd.x > centerPoint.x)
        {
            if (mouseEnd.y > centerPoint.y)
            {
                if (rotateSelect || (moveUp && moveRight) || (moveDown && moveLeft))
                {
                    rotationClock = 0;
                }
                else if (moveUp || moveLeft)
                {
                    rotationClock = 1;
                }
                else if (moveDown || moveRight)
                {
                    rotationClock = -1;
                }
                else
                {
                    rotationClock = 0;
                }
            }
            else
            {
                if (rotateSelect || (moveUp && moveLeft) || (moveDown && moveRight))
                {
                    rotationClock = 0;
                }
                else if (moveUp || moveRight)
                {
                    rotationClock = 1;
                }
                else if (moveDown || moveLeft)
                {
                    rotationClock = -1;
                }
                else
                {
                    rotationClock = 0;
                }
            }
        }
        else
        {
            if (mouseEnd.y > centerPoint.y)
            {
                if (rotateSelect || (moveUp && moveLeft) || (moveDown && moveRight))
                {
                    rotationClock = 0;
                }
                else if (moveDown || moveLeft)
                {
                    rotationClock = 1;
                }
                else if (moveUp || moveRight)
                {
                    rotationClock = -1;
                }
                else
                {
                    rotationClock = 0;
                }
            }
            else
            {
                if (rotateSelect || (moveUp && moveRight) || (moveDown && moveLeft))
                {
                    rotationClock = 0;
                }
                else if (moveDown || moveRight)
                {
                    rotationClock = 1;
                }
                else if (moveUp || moveLeft)
                {
                    rotationClock = -1;
                }
                else
                {
                    rotationClock = 0;
                }
            }
        }
        if (rotationClock != 0)
        {
            isMoving = true;
            rotationLock = true;
            canRotate = false;
        }

        moveLeft = false;
        moveUp = false;
        moveRight = false;
        moveDown = false;
        isMoveSet = false;

        if (rotateSelect)
        {
            rotateSelect.GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    private void ResetTransform()
    {
        for (i = 0; i < line; i++)
        {
            for (j = 0; j < column; j++)
            {
                if (j % 2 != 0)
                {
                    b -= 0.5f;
                }
                hexagonLocation[i, j] = new Vector3(a, b, hexagonLocation[i, j].z);
                RaycastHit2D checkHit = Physics2D.Raycast(hexagonLocation[i, j], Vector2.zero);
                if (checkHit)
                {
                    isDestroy = false;
                    hexagonObject[i, j] = checkHit.transform.gameObject;
                    hexCon = hexagonObject[i, j].GetComponent<HexagonController>();
                    if (turnCount == 3)
                    {
                        hexagonObject[i, j].transform.position = checkHit.transform.position;
                        hexagonObject[i, j].transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    CheckHits();
                    if (isDestroy)
                    {
                        hexCon.isDestroy = true;
                        stopTurn = true;
                        if (choose)
                        {
                            choose.transform.parent = null;
                        }
                        if (choose1)
                        {
                            choose1.transform.parent = null;
                        }
                        if (choose2)
                        {
                            choose2.transform.parent = null;
                        }
                    }
                }
                if (j % 2 != 0)
                {
                    b += 0.5f;
                }
                a += 0.9f;
            }
            a = xPosition;
            b++;
        }
        b = yPosition;
    }

    //SELECTION PROCESS------------------------
    private void MousePull()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!isMoving)
            {
                hit = Physics2D.Raycast(mouse2D, Vector2.zero, hexagonLayer);
                select = hit.transform;

                //IF SELECTS
                if (select && preSelect == select) //For short if selected object when the push and pull is same...
                {
                    //RESET CHOOSES
                    if (choose)
                    {
                        choose.transform.position = new Vector3(choose.transform.position.x, choose.transform.position.y, 0);
                    }
                    if (choose1)
                    {
                        choose1.transform.position = new Vector3(choose1.transform.position.x, choose1.transform.position.y, 0);
                    }
                    if(choose2)
                    {
                        choose2.transform.position = new Vector3(choose2.transform.position.x, choose2.transform.position.y, 0);
                    }

                    //SET NEW SELECTION
                    select.transform.position = new Vector3(select.transform.position.x, select.transform.position.y, -2);
                    //select.gameObject.layer = LayerMask.NameToLayer("Selected"); //Change it for Raycast
                    //Debug.Log("Layer:" + select.gameObject.layer);

                    CalculateOthers();

                    //SET SECOND SELECTION
                    select1 = hit1.transform;
                    if (select1)
                    {
                        select1.transform.position = new Vector3(select1.transform.position.x, select1.transform.position.y, -2);
                    }

                    //SET THIRD SELECTION
                    select2 = hit2.transform;
                    if (select2)
                    {
                        select2.transform.position = new Vector3(select2.transform.position.x, select2.transform.position.y, -2);
                    }

                    //TRANSFER SELECTIONS
                    choose = select;
                    choose1 = select1;
                    choose2 = select2;

                    //FRAME POSITION SET
                    if(choose)
                    {
                        selector.transform.position = new Vector3(choose.transform.position.x, select.transform.position.y, -1);
                    }
                    if (choose1)
                    {
                        selector1.transform.position = new Vector3(choose1.transform.position.x, select1.transform.position.y, -1);
                    }
                    if (choose2)
                    {
                        selector2.transform.position = new Vector3(choose2.transform.position.x, select2.transform.position.y, -1);
                    }
                    if(choose && choose1 && choose2)
                    {
                        centerPoint = new Vector3((choose.transform.position.x + choose1.transform.position.x + choose2.transform.position.x) / 3f, (choose.transform.position.y + choose1.transform.position.y + choose2.transform.position.y) / 3f, -3);
                        originPoint.transform.position = centerPoint;
                    }
                    //RESET SELECTIONS
                    //select.gameObject.layer = LayerMask.NameToLayer("Hexagon"); //Change it for Raycast
                    hit = new RaycastHit2D();
                    hit1 = new RaycastHit2D();
                    hit2 = new RaycastHit2D();
                }
            }
            canRotate = true;
        }

    }

    private void CalculateOthers()
    {
        selectAngle = Mathf.Atan2(mousePosition.y - select.transform.position.y, mousePosition.x - select.transform.position.x) * 180 / Mathf.PI;
        if (selectAngle < 0)
        {
            selectAngle += 360f;
        }

        clockWise = IsClockwise();
        for (counter = 1; counter <= 6; counter++)
        {
            SelectHexa();
            if (hit1 != new RaycastHit2D() && hit2 != new RaycastHit2D())
            {
                break;
            }
            selectAngle += 60 * counter * clockWise;
            if (selectAngle >= 360)
            {
                selectAngle -= 360;
            }
            if (selectAngle <= 0)
            {
                selectAngle += 360;
            }
            SelectHexa();
            if (hit1 != new RaycastHit2D() && hit2 != new RaycastHit2D())
            {
                break;
            }
            selectAngle -= 60 * (1 + counter) * clockWise;
            if (selectAngle >= 360)
            {
                selectAngle -= 360;
            }
            if (selectAngle <= 0)
            {
                selectAngle += 360;
            }
            SelectHexa();
            if (hit1 != new RaycastHit2D() && hit2 != new RaycastHit2D())
            {
                break;
            }
        }
    }

    private int IsClockwise()
    {
        if (selectAngle >= 120f && selectAngle < 180f)
        {
            if (180 - selectAngle >= 120 - selectAngle)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (selectAngle >= 180f && selectAngle < 240f)
        {
            if (240 - selectAngle >= 180 - selectAngle)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (selectAngle >= 240f && selectAngle < 300f)
        {
            if (300 - selectAngle >= 240 - selectAngle)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (selectAngle >= 300f && selectAngle < 360f)
        {
            if (360 - selectAngle >= 300 - selectAngle)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (selectAngle >= 0f && selectAngle < 60f)
        {
            if (60 - selectAngle >= 0 - selectAngle)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (selectAngle >= 60f && selectAngle < 120f)
        {
            if (120 - selectAngle >= 60 - selectAngle)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        return 1;
    }

    private void SelectHexa()
    {
        if (selectAngle >= 90f && selectAngle < 150f)
        {
            direction1 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 90f), Mathf.Sin(Mathf.Deg2Rad * 90f)).normalized;
            direction2 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 150f), Mathf.Sin(Mathf.Deg2Rad * 150f)).normalized;
        }
        else if (selectAngle >= 150f && selectAngle < 210f)
        {
            direction1 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 150f), Mathf.Sin(Mathf.Deg2Rad * 150f)).normalized;
            direction2 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 210f), Mathf.Sin(Mathf.Deg2Rad * 210f)).normalized;
        }
        else if (selectAngle >= 210f && selectAngle < 270f)
        {
            direction1 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 210f), Mathf.Sin(Mathf.Deg2Rad * 210f)).normalized;
            direction2 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 270f), Mathf.Sin(Mathf.Deg2Rad * 270f)).normalized;
        }
        else if (selectAngle >= 270f && selectAngle < 330f)
        {
            direction1 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 270f), Mathf.Sin(Mathf.Deg2Rad * 270f)).normalized;
            direction2 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 330f), Mathf.Sin(Mathf.Deg2Rad * 330f)).normalized;
        }
        else if ((selectAngle >= 330f && selectAngle < 360f) || (selectAngle >= 0f && selectAngle < 30f))
        {
            direction1 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 330f), Mathf.Sin(Mathf.Deg2Rad * 330f)).normalized;
            direction2 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sin(Mathf.Deg2Rad * 30f)).normalized;
        }
        else if (selectAngle >= 30f && selectAngle < 90f)
        {
            direction1 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 30f), Mathf.Sin(Mathf.Deg2Rad * 30f)).normalized;
            direction2 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * 90f), Mathf.Sin(Mathf.Deg2Rad * 90f)).normalized;
        }

        hit1 = Physics2D.Raycast(select.transform.position + new Vector3(direction1.x, direction1.y, select.transform.position.z), Vector2.zero);
        hit2 = Physics2D.Raycast(select.transform.position + new Vector3(direction2.x, direction2.y, select.transform.position.z), Vector2.zero);
        //Debug.DrawRay(select.transform.position, direction1, Color.green);
        //Debug.DrawRay(select.transform.position, direction2, Color.red);
    }
    //-----------------------------------------

    private void RemoveSelector()
    {
        if (!resetParameters && totalHex > hexCount)
        {
            resetParameters = true;
            if (selector.transform.localRotation != Quaternion.Euler(0f, 0f, 0f))
            {
                selector.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            if (selector1.transform.localRotation != Quaternion.Euler(0f, 0f, 0f))
            {
                selector1.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            if (selector2.transform.localRotation != Quaternion.Euler(0f, 0f, 0f))
            {
                selector2.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            if (selector.GetComponent<SpriteRenderer>().enabled)
            {
                selector.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (selector1.GetComponent<SpriteRenderer>().enabled)
            {
                selector1.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (selector2.GetComponent<SpriteRenderer>().enabled)
            {
                selector2.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (originPoint.GetComponent<SpriteRenderer>().enabled)
            {
                originPoint.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    private void ResetHexagons()
    {
        if (totalHex > hexCount && !moveDone)
        {
            tripleCheck = true;
            moveDone = true;
            // RESET HEXAGON POSITIONS AND ROTATIONS
            for (i = 0; i < line; i++)
            {
                for (j = 0; j < column; j++)
                {
                    if (j % 2 != 0)
                    {
                        b -= 0.5f;
                    }
                    RaycastHit2D checkHit = Physics2D.Raycast(hexagonLocation[i, j], Vector2.zero);
                    if (checkHit)
                    {
                        //Debug.Log("Here");
                        hexagonObject[i, j] = checkHit.transform.gameObject;
                        hexagonLocation[i, j] = new Vector3(a, b, 0f);
                        hexCon = hexagonObject[i, j].GetComponent<HexagonController>();
                        if (j % 2 == 0)
                        {
                            hexCon.oddColumn = false;
                        }
                        else
                        {
                            hexCon.oddColumn = true;
                        }
                        if (hexagonObject[i, j].transform.position == checkHit.transform.position && !hexCon.isMoving)
                        {
                            //Debug.Log("Here");
                            RaycastHit2D underHit = Physics2D.Raycast(new Vector2(hexagonObject[i, j].transform.position.x, hexagonObject[i, j].transform.position.y - 0.6f), Vector2.zero);
                            if (!underHit)
                            {
                                tripleCheck = false;
                                hexCon.storedPosition = hexagonObject[i, j].transform.position;
                                hexCon.isMoving = true;
                                if (i == 0)
                                {
                                    hexCon.isMoving = false;
                                }
                            }
                        }
                        if (hexCon.isMoving)
                        {
                            moveDone = false;
                        }
                    }
                    if (j % 2 != 0)
                    {
                        b += 0.5f;
                    }
                    a += 0.9f;
                }
                a = xPosition;
                b++;
            }
            b = yPosition;
            //Debug.Log(moveDone);
        }

        AddNew();

        DestroyOthers();

        if (tripleCheck)
        {
            //Debug.Log("HEY LISTEN!");
            turnEnd = true;
            RoutineCheck();
            AssignNew();
            ResetBack();
            tripleCheck = false;
        }
    }

    private void AddNew()
    {
        if (moveDone)
        {
            tripleCheck = true;
            for (i = 0; i < line; i++)
            {
                for (j = 0; j < column; j++)
                {
                    if (j % 2 != 0)
                    {
                        b -= 0.5f;
                    }
                    hexagonLocation[i, j] = new Vector3(a, b, 0f);
                    RaycastHit2D checkHit = Physics2D.Raycast(hexagonLocation[i, j], Vector2.zero);
                    if (checkHit)
                    {
                        hexagonObject[i, j] = checkHit.transform.gameObject;
                        if (hexagonObject[i, j])
                        {
                            hexagonLocation[i, j] = new Vector3(a, b, 0f);
                            hexCon = hexagonObject[i, j].GetComponent<HexagonController>();
                            hexagonObject[i, j].transform.position = checkHit.transform.position;
                            hexagonObject[i, j].transform.eulerAngles = new Vector3(0, 0, 0);
                            if (j % 2 == 0)
                            {
                                hexCon.oddColumn = false;
                            }
                            else
                            {
                                hexCon.oddColumn = true;
                            }
                            RaycastHit2D overHit = Physics2D.Raycast(new Vector2(hexagonObject[i, j].transform.position.x, hexagonObject[i, j].transform.position.y + 0.6f), Vector2.up, 100f, hexagonLayer);
                            if (!overHit && i != line - 1 && ((System.Math.Round(hexagonObject[i, j].transform.position.y, 1) != fLine && !hexCon.oddColumn) || (System.Math.Round(hexagonObject[i, j].transform.position.y, 2) != fLine - 0.5f && hexCon.oddColumn)))
                            {
                                //Debug.Log("Hits");
                                if (hexCount < totalHex)
                                {
                                    tripleCheck = false;
                                    randomColor = (int)(System.Math.Round(Random.value / 2.5f, 1) * 10);
                                    hexagonObject[i + 1, j] = Instantiate(hexagonSample, hexagonLocation[i + 1, j], Quaternion.identity) as GameObject;
                                    hexagonObject[i + 1, j].GetComponent<SpriteRenderer>().color = hexagonColor[randomColor];
                                    hexagonObject[i + 1, j].GetComponent<HexagonController>().enabled = true;
                                    hexCount++;
                                    if (bombReady > 0)
                                    {
                                        //Debug.Log("AlsoHere");
                                        hexCon.isBomb = true;
                                        bombReady--;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (i == 0 && hexCount < totalHex)
                        {
                            tripleCheck = false;
                            randomColor = (int)(System.Math.Round(Random.value / 2.5f, 1) * 10);
                            hexagonObject[i, j] = Instantiate(hexagonSample, hexagonLocation[i, j], Quaternion.identity) as GameObject;
                            hexagonObject[i, j].GetComponent<SpriteRenderer>().color = hexagonColor[randomColor];
                            hexagonObject[i, j].GetComponent<HexagonController>().enabled = true;
                            hexCount++;
                            if (bombReady > 0)
                            {
                                //Debug.Log("AlsoHere");
                                hexCon.isBomb = true;
                                bombReady--;
                            }
                        }
                    }
                    if (j % 2 != 0)
                    {
                        b += 0.5f;
                    }
                    a += 0.9f;
                }
                a = xPosition;
                b++;
            }
            b = yPosition;
        }
    }

    private void DestroyOthers()
    {
        if (totalHex == hexCount && moveDone)
        {
            tripleCheck = true;
            moveDone = false;
            for (i = 0; i < line; i++)
            {
                for (j = 0; j < column; j++)
                {
                    if (j % 2 != 0)
                    {
                        b -= 0.5f;
                    }
                    hexagonLocation[i, j] = new Vector3(a, b, hexagonLocation[i, j].z);
                    RaycastHit2D checkHit = Physics2D.Raycast(hexagonLocation[i, j], Vector2.zero);
                    if (checkHit)
                    {
                        isDestroy = false;
                        hexagonObject[i, j] = checkHit.transform.gameObject;
                        hexCon = hexagonObject[i, j].GetComponent<HexagonController>();
                        hexagonObject[i, j].transform.position = checkHit.transform.position;
                        hexagonObject[i, j].transform.eulerAngles = new Vector3(0, 0, 0);
                        CheckHits();
                        if (isDestroy)
                        {
                            tripleCheck = false;
                            hexCon.isDestroy = true;
                            moveDone = true;
                        }
                    }
                    if (j % 2 != 0)
                    {
                        b += 0.5f;
                    }
                    a += 0.9f;
                }
                a = xPosition;
                b++;
            }
            b = yPosition;
            moveDone = false;
        }
    }

    private void CheckHits()
    {
        RaycastHit2D hit1 = Physics2D.Raycast(new Vector2(hexagonObject[i, j].transform.position.x + 1f, hexagonObject[i, j].transform.position.y + 0.5f), Vector2.zero, hexagonLayer);
        if (hit1)
        {
            set1 = hit1.transform;
            edge1 = set1.gameObject;
            sR1 = set1.GetComponent<SpriteRenderer>();
        }
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(hexagonObject[i, j].transform.position.x + 0f, hexagonObject[i, j].transform.position.y + 1f), Vector2.zero, hexagonLayer);
        if (hit2)
        {
            set2 = hit2.transform;
            edge2 = set2.gameObject;
            sR2 = set2.GetComponent<SpriteRenderer>();
        }
        RaycastHit2D hit3 = Physics2D.Raycast(new Vector2(hexagonObject[i, j].transform.position.x - 1f, hexagonObject[i, j].transform.position.y + 0.5f), Vector2.zero, hexagonLayer);
        if (hit3)
        {
            set3 = hit3.transform;
            edge3 = set3.gameObject;
            sR3 = set3.GetComponent<SpriteRenderer>();
        }
        RaycastHit2D hit4 = Physics2D.Raycast(new Vector2(hexagonObject[i, j].transform.position.x - 1f, hexagonObject[i, j].transform.position.y - 0.5f), Vector2.zero, hexagonLayer);
        if (hit4)
        {
            set4 = hit4.transform;
            edge4 = set4.gameObject;
            sR4 = set4.GetComponent<SpriteRenderer>();
        }
        RaycastHit2D hit5 = Physics2D.Raycast(new Vector2(hexagonObject[i, j].transform.position.x + 0f, hexagonObject[i, j].transform.position.y - 1f), Vector2.zero, hexagonLayer);
        if (hit5)
        {
            set5 = hit5.transform;
            edge5 = set5.gameObject;
            sR5 = set5.GetComponent<SpriteRenderer>();
        }
        RaycastHit2D hit6 = Physics2D.Raycast(new Vector2(hexagonObject[i, j].transform.position.x + 1f, hexagonObject[i, j].transform.position.y - 0.5f), Vector2.zero, hexagonLayer);
        if (hit6)
        {
            set6 = hit6.transform;
            edge6 = set6.gameObject;
            sR6 = set6.GetComponent<SpriteRenderer>();
        }
        SpriteRenderer sR = hexagonObject[i, j].GetComponent<SpriteRenderer>();
        if (edge1 && sR.color == sR1.color)
        {
            if (edge2 && sR.color == sR2.color)
            {
                isDestroy = true;
                //Debug.Log("1-2");
            }
            if (edge6 && sR.color == sR6.color)
            {
                isDestroy = true;
                //Debug.Log("6-1");
            }
        }
        if (edge2 && sR.color == sR2.color)
        {
            if (edge3 && sR.color == sR3.color)
            {
                isDestroy = true;
                //Debug.Log("2-3");
            }
        }
        if (edge3 && sR.color == sR3.color)
        {
            if (edge4 && sR.color == sR4.color)
            {
                isDestroy = true;
                //Debug.Log("3-4");
            }
        }
        if (edge4 && sR.color == sR4.color)
        {
            if (edge5 && sR.color == sR5.color)
            {
                isDestroy = true;
                //Debug.Log("4-5");
            }
        }
        if (edge5 && sR.color == sR5.color)
        {
            if (edge6 && sR.color == sR6.color)
            {
                isDestroy = true;
                //Debug.Log("5-6");
            }
        }
        edge1 = GetComponent<GameObject>();
        edge2 = GetComponent<GameObject>();
        edge3 = GetComponent<GameObject>();
        edge4 = GetComponent<GameObject>();
        edge5 = GetComponent<GameObject>();
        edge6 = GetComponent<GameObject>();
    }

    private void AssignNew()
    {
        RaycastHit2D hit = Physics2D.Raycast(selector.transform.position, Vector2.zero);
        RaycastHit2D hit1 = Physics2D.Raycast(selector1.transform.position, Vector2.zero);
        RaycastHit2D hit2 = Physics2D.Raycast(selector2.transform.position, Vector2.zero);
        choose = hit.transform;
        choose1 = hit1.transform;
        choose2 = hit2.transform;
        if(choose)
        {
            choose.transform.position = new Vector3(choose.transform.position.x, choose.transform.position.y, -2);
        }
        if(choose1)
        {
            choose1.transform.position = new Vector3(choose1.transform.position.x, choose1.transform.position.y, -2);
        }
        if(choose2)
        {
            choose2.transform.position = new Vector3(choose2.transform.position.x, choose2.transform.position.y, -2);
        }
    }

    private void ResetBack()
    {
        turnCount = 1;
        isMoving = false;
        isRotating = false;
        rotationLock = false;
        isMoveSet = false;
        moveLeft = false;
        moveUp = false;
        moveRight = false;
        moveDown = false;
        rotationHit = new RaycastHit2D();
        rotationReverseHit = new RaycastHit2D();
        rotationClock = 0;
        stopTurn = false;
        if (choose)
        {
            choose.transform.parent = null;
        }
        if (choose1)
        {
            choose1.transform.parent = null;
        }
        if (choose2)
        {
            choose2.transform.parent = null;
        }
        selector.transform.parent = null;
        selector1.transform.parent = null;
        selector2.transform.parent = null;
        resetParameters = false;
        //RESET POSITION
        selector.transform.position = selectorPosTemp;
        selector1.transform.position = selector1PosTemp;
        selector2.transform.position = selector2PosTemp;
        //RESET ROTATION
        selector.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        selector1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        selector2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        originPoint.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        selector.GetComponent<SpriteRenderer>().enabled = true;
        selector1.GetComponent<SpriteRenderer>().enabled = true;
        selector2.GetComponent<SpriteRenderer>().enabled = true;
        originPoint.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void DrawScore()
    {
        Vector3 worldPosition = new Vector3(downBlock.transform.position.x, leftBlock.GetComponent<SpriteRenderer>().bounds.size.y * 0.7f, -5);
        Vector3 textPosition = Camera.main.WorldToScreenPoint(worldPosition);
        scoreText.transform.position = textPosition;
        scorePoint = destroyCount * 5;
        scoreText.text = "Score:" + scorePoint.ToString();
        if (scorePoint >= newTarget)
        {
            //Debug.Log("Here");
            bombReady++;
            newTarget += bombCounter;
        }

    }

    private void RoutineCheck()
    {
        canDestroy = false;
        //Debug.Log("Here");
        RoutineLoop();

        //GAME OVER CONDITION
        if (!canDestroy)
        {
            if(startState)
            {
                SceneManager.LoadScene(1, LoadSceneMode.Single);
            }
            else
            {
                gameOver = true;
                //Debug.Log("Game Over");
                SceneManager.LoadScene(3, LoadSceneMode.Single);
            }
        }
        else
        {
            canDestroy = false;
        }
    }

    private void RoutineLoop()
    {
        for (i = 0; i < line; i++)
        {
            for (j = 0; j < column; j++)
            {
                hexCon = hexagonObject[i, j].GetComponent<HexagonController>();
                if (j % 2 == 1)
                {
                    if (i != 0 && j != 0 && hexagonObject[i - 1, j] && hexagonObject[i - 1, j - 1])
                    {
                        //Debug.Log(string.Format("MO1, i: {0} j: {1}", i, j));
                        Method1Odd();
                    }
                    else
                    if (i != 0 && j != 0 && hexagonObject[i - 1, j - 1] && hexagonObject[i, j - 1])
                    {
                        //Debug.Log(string.Format("MO2, i: {0} j: {1}", i, j));
                        Method2Odd();
                    }
                    if (i != line - 1 && j != 0 && hexagonObject[i, j - 1] && hexagonObject[i + 1, j])
                    {
                        //Debug.Log(string.Format("MO3, i: {0} j: {1}", i, j));
                        Method3Odd();
                    }
                    if (i != line - 1 && j != column - 1 && hexagonObject[i + 1, j] && hexagonObject[i, j + 1])
                    {
                        //Debug.Log(string.Format("MO4, i: {0} j: {1}", i, j));
                        Method4Odd();
                    }
                    if (i != 0 && j != column - 1 && hexagonObject[i, j + 1] && hexagonObject[i - 1, j + 1])
                    {
                        //Debug.Log(string.Format("MO5, i: {0} j: {1}", i, j));
                        Method5Odd();
                    }
                    if (i != 0 && j != column - 1 && hexagonObject[i - 1, j + 1] && hexagonObject[i - 1, j])
                    {
                        //Debug.Log(string.Format("MO6, i: {0} j: {1}", i, j));
                        Method6Odd();
                    }
                }
                else if (j % 2 == 0)
                {
                    if (i != 0 && j != 0 && hexagonObject[i - 1, j] && hexagonObject[i, j - 1])
                    {
                        //Debug.Log(string.Format("ME1, i: {0} j: {1}", i, j));
                        Method1Even();
                    }
                    if (i != line - 1 && j != 0 && hexagonObject[i, j - 1] && hexagonObject[i + 1, j - 1])
                    {
                        //Debug.Log(string.Format("ME2, i: {0} j: {1}", i, j));
                        Method2Even();
                    }
                    if (i != line - 1 && j != 0 && hexagonObject[i + 1, j - 1] && hexagonObject[i + 1, j])
                    {
                        //Debug.Log(string.Format("ME3, i: {0} j: {1}", i, j));
                        Method3Even();
                    }
                    if (i != line - 1 && j != column - 1 && hexagonObject[i + 1, j] && hexagonObject[i + 1, j + 1])
                    {
                        //Debug.Log(string.Format("ME4, i: {0} j: {1}", i, j));
                        Method4Even();
                    }
                    if (i != line - 1 && j != column - 1 && hexagonObject[i + 1, j + 1] && hexagonObject[i, j + 1])
                    {
                        //Debug.Log(string.Format("ME5, i: {0} j: {1}", i, j));
                        Method5Even();
                    }
                    if (i != 0 && j != column - 1 && hexagonObject[i, j + 1] && hexagonObject[i - 1, j])
                    {
                        //Debug.Log(string.Format("ME6, i: {0} j: {1}", i, j));
                        Method6Even();
                    }
                }
                if(canDestroy)
                {
                    break;
                }
            }
            if (canDestroy)
            {
                break;
            }
        }
    }

    private void Method1Odd()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i - 1, j];
        cObject = hexagonObject[i - 1, j - 1];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(-1, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(-1, -1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();
    }

    private void Method2Odd()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i - 1, j - 1];
        cObject = hexagonObject[i, j - 1];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(-1, -1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(0, -1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();
    }

    private void Method3Odd()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i, j - 1];
        cObject = hexagonObject[i + 1, j];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(0, -1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(1, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();
    }

    private void Method4Odd()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i + 1, j];
        cObject = hexagonObject[i, j + 1];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(1, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(0, 1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

    }

    private void Method5Odd()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i, j + 1];
        cObject = hexagonObject[i - 1, j + 1];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(0, 1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(-1, 1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();
    }

    private void Method6Odd()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i - 1, j + 1];
        cObject = hexagonObject[i - 1, j];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(-1, 1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(-1, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();
    }

    private void Method1Even()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i - 1, j];
        cObject = hexagonObject[i, j - 1];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(-1, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(0, -1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();
    }

    private void Method2Even()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i, j - 1];
        cObject = hexagonObject[i + 1, j - 1];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(0, -1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(1, -1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();
    }

    private void Method3Even()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i + 1, j - 1];
        cObject = hexagonObject[i + 1, j];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(1, -1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(1, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();
    }

    private void Method4Even()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i + 1, j];
        cObject = hexagonObject[i + 1, j + 1];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(1, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(1, 1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

    }

    private void Method5Even()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i + 1, j + 1];
        cObject = hexagonObject[i, j + 1];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(1, 1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(0, 1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();
    }

    private void Method6Even()
    {
        aObject = hexagonObject[i, j];
        bObject = hexagonObject[i, j + 1];
        cObject = hexagonObject[i - 1, j];
        aSelect = aObject;
        aChange = new Vector2(0, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(0, 1);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonOdd();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();

        aChange = new Vector2(-1, 0);

        iSafe = i + (int)aChange.x;
        jSafe = j + (int)aChange.y;

        HitComparisonEven();
        
        //Debug.Log(string.Format("turn: {0}", turn));

        SwapObject();
    }

    private void HitComparisonOdd()
    {
        //1
        if (iSafe != 0 && hexagonObject[iSafe - 1, jSafe] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe - 1, jSafe].GetComponent<SpriteRenderer>().color)
        {
            if (jSafe != 0 && hexagonObject[iSafe - 1, jSafe - 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe - 1, jSafe - 1].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("O1, i: {0} j: {1}", i, j));
            }
        }

        //2
        if (iSafe != 0 && jSafe != 0 && hexagonObject[iSafe - 1, jSafe - 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe - 1, jSafe - 1].GetComponent<SpriteRenderer>().color)
        {
            if (hexagonObject[iSafe, jSafe - 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe, jSafe - 1].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("O2, i: {0} j: {1}", i, j));
            }
        }
        //3
        if (jSafe != 0 && hexagonObject[iSafe, jSafe - 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe, jSafe - 1].GetComponent<SpriteRenderer>().color)
        {
            if (iSafe != line - 1 && hexagonObject[iSafe + 1, jSafe] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe + 1, jSafe].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("O3, i: {0} j: {1}", i, j));
            }
        }
        //4
        if (iSafe != line - 1 && hexagonObject[iSafe + 1, jSafe] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe + 1, jSafe].GetComponent<SpriteRenderer>().color)
        {
            if (jSafe != column - 1 && hexagonObject[iSafe, jSafe + 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe, jSafe + 1].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("O4, i: {0} j: {1}", i, j));
            }
        }
        //5
        if (jSafe != column - 1 && hexagonObject[iSafe, jSafe + 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe, jSafe + 1].GetComponent<SpriteRenderer>().color)
        {
            if (iSafe != 0 && hexagonObject[iSafe - 1, jSafe + 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe - 1, jSafe + 1].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("O5, i: {0} j: {1}", i, j));
            }
        }
        //6
        if (iSafe != 0 && jSafe != column - 1 && hexagonObject[iSafe - 1, jSafe + 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe - 1, jSafe + 1].GetComponent<SpriteRenderer>().color)
        {
            if (hexagonObject[iSafe - 1, jSafe] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe - 1, jSafe].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("O6, i: {0} j: {1}", i, j));
            }
        }
        //Debug.Log("Can Destroy: " + canDestroy);
    }

    private void HitComparisonEven()
    {
        //1
        if (iSafe != 0 && hexagonObject[iSafe - 1, jSafe] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe - 1, jSafe].GetComponent<SpriteRenderer>().color)
        {
            if (jSafe != 0 && hexagonObject[iSafe, jSafe - 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe, jSafe - 1].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("E1, i: {0} j: {1}", i, j));
            }
        }

        //2
        if (jSafe != 0 && hexagonObject[iSafe, jSafe - 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe, jSafe - 1].GetComponent<SpriteRenderer>().color)
        {
            if (iSafe != line - 1 && hexagonObject[iSafe + 1, jSafe - 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe + 1, jSafe - 1].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("E2, i: {0} j: {1}", i, j));
            }
        }
        //3
        if (iSafe != line - 1 && jSafe != 0 && hexagonObject[iSafe + 1, jSafe - 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe + 1, jSafe - 1].GetComponent<SpriteRenderer>().color)
        {
            if (hexagonObject[iSafe + 1, jSafe] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe + 1, jSafe].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("E3, i: {0} j: {1}", i, j));
            }
        }
        //4
        if (iSafe != line - 1 && hexagonObject[iSafe + 1, jSafe] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe + 1, jSafe].GetComponent<SpriteRenderer>().color)
        {
            if (jSafe != column - 1 && hexagonObject[iSafe + 1, jSafe + 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe + 1, jSafe + 1].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("E4, i: {0} j: {1}", i, j));
            }
        }
        //5
        if (iSafe != line - 1 && jSafe != column - 1 && hexagonObject[iSafe + 1, jSafe + 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe + 1, jSafe + 1].GetComponent<SpriteRenderer>().color)
        {
            if (hexagonObject[iSafe, jSafe + 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe, jSafe + 1].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("E5, i: {0} j: {1}", i, j));
            }
        }
        //6
        if (jSafe != column - 1 && hexagonObject[iSafe, jSafe + 1] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe, jSafe + 1].GetComponent<SpriteRenderer>().color)
        {
            if (iSafe != 0 && hexagonObject[iSafe - 1, jSafe] && aSelect.GetComponent<SpriteRenderer>().color == hexagonObject[iSafe - 1, jSafe].GetComponent<SpriteRenderer>().color)
            {
                canDestroy = true;
                //Debug.Log(string.Format("E6, i: {0} j: {1}", i, j));
            }
        }
        //Debug.Log("Can Destroy: " + canDestroy);
    }

    private void SwapObject()
    {
        GameObject tempObject = new GameObject();
        tempObject.AddComponent<SpriteRenderer>();
        tempObject.GetComponent<SpriteRenderer>().color = cObject.GetComponent<SpriteRenderer>().color;
        cObject.GetComponent<SpriteRenderer>().color = bObject.GetComponent<SpriteRenderer>().color;
        bObject.GetComponent<SpriteRenderer>().color = aObject.GetComponent<SpriteRenderer>().color;
        aObject.GetComponent<SpriteRenderer>().color = tempObject.GetComponent<SpriteRenderer>().color;
        //Debug.Log(string.Format("COLORS A: {0} B: {1}, C: {2}", aObject.GetComponent<SpriteRenderer>().color, bObject.GetComponent<SpriteRenderer>().color, cObject.GetComponent<SpriteRenderer>().color));
        if (aSelect == aObject)
        {
            aSelect = bObject;
        }
        else if (aSelect == bObject)
        {
            aSelect = cObject;
        }
        else if (aSelect == cObject)
        {
            aSelect = aObject;
        }
        //Debug.Log(aSelect.transform.position);
    }

}