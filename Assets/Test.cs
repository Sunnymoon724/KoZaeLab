#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Button("Test1")]
    void Text1()
    {
        Debug.Log(Application.targetFrameRate);
        
        LogSvc.Build.I(Application.targetFrameRate);
    }
}
#endif