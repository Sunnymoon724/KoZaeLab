using KZLib;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        int result = KZLib.CommonUtility.LoopClamp(5, 10);
        Debug.Log(result);
    }
}