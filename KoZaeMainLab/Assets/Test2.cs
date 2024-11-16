using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KZLib;
using KZLib.KZResolver;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    [Button("Check")]
    void Button()
    {
        var pathGroup = CommonUtility.GetAssetPathGroup("t:prefab");

        foreach(var path in pathGroup)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            var rendererGroup = asset.GetComponentsInChildren<CanvasRenderer>();

            foreach(var renderer in rendererGroup)
            {
                renderer.cullTransparentMesh = true;
            }

            AssetDatabase.SaveAssets();
        }
    }
}
