using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreController : MonoBehaviour
{
    static private int yourScore = 0;
    
    void Start()
    {
    }

 
    void Update()
    {
        if(GameObject.Find("Selectioner"))
        {
            GameObject.Find("Selectioner").GetComponent<SelectionController>().enabled = true;
            yourScore = GameObject.Find("Selectioner").GetComponent<SelectionController>().destroyCount * 5;
        }
        if (GameObject.Find("YourScoreText"))
        {
            GameObject.Find("YourScoreText").GetComponent<Text>().text = yourScore.ToString();
        }
        //Debug.Log("YourScore =" + yourScore);
    }
}
