using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Block blockScript;
    public void MyButtons(int alpha)
    {
        blockScript = FindObjectOfType<Block>();
        if (alpha == 1)
        {
            blockScript.StartShuffle();
        }
        if (alpha == 2)
        {
            blockScript.StartFind();
        }
    }
}
