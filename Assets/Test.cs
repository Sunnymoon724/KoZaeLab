#if UNITY_EDITOR
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Button("Test1")]
    void Text1()
    {
        
    }
    
    public void Update()
    {
        Debug.Log(Application.targetFrameRate);
    }
}
#endif