using System.Collections;
using System.Collections.Generic;
using KZLib.AttributeDrawer;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField,KZRichTextDisplay]
    private string m_Text;

    // Start is called before the first frame update
    void Start()
    {
        m_Text = "<b>SSABI</b> <i>SSABI</i>";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
