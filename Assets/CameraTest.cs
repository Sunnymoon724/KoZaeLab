using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Cysharp.Threading.Tasks;
using KZLib;
using KZLib.KZAttribute;
using KZLib.KZDevelop;
using MessagePack;
using Newtonsoft.Json;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class CameraTest : MonoBehaviour
{
    [Serializable]
    private class CaTest1
    {
        public int a;
        public int B { get; set; }
        public List<int> c;
    }

    [MessagePackObject]
    public class CaTest2
    {
        [Key(0)]
        public int a;

        [Key(1)]
        public int B { get; set; }

        [Key(2)]
        public List<int> c;
    }

    [Button("Start")]
    private void TestTest()
    {
        var data = new CaTest2 {  a = 1, B = 2, c = new List<int> { 1, 2, 3, 4, 5 }, };
        var count = 1;
        var stopwatch = new Stopwatch();

        stopwatch.Restart();

        for(var i=0;i<count;i++) 
        {
            var serialized1 = JsonConvert.SerializeObject(data);
            var deserialized1 = JsonConvert.DeserializeObject<CaTest2>(serialized1);
        }

        stopwatch.Stop();

        LogTag.Data.I($"T : {stopwatch.Elapsed.TotalSeconds}");

        stopwatch.Restart();

        for(var i=0;i<count;i++) 
        {
            var serialized2 = MessagePackSerializer.Serialize(data);
            var deserialized2 = MessagePackSerializer.Deserialize<CaTest2>(serialized2);
        }

        stopwatch.Stop();

        LogTag.Data.I($"T : {stopwatch.Elapsed.TotalSeconds}");
    }
}