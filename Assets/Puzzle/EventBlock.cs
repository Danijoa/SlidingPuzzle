using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBlock : MonoBehaviour
{ 
    public event System.Action<EventBlock> BlockPressed;

    public Vector2 coord;

    //좌표, 이미지
    public void Init(Vector2 initCoord, Texture2D Image)
    {
        //좌표
        coord = initCoord;

        //이미지 렌더
        GetComponent<MeshRenderer>().material.mainTexture = Image;
        //이미지 조명을 끔 
        GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Texture");
    }

    //블럭 부드럽게 이동 
    public void SmoothMove(Vector2 tempPosition, float time)
    {
        StartCoroutine(SmoothMoveBlock(tempPosition, time));        
    }

    IEnumerator SmoothMoveBlock(Vector2 tempPosition, float time)
    {
        Vector2 eventBlockPosition = this.transform.position;
        float t = 0;
        while(t < 1)
        {
            t += Time.smoothDeltaTime / time;
            //eventBlockPosition에서 tempPosition을 향하여
            this.transform.position = Vector2.Lerp(eventBlockPosition, tempPosition, t);
            yield return null;
        }
        //print(this.blockNum);
    }

    //마우스 입력이 일어난 블럭 지정
    void OnMouseDown()
    {
        BlockPressed(this);
    }
}
