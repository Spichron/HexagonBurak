using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitController : MonoBehaviour
{
    private GameObject downBlock;
    private GameObject leftBlock;

    private bool isSelected = false;

    private int line = 9;
    private int column = 8;

    private float fLine;
    private float fColumn;
    private float xCamera;
    private float yCamera;

    public int buttonSetup = 0;

    private void Start()
    {
        downBlock = GameObject.Find("DownBlock");
        leftBlock = GameObject.Find("LeftBlock");
        if (SceneManager.GetActiveScene().name != "Game Scene")
        {
            Initials();
        }
    }

    void OnLevelWasLoaded()
    {
        downBlock = GameObject.Find("DownBlock");
        leftBlock = GameObject.Find("LeftBlock");
        if (SceneManager.GetActiveScene().name != "Game Scene")
        {
            Initials();
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Menu Scene")
        {
            transform.position = new Vector3(downBlock.transform.position.x, leftBlock.GetComponent<SpriteRenderer>().bounds.size.y * 0f, -5);
            transform.localScale = new Vector3(downBlock.GetComponent<SpriteRenderer>().bounds.size.x / 2.5f, leftBlock.GetComponent<SpriteRenderer>().bounds.size.y * 0.2f, 1);
        }
        if (SceneManager.GetActiveScene().name == "Over Scene")
        {
            transform.position = new Vector3(downBlock.transform.position.x * 3 / 2, leftBlock.GetComponent<SpriteRenderer>().bounds.size.y * -0.1f, -5);
            transform.localScale = new Vector3(downBlock.GetComponent<SpriteRenderer>().bounds.size.x / 7.5f, leftBlock.GetComponent<SpriteRenderer>().bounds.size.y * 0.1f, 1);
        }
        if (SceneManager.GetActiveScene().name == "Game Scene")
        {
            transform.position = new Vector3(downBlock.transform.position.x * 3 / 2, leftBlock.GetComponent<SpriteRenderer>().bounds.size.y * -0.1f, -5);
            transform.localScale = new Vector3(downBlock.GetComponent<SpriteRenderer>().bounds.size.x / 7.5f, leftBlock.GetComponent<SpriteRenderer>().bounds.size.y * 0.1f, 1);
        }

        GameObject thisTextObject = GameObject.Find("ExitText");
        Text thisText = thisTextObject.GetComponent<Text>();
        Vector3 textPosition = Camera.main.WorldToScreenPoint(transform.position);
        thisText.transform.position = textPosition;
        if (SceneManager.GetActiveScene().name == "Menu Scene")
        {
            thisText.text = "QUIT";
        }
        else if (SceneManager.GetActiveScene().name == "Game Scene")
        {
            thisText.text = "RETURN";
        }
        else
        {
            thisText.text = "RETURN";
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit1 = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);

        if (hit1 && hit1.transform.gameObject == transform.gameObject && Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Here");
            isSelected = true;
            thisText.color += Color.HSVToRGB(0f, 0f, 0.2f);
            GetComponent<SpriteRenderer>().color -= Color.HSVToRGB(0f, 0f, 0.2f);
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1);
        }
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);
        if (hit2 && hit2.transform.gameObject == transform.gameObject && Input.GetMouseButtonUp(0) && isSelected)
        {
            if (SceneManager.GetActiveScene().name == "Menu Scene")
            {
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
        }
        if (Input.GetMouseButtonUp(0) && isSelected)
        {
            isSelected = false;
            thisText.color -= Color.HSVToRGB(0f, 0f, 0.2f);
            GetComponent<SpriteRenderer>().color += Color.HSVToRGB(0f, 0f, 0.2f);
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1);
        }
    }

    private void Initials()
    {
        fLine = line;
        fColumn = column;
        xCamera = 0.6f + 0.45f * (fColumn - 1f);
        yCamera = 0.25f + (fLine / 2f);


        Camera.main.transform.position = new Vector3(xCamera, yCamera, Camera.main.transform.position.z);

        downBlock.transform.position = new Vector3(xCamera, -0.5f, downBlock.transform.position.z);
        leftBlock.transform.position = new Vector3(-0.5f, yCamera, leftBlock.transform.position.z);

         downBlock.transform.localScale = new Vector3(1.7f + 0.9f * (fColumn - 1f), downBlock.transform.localScale.y, downBlock.transform.localScale.z);
        leftBlock.transform.localScale = new Vector3(leftBlock.transform.localScale.x, (1f + fLine) * 1.5f, leftBlock.transform.localScale.z);

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = downBlock.GetComponent<SpriteRenderer>().bounds.size.x / leftBlock.GetComponent<SpriteRenderer>().bounds.size.y;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = leftBlock.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
        Camera.main.orthographicSize = leftBlock.GetComponent<SpriteRenderer>().bounds.size.y / 2 * differenceInSize;
        }
    }
}