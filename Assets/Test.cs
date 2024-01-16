using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{

    [Button(SdfIconType.PlayFill)]
    private void AAAA()
    {
        var colorArray = new Color32[1920*1080];

        for(int i = 0; i < colorArray.Length; i++)
        {
            colorArray[i] = new Color32((byte)Tools.GetRndInt(0,255),(byte)Tools.GetRndInt(0,255),(byte)Tools.GetRndInt(0,255),(byte)Tools.GetRndInt(0,255));
        }

         // 원본 데이터 생성
        byte[] originalData = ConvertColorArrayToByteArray(colorArray);

        // 압축 전 데이터 크기 출력
        Debug.Log($"Original Data Size: {originalData.Length} bytes");

        // 데이터 압축
        byte[] compressedData = CompressData(originalData);

        // 압축 후 데이터 크기 출력
        Debug.Log($"Compressed Data Size: {compressedData.Length} bytes");

        // 압축 해제
        byte[] decompressedData = DecompressData(compressedData);

        // 압축 해제 후 데이터 크기 출력 (압축 해제가 정상적으로 이루어졌는지 확인)
        Debug.Log($"Decompressed Data Size: {decompressedData.Length} bytes");

        var result = ConvertByteArrayToColor32Array(decompressedData);

        for(int i = 0; i < result.Length; i++)
        {
            if(!result[i].Equals(colorArray[i]))
            {
                Debug.Log("111");
            }
        }
    }

    byte[] ConvertBoolArrayToByteArray(bool[] boolArray)
    {
        int byteCount = (boolArray.Length + 7) / 8; // bool 배열의 길이에 따라 필요한 바이트 개수 계산
        byte[] byteArray = new byte[byteCount];

        for (int i = 0; i < boolArray.Length; i++)
        {
            int byteIndex = i / 8;
            int bitIndex = i % 8;

            if (boolArray[i])
            {
                // 해당하는 비트를 1로 설정
                byteArray[byteIndex] |= (byte)(1 << bitIndex);
            }
            // 기본적으로 0으로 설정되어 있으므로 별도의 조건 없음
        }

        return byteArray;
    }

    bool[] ConvertByteArrayToBoolArray(byte[] byteArray)
    {
        bool[] boolArray = new bool[byteArray.Length * 8];

        for (int i = 0; i < byteArray.Length; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                int bitIndex = i * 8 + j;
                boolArray[bitIndex] = (byteArray[i] & (1 << j)) != 0;
            }
        }

        return boolArray;
    }

    byte[] ConvertColorArrayToByteArray(Color32[] colors)
    {
        // 각 Color32를 바이트로 변환하여 새로운 바이트 배열에 저장
        byte[] byteArray = new byte[colors.Length * 4]; // 각 Color32가 4바이트이므로 곱하기 4

        for (int i = 0; i < colors.Length; i++)
        {
            int startIndex = i * 4;
            byteArray[startIndex] = colors[i].r;
            byteArray[startIndex + 1] = colors[i].g;
            byteArray[startIndex + 2] = colors[i].b;
            byteArray[startIndex + 3] = colors[i].a;
        }

        return byteArray;
    }

    Color32[] ConvertByteArrayToColor32Array(byte[] byteArray)
    {
        // 각각의 바이트를 사용하여 Color32 배열 생성
        Color32[] colorArray = new Color32[byteArray.Length / 4];

        for (int i = 0; i < colorArray.Length; i++)
        {
            int startIndex = i * 4;

            byte r = byteArray[startIndex];
            byte g = byteArray[startIndex + 1];
            byte b = byteArray[startIndex + 2];
            byte a = byteArray[startIndex + 3];

            colorArray[i] = new Color32(r, g, b, a);
        }

        return colorArray;
    }

    byte[] CompressData(byte[] data)
    {
        using (MemoryStream compressedStream = new MemoryStream())
        {
            using (GZipStream compressionStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                compressionStream.Write(data, 0, data.Length);
            }
            return compressedStream.ToArray();
        }
    }

    byte[] DecompressData(byte[] compressedData)
{
    using (MemoryStream compressedStream = new MemoryStream(compressedData))
    {
        using (GZipStream decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        {
            using (MemoryStream decompressedStream = new MemoryStream())
            {
                decompressionStream.CopyTo(decompressedStream);
                return decompressedStream.ToArray();
            }
        }
    }
}
}

