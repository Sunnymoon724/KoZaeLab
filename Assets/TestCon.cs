using KZLib;
using UnityEngine;

public class TestCon : MonoBehaviour
{
    [SerializeField]
    private string m_ObjectPath = null;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ResMgr.In.GetObject(m_ObjectPath);
        }
    }
}