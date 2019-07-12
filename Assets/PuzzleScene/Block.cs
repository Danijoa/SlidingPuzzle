using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    //원본 이미지
    public Texture2D myImage;

    int blockLine;

    public int shuffleTime = 10;

    EventBlock emptyBlock;
    EventBlock eventBlock;
    //셔플 블럭 배열
    EventBlock[,] shuffleB;

    //처음,타겟 숫자 배열
    NumArray targetArray;
    //변화하는 숫자 배열 = Auto를 눌렀을때 처음 검사하게 되는 숫자 배열
    NumArray numArray;

    Vector2 previousD;
    int remainShuffle;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI solText;

    void Start()
    {
        blockLine = LoadScene.lineNum;
        levelText.text = blockLine + "X" + blockLine;

        //카메라 화면 고정
        Camera.main.orthographicSize = blockLine;
        CreateBlock();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartShuffle();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartFind();
        }
    }

    //블럭 배열 생성----------------------------------------------
    void CreateBlock()
    {
        shuffleB = new EventBlock[blockLine,blockLine];
        //초기 번호 배열
        targetArray = gameObject.AddComponent<NumArray>();
        targetArray.MakeLine(blockLine);
        for (int y = 0; y < blockLine; y++)
        {
            for (int x = 0; x < blockLine; x++)
            {
                int myNum = x + (blockLine * y) + 1;
                targetArray.MakeArray(x, y, myNum);
            }
        }
        numArray = gameObject.AddComponent<NumArray>();
        numArray.MakeLine(blockLine);

        //분할 이미지
        Texture2D[,] Image = SetImage.ImageBlock(myImage, blockLine);

        for (int y = 0; y < blockLine; y++)
        {
            for (int x = 0; x < blockLine; x++)
            {
                //Quad로 블럭 생성
                GameObject myBlock = GameObject.CreatePrimitive(PrimitiveType.Quad);
                myBlock.transform.localPosition = new Vector2(-0.5f * (blockLine - 1), -0.5f * (blockLine - 1) -1) + new Vector2(x, y);

                //이벤트 설정
                eventBlock = myBlock.AddComponent<EventBlock>();
                //좌표, 이미지 설정
                eventBlock.Init(new Vector2(x, y), Image[x, y]);
                //셔플 배열에 블럭 넣기
                shuffleB[x, y] = eventBlock;

                //번호 배열 정보
                int myNum = x + (blockLine * y) + 1;
                //변화하는 번호 배열
                numArray.MakeArray(x, y, myNum);

                //초기 비어있는 블럭
                if (x == blockLine - 1 && y == 0)
                {
                    myBlock.SetActive(false);
                    emptyBlock = eventBlock;
                }

                //마우스 입력이 일어나면
                eventBlock.BlockPressed += MoveBlock;
            }
        }
    }

    //블럭 교환----------------------------------------------
    void MoveBlock(EventBlock eventBlock)
    {
        //인접블럭만 처리
        if ((eventBlock.coord - emptyBlock.coord).sqrMagnitude == 1.0f)
        {
            //변화하는 셔플 블럭 배열
            if (remainShuffle > 0)
            {
                shuffleB[(int)eventBlock.coord.x, (int)eventBlock.coord.y] = emptyBlock;
                shuffleB[(int)emptyBlock.coord.x, (int)emptyBlock.coord.y] = eventBlock;
            }

            //좌표 교환
            Vector2 tempCoord = emptyBlock.coord;
            emptyBlock.coord = eventBlock.coord;
            eventBlock.coord = tempCoord;

            //이미지 교환 
            Vector2 tempPosition = emptyBlock.transform.position;
            emptyBlock.transform.position = eventBlock.transform.position;
            eventBlock.SmoothMove(tempPosition, 0.3f);

            //블럭 번호 교환
            int tempNum = numArray.myArray[(int)emptyBlock.coord.x, (int)emptyBlock.coord.y];
            numArray.myArray[(int)emptyBlock.coord.x, (int)emptyBlock.coord.y] = numArray.myArray[(int)eventBlock.coord.x, (int)eventBlock.coord.y];
            numArray.myArray[(int)eventBlock.coord.x, (int)eventBlock.coord.y] = tempNum;

            //움직인 블럭 번호 출력
            print(numArray.myArray[(int)eventBlock.coord.x, (int)eventBlock.coord.y]);
            print(numArray.myArray[(int)emptyBlock.coord.x, (int)emptyBlock.coord.y]);
        }
        if (remainShuffle > 0)
            Invoke("ShuffleBlock", 0.3f);
    }

    //셔플----------------------------------------------
    public void StartShuffle()
    {
        remainShuffle = shuffleTime;
        ShuffleBlock();
    }

    void ShuffleBlock()
    {
        //오 왼 위 아래
        Vector2[] vecD = { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) };
        int randomD = Random.Range(0, vecD.Length);

        for (int i = 0; i < vecD.Length; i++)
        {
            //이동할 수 없는 방향으로 random이 걸렸을 경우 대비
            Vector2 direction = vecD[(randomD + i) % vecD.Length];
            //원래 있던 자리가 아닐때
            if (direction != -1 * previousD)
            {
                Vector2 randomMoveCoord = emptyBlock.coord + direction;

                if (randomMoveCoord.x >= 0 && randomMoveCoord.x < blockLine
                    && randomMoveCoord.y >= 0 && randomMoveCoord.y < blockLine)
                {
                    remainShuffle--;
                    MoveBlock(shuffleB[(int)randomMoveCoord.x, (int)randomMoveCoord.y]);
                    previousD = direction;
                    break;
                }
            }
        }
    }

    //길 찾기----------------------------------------------
    public void StartFind()
    {
        //Auto 눌렀을 때 번호 배열, 타겟 번호 배열
        FindPath(numArray, targetArray);
    }

    void FindPath(NumArray startArray, NumArray tar_Array)
    {
        print("FindPath Start!");
        //시작 숫자 배열
        startArray.gCost = 0;
        startArray.hCost = GetHCost(startArray);
        //타겟 숫자 배열
        NumArray targetArray = tar_Array;

        //확인 필요한 배열들
        List<NumArray> openSet = new List<NumArray>();
        //확인 했던 배열들
        //HashSet<NumArray> closedSet = new HashSet<NumArray>();
        List<NumArray> closedSet = new List<NumArray>();
        openSet.Add(startArray);

        while (openSet.Count > 0)
        {
            //openSet에서 가장 작은 fCost 찾기
            NumArray currArray = openSet[0];
            for (int i = 0; i < openSet.Count; i++)
            {
                if ((GetFCost(openSet[i]) < GetFCost(currArray)) ||
                    (GetFCost(openSet[i]) == GetFCost(currArray) && openSet[i].hCost < currArray.hCost))
                {
                    currArray = openSet[i];
                }
            }
            //선택된 배열 openSet에서 삭제
            openSet.Remove(currArray);
            //선택된 배열 closedSet에 삽입
            closedSet.Add(currArray);

            int checkFin = 0;
            for (int y = 0; y < blockLine; y++)
            {
                for (int x = 0; x < blockLine; x++)
                {
                    if (currArray.myArray[x, y] == targetArray.myArray[x, y])
                        checkFin++;
                }
            }

            //타겟과 일치하면 정지
            if (checkFin == blockLine*blockLine)
            {
                print("FindPath End!");
                RetracePath(startArray, currArray);
                return;
            }

            //이동 할 수 있는 방향으로 움직인 배열
            List<NumArray> getChild = GetChild(currArray);
            foreach (NumArray childArray in getChild)
            {
                for (int i = 0; i < closedSet.Count; i++)
                {
                    if (closedSet[i].myArray == childArray.myArray)
                    {
                        continue;
                    }
                }

                bool check = true;
                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].myArray == childArray.myArray)
                    {
                        check = false;
                        break;
                    }
                }
                
                if (check == true)
                {
                    childArray.gCost = currArray.gCost + 1;
                    childArray.hCost = GetHCost(childArray);
                    childArray.parent = currArray;

                    openSet.Add(childArray);
                }
            }
        }
    }

    int GetFCost(NumArray openSet)
    {
        int fCost = 0;
        fCost = openSet.gCost + openSet.hCost;
        return fCost;
    }

    void RetracePath(NumArray startNode, NumArray endArray)
    {
        List<Vector2> path = new List<Vector2>();
        NumArray currentArray = gameObject.AddComponent<NumArray>();
        currentArray = endArray;

        while (true)
        {
            int checkFin = 0;
            for (int y = 0; y < blockLine; y++)
            {
                for (int x = 0; x < blockLine; x++)
                {
                    if (currentArray.myArray[x, y] == startNode.myArray[x, y])
                        checkFin++;
                }
            }
            if (checkFin == blockLine*blockLine)
                break;

            path.Add(currentArray.direction);
            currentArray = currentArray.parent;
        }
        path.Reverse();

        Vector2 right = new Vector2(1, 0);
        Vector2 left = new Vector2(-1, 0);
        Vector2 up = new Vector2(0, 1);
        Vector2 down = new Vector2(0, -1);

        List<string> saveSol = new List<string>();
        for (int i = 0; i < path.Count; i++)
        {
            if (path[i] == right)
            {
                saveSol.Add("Right ");
                print("right");
            }
            if (path[i] == left)
            {
                saveSol.Add("Left ");
                print("left");
            }
            if (path[i] == up)
            {
                saveSol.Add("Up ");
                print("up");
            }
            if (path[i] == down)
            {
                saveSol.Add("Down ");
                print("down");
            }
        }

        string solution = "Solution: ";
        foreach (string sol in saveSol)
        {
            solution = solution.ToString() + sol.ToString();
        }
        solText.text = solution;
    }

    int GetHCost(NumArray childArray)
    {
        int hCost = 0;
        for (int curY = 0; curY < blockLine; curY++)
        {
            for (int curX = 0; curX < blockLine; curX++)
            {
                int num = childArray.myArray[curX, curY];
                int tarX = (num - 1) % blockLine;
                int tarY = (num - 1) / blockLine;
                hCost += (Mathf.Abs(tarX - curX) + Mathf.Abs(tarY - curY));
            }
        }
        return hCost;
    }

    public bool checkFirst = true;
    public int count = 0;
    public NumArray[] tempArray;
    public List<NumArray> GetChild(NumArray currArray)
    {
        List<NumArray> childArray = new List<NumArray>();

        int emptyX = 0;
        int emptyY = 0;

        for (int y = 0; y < blockLine; y++)
        {
            for (int x = 0; x < blockLine; x++)
            {
                //번호 blockLine(3)이 비어있는 블럭
                if (currArray.myArray[x, y] == blockLine)
                {
                    emptyX = x;
                    emptyY = y;
                }
            }
        }

        if (checkFirst == true)
        {
            tempArray = new NumArray[1000];
            for (int i = 0; i < 1000; i++)
            {
                tempArray[i] = gameObject.AddComponent<NumArray>();
                tempArray[i].MakeLine(blockLine);
            }
            checkFirst = false;
        }

        int[,] copyCrr = new int[blockLine, blockLine];
        copyCrr = currArray.myArray.Clone() as int[,];
        for (int i = count; i <1000; i++)
        {
            tempArray[i].myArray = copyCrr.Clone() as int[,];
        }

        //오른쪽 왼쪽 위 아래
        Vector2[] vecD = { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) };
        for (int i = 0; i < vecD.Length; i++)
        {
            int checkX = emptyX + (int)vecD[i].x;
            int checkY = emptyY + (int)vecD[i].y;

            if ((checkX >= 0 && checkX < blockLine) && (checkY >= 0 && checkY < blockLine))
            {
                int tempNum = tempArray[count].myArray[emptyX, emptyY];
                tempArray[count].myArray[emptyX, emptyY] = tempArray[count].myArray[checkX, checkY];
                tempArray[count].myArray[checkX, checkY] = tempNum;

                tempArray[count].direction = vecD[i];

                childArray.Add(tempArray[count]);

                count++;
            }
        }
        return childArray;
    }
}
 