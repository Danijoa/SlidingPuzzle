using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumArray: MonoBehaviour 
{
    public NumArray parent;

    public int[,] myArray;
    public int blockLine;

    public Vector2 direction;

    public void MakeLine(int line)
    {
        blockLine = line;
        myArray = new int[blockLine, blockLine];
    }

    public void MakeArray(int x, int y, int num)
    {
        myArray[x, y] = num;
    }

    //블럭 움직인 횟수
    public int gCost;
    //현재~타겟 멘헤튼 거리
    public int hCost;
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
