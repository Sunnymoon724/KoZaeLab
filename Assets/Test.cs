using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class Test : MonoBehaviour
{
    void Start()
    {
        var array = new int[100];

        for(int i=0;i<100;i++)
        {
            array[i] = i;
        }

        var data = SplitVerticalTexture(array,2);

        Debug.Log(string.Join("-",array));

        for(int i=0;i<data.Length;i++)
        {
            Debug.Log(string.Join("-",data[i]));
        }        
    }

    public static int[][] SplitVerticalTexture(int[] _array,int _count)
	{
        var oldWidth = 13;
        var colorArray = new int[_count][];
        var widthArray = new int[_count];

        var colorList = new List<List<int>>();

		for(int i=0;i<_count;i++)
		{
            colorArray[i] = new int[100/_count];
            colorList.Add(new List<int>());

            widthArray[i] = i%2 == 0 ? Mathf.CeilToInt(oldWidth/(float)_count) : Mathf.FloorToInt(oldWidth/(float)_count);
		}

		for(int i=0;i<_array.Length;i++)
		{
			var pivot = i%oldWidth;

			for(int j=0;j<_count;j++)
			{
				if(j*widthArray[j] <= pivot && pivot < (j+1)*widthArray[j])
				{
                    colorList[j].Add(_array[i]);
				}
			}
		}

        for(int i=0;i<_count;i++)
		{
            for(int j=0;j<colorArray[i].Length;j++)
            {
                colorArray[i][j] = colorList[i][j];
            }
		}        

		return colorArray;
	}

    // public static Texture2D[] SplitVerticalTexture(Texture2D _texture,int _count)
	// {
	// 	if(_count <= 0)
	// 	{
	// 		return null;
	// 	}

	// 	if(_count == 1)
	// 	{
	// 		return new Texture2D[] { CopyTexture(_texture) };
	// 	}
		
	// 	var resultArray = new Texture2D[_count];
	// 	var colorArray = new Color32[_count][];
	// 	var newWidth = _texture.width/_count;

	// 	for(int i=0;i<_count;i++)
	// 	{
	// 		resultArray[i] = new Texture2D(newWidth,_texture.height,_texture.format,false);

	// 		colorArray[i] = resultArray[i].GetPixels32();
	// 	}

	// 	var pixelArray = _texture.GetPixels32();

	// 	for(int i=0;i<pixelArray.Length;i++)
	// 	{
	// 		var pivot = i%_texture.width;

	// 		for(int j=0;j<_count;j++)
	// 		{
	// 			if(j*newWidth <= pivot && pivot < (j+1)*newWidth)
	// 			{
	// 				colorArray[j][pivot] = pixelArray[i];
	// 			}
	// 		}
	// 	}

	// 	for(int i=0;i<_count;i++)
	// 	{
	// 		resultArray[i].SetPixels32(colorArray[i]);
	// 		resultArray[i].Apply();
	// 	}

	// 	return resultArray;
	// }
}