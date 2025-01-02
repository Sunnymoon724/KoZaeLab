using System;
using KZLib;
using KZLib.KZUtility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Test : MonoBehaviour
{
	public string m_path = null;

	[Button("Test1")]
    void Test1()
    {
		// LogTag.Build.I("Absolute_True: "+CommonUtility.GetAbsolutePath(m_path,true));

		// LogTag.Build.I("Assets: "+CommonUtility.GetAssetsPath(m_path));

		// LogTag.Build.I("Local: "+CommonUtility.GetLocalPath(m_path));
    }
}