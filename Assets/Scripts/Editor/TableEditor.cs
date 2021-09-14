using Sirenix.OdinInspector.Editor;
using System.IO;
using System.Linq;
using Table;
using UnityEditor;
using UnityEngine;

public partial class TableEditor : OdinMenuEditorWindow
{
    partial void AddOdinMenuTree(ref OdinMenuTree _tree)
    {
        _tree.Add("엑셀 테이블",GetTable<ExcelExampleTable>("ExcelExampleTable"));

        //if (Directory.Exists(SheetSetting.In.ScriptPath))
        //{
        //    foreach(var path in Directory.GetFiles(SheetSetting.In.ScriptPath,"*.cs"))
        //    {
        //        var name = Path.GetFileNameWithoutExtension(path);
        //        var type = System.Type.GetType($"Table.{name}, Assembly-CSharp");

        //        if(type != null)
        //        {
        //            var guid = AssetDatabase.FindAssets($"t:{name} {name}").FirstOrDefault();
        //            var data = string.IsNullOrEmpty(guid) ? CreateTable(name,type) : AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid),type);

        //            if(data != null)
        //            {
        //                _tree.Add(name,data);
        //            }
        //        }
        //    }

        //    AssetDatabase.Refresh();
        //}
    }

    //Object CreateTable(string _name,System.Type _type)
    //{
    //    var path = Path.Combine(SheetSetting.In.AssetPath,string.Concat(_name,".asset"));
    //    int index = path.IndexOf("Assets");
    //    var instance = CreateInstance(_type);

    //    if(instance != null)
    //    {
    //        AssetDatabase.CreateAsset(instance,path.Substring(index));
    //    }

    //    return instance;
    //}
}