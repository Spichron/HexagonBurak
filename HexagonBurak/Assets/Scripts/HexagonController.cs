using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexagonController : MonoBehaviour
{
    private Text bombText;

    private Vector3 bombPosition;

    private GameObject controller;
    private GameObject bombTextObject;
    private GameObject textCanvas;

    private SelectionController selCon;

    private bool createBomb = false;

    //PUBLIC

    public bool isDestroy = false;
    public bool isMoving = false;
    public bool oddColumn = false;
    public bool isBomb = false;

    public Vector3 storedPosition;

    public int bombTimer = 7;

    void Start()
    {
        textCanvas = GameObject.Find("Canvas");
        controller = GameObject.Find("Selectioner");
        selCon = controller.GetComponent<SelectionController>();
    }

    private void Update()
    {
        MoveDown();
        BombAction();
        ThenDestroy();
    }

    private void ThenDestroy() //More like HEXAGONE
    {
        if (isDestroy)
        {
            selCon.hexCount--;
            selCon.destroyCount++;
            Destroy(bombTextObject);
            Destroy(bombText);
            Destroy(gameObject);
        }
    }

    private void MoveDown()
    {
        if (isMoving && ((oddColumn && System.Math.Round(transform.position.y,2) > 0.5f) || (!oddColumn && System.Math.Round(transform.position.y, 2) > 1f)))
        {
            if (System.Math.Round(storedPosition.y, 2) - System.Math.Round(transform.position.y, 2) < 1)
            {
                transform.position += new Vector3(0f, -3f, 0f) * Time.deltaTime;
            }
            else
            {
                transform.position = storedPosition + new Vector3(0f, -1f, 0f);
                isMoving = false;
            }
            if((oddColumn && System.Math.Round(transform.position.y, 2) <= 0.5f) || (!oddColumn && System.Math.Round(transform.position.y, 2) <= 1f))
            {
                isMoving = false;
            }
        }
    }

    private void BombAction()
    {
        if(isBomb)
        {
            if (!createBomb)
            {
                GetComponent<SpriteRenderer>().sprite = selCon.bombSprite;
                bombTextObject = Instantiate(GameObject.Find("BombText")) as GameObject;
                bombTextObject.transform.SetParent(textCanvas.transform);
                bombText = bombTextObject.GetComponent<Text>();
                createBomb = true;
            }

            if (bombTextObject)
            {
                bombPosition = Camera.main.WorldToScreenPoint(transform.position);
                bombText.transform.position = bombPosition;
                bombText.text = bombTimer.ToString();
            }

         

            if (selCon.turnEnd)
            {
                bombTimer--;
            }

            if (bombTimer <= 0)
            {
                selCon.gameOver = true;
            }
        }
    }
}
