using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    private string PATH = "/Users/kozae/Documents/Projects/KoZaeLab/Test";

    [Button("Test")]
    void OnPlayTest()
    {
        FileUtility.Compress(PATH,"/Users/kozae/Documents/Projects/KoZaeLab/Test");
    }
}
