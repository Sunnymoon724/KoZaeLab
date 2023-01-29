using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildEditorUtil
{
    private static GUIStyle _boxStyle = null;

    public static GUIStyle BoxStyle
    {
        get
        {
            if (_boxStyle == null)
            {
                _boxStyle = EditorStyles.helpBox;

                _boxStyle.padding.left = 4;
                _boxStyle.padding.right = 4;
                _boxStyle.padding.top = 4;
                _boxStyle.padding.bottom = 4;

                _boxStyle.margin.top = 4;
                _boxStyle.margin.bottom = 4;
                _boxStyle.margin.left = 4;
                _boxStyle.margin.right = 4;

                //_boxStyle.fixedWidth = 550f;
            }

            return _boxStyle;
        }
    }

    private static BuildTargetGroup GetActiveBuildGroup()
    {
        return BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
    }

    private static string GetDefines()
    {
        return PlayerSettings.GetScriptingDefineSymbolsForGroup(GetActiveBuildGroup());
    }

    public static bool CheckDefine(string defineName)
    {
        string strDefines = GetDefines();
        string[] defineSplit = strDefines.Split(';');
        bool isDefined = false;
        for (int i = 0, len = defineSplit.Length; i < len; i++)
        {
            var define = defineSplit[i];
            if (define.Equals(defineName))
            {
                isDefined = true;
                break;
            }
        }

        return isDefined;
    }

    public static void SetDefine(string defineName, bool isEnable)
    {
        string strDefines = GetDefines();
        List<string> defines = strDefines.Split(';').ToList();

        if(defines.Contains(defineName))
        {
            if (isEnable == false)
            {
                defines.Remove(defineName);
            }
        }
        else if(isEnable)
        {
            defines.Add(defineName);
        }

        StringBuilder strb = new StringBuilder();
        defines.ForEach(define =>
        {
            if (string.IsNullOrEmpty(define))
                return;

            strb.Append(define);
            strb.Append(";");
        });

        PlayerSettings.SetScriptingDefineSymbolsForGroup(GetActiveBuildGroup(), strb.ToString());
    }

    public static string ConvertSlashToUnicodeSlash(string text_)
    {
        return text_.Replace('/', '\u2215');
    }

    public static string ConvertUnicodeSlashToSlash(string text_)
    {
        return text_.Replace('\u2215', '/');
    }

}
