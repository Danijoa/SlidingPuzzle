using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SetImage
{
    public static Texture2D[,] ImageBlock(Texture2D myImage, int blockLine)
    {
        //이미지를 정사각형으로 자르기
        int ImageSize = Mathf.Min(myImage.width, myImage.height);
        int ImageBlockSize = ImageSize / blockLine;

        //이미지 블럭들 만들기
        Texture2D[,] myImageBlocks = new Texture2D[blockLine, blockLine];
        for (int y = 0; y < blockLine; y++)
        {
            for (int x = 0; x < blockLine; x++)
            {
                Texture2D myImageBlock = new Texture2D(ImageBlockSize, ImageBlockSize);
                //이미지를 블럭 사이즈로 나눈후, 블럭 크키게 맞추기
                myImageBlock.SetPixels(myImage.GetPixels(x * ImageBlockSize, y * ImageBlockSize, ImageBlockSize, ImageBlockSize));
                myImageBlock.Apply();
                myImageBlocks[x, y] = myImageBlock;
            }
        }

        return myImageBlocks;
    }
}
