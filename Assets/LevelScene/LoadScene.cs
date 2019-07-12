using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int lineNum;

    public void playScene(int buttonNum)
    {
        if (buttonNum == 3)
        {
            lineNum = 3;
            SceneManager.LoadScene("Puzzle");
        }
        if (buttonNum == 4)
        {
            lineNum = 4;
            SceneManager.LoadScene("Puzzle");
        }
        if (buttonNum == 5)
        {
            lineNum = 5;
            SceneManager.LoadScene("Puzzle");
        }
    }
}
