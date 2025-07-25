#if UNITY_EDITOR
using System.Reflection;
using KZLib.KZData;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Button("Test1")]
    void Text1()
    {
        HeroProto heroProto = new HeroProto();
        
        MotionProto motionProto = new MotionProto();
        
    }
    
    public void Update()
    {
        Debug.Log(Application.targetFrameRate);
    }
}
#endif