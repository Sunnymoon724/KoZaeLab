using System;
using System.Collections.Generic;
using KZLib;
using KZLib.KZUtility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public interface ITest
{

}

public class TestA : ITest
{
	public int AAA { get;set; }
}

public class Test : MonoBehaviour
{
	[SerializeReference]
    private List<ITest> m_testList = new();

	[Button("Add")]
    private void AddTest()
    {
        m_testList.Add(new TestA());


        var con = ConfigMgr.In.Access<ConfigData.GameConfig>();
    }
}