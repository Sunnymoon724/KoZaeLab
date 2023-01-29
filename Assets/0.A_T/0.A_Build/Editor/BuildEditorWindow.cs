// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using System;
 

// using System.IO;

// namespace RDBuildEditor
// {
//     public class BuildEditorWindow : EditorWindow
//     {
//         private static BuildEditorUserSetting m_UserSetting = null;

//         private List<string> m_PlatformTypeList = null;
//         //private string m_selectedPlatformType = "";

//         private List<string> m_ServerTypeList = null;
//         private string m_selectedServerType = "";

//         private List<string> m_GameModeList = null;

//         private BuildEditorServerSetting m_Setting = null;
//         private BuildEditorServerSetting.ServerSetting m_currentSetting = null;

//         public static void Open()
//         {
//             Debug.Log("[test] Open : ");
//             var window = (BuildEditorWindow)GetWindow(typeof(BuildEditorWindow));
//             window.Init();
//         }

//         private void Init()
//         {
//             titleContent = new GUIContent("Build Window");
//             autoRepaintOnSceneChange = true;
//             Refresh();
//         }

//         private void Refresh()
//         {
//             m_Setting = new BuildEditorServerSetting();
//             m_Setting.LoadJson();

//             m_ServerTypeList = m_Setting.ServerTypes;
//             m_PlatformTypeList = m_Setting.PlatformTypes;
//             m_GameModeList = m_Setting.GameModes;
//         }

//         private void OnFocus()
//         {
//             Refresh();
//         }


//         private void OnGUI()
//         {
//             if (m_Setting == null)
//             {
//                 return;
//             }

//             BuildButtonGUI();
//             ServerTypeChangeGUI();
//             //BuildEditorSettingUI();
//             OptionGUI();
//         }

//         private void BuildButtonGUI()
//         {
//             EditorGUILayout.BeginVertical(BuildEditorUtil.BoxStyle);
//             EditorGUILayout.BeginHorizontal();
//             GUI.backgroundColor = Color.cyan;
//             if (GUILayout.Button(GetBuildBtnText(), GUILayout.MinHeight(50f)))
//             {
//                 BuildParam buildParam = BuildEditorServerSetting.ConvertUserSetting(m_currentSetting);
// #if UNITY_ANDROID
//                 BuildEditor.AndroidBuild(".apk", buildParam);
// #elif UNITY_IOS
//                 BuildEditor.iOSBuild(Path.GetFullPath("Build"), buildParam);
// #elif UNITY_STANDALONE
//                 string path = BuildUtil.GetBuildDirectory();
//                 string strTargetFileName = path + "/" + "WOD_PC" + "/" + BuildEditor.APP_NAME + ".exe";
//                 BuildEditor.PCBuild(strTargetFileName, buildParam);
// #endif
//                 GUIUtility.ExitGUI();
//             }
//             GUI.backgroundColor = Color.white;
//             EditorGUILayout.EndHorizontal();

//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button(GetBtnTextGenVersionFile(), GUILayout.MinHeight(30f)))
//             {
//                 BuildEditor.GenericBuildInfo(BuildEditorServerSetting.ConvertUserSetting(m_currentSetting));
//             }

//             GUI.backgroundColor = Color.red;
//             if (GUILayout.Button(GetBtnTextDeleteVersionFile(), GUILayout.MinHeight(30f), GUILayout.Width(100f)))
//             {
//                 BuildEditor.DeleteBuildInfo();
//             }
//             GUI.backgroundColor = Color.white;
//             EditorGUILayout.EndHorizontal();

//             EditorGUILayout.EndVertical();
//         }


//         private void ServerTypeChangeGUI()
//         {
//             EditorGUILayout.BeginVertical(BuildEditorUtil.BoxStyle);
//             string prevServerType = m_selectedServerType;
//             DropDownMenuGUI(GetDropMenuTextServerType(), m_ServerTypeList, ref m_selectedServerType);
//             if (prevServerType != m_selectedServerType)
//             {
//                 m_currentSetting =
//                     m_Setting.m_serverSettingList.ServerSettings.Find(x => x.ServerType == m_selectedServerType);

//                 Debug.Log("[test] Change : ", m_selectedServerType);

//                 // PlatformChange(m_currentSetting.PlatformType);
// #if UNITY_IOS || UNITY_ANDROID
//                 ServerTypeChangeFile(m_selectedServerType);
// #endif
//             }
//             EditorGUILayout.EndVertical();
//         }

//         private bool m_IsEditUserSetting = false;


//         //private void BuildEditorSettingUI()
//         //{
//         //    EditorGUILayout.BeginVertical(BuildEditorUtil.BoxStyle);
//         //    EditorGUILayout.ObjectField(m_currentSetting, typeof(BuildEditorServerSetting.ServerSetting), false);
//         //    EditorGUILayout.EndVertical();
//         //}

//         private void OptionGUI()
//         {
//             if (m_Setting == null || m_currentSetting == null)
//             {
//                 return;
//             }

//             EditorGUILayout.BeginVertical(BuildEditorUtil.BoxStyle);

//             //DropDownMenuGUI("서버 타입", m_Setting.ServerTypes, ref current.ServerType);
//             DropDownMenuGUI(GetGetDropMenuTextABURL(), m_Setting.BundleUrls, ref m_currentSetting.BundleUrl);

//             LabelTextField(GetLabelTextGameVersion(), ref m_currentSetting.GameVersion);
//             LabelTextField(GetLabelTextVersionCode(), ref m_currentSetting.VersionCode);
//             LabelTextField(GetLabelTextResVersion(), ref m_currentSetting.BundleVersion);
//             LabelTextField(GetLabelTextLoginType(), ref m_currentSetting.LoginType);
//             DropDownMenuGUI(GetLabelTextGameMode(), m_GameModeList, ref m_currentSetting.GameMode);
//             DropDownMenuGUI(GetLabelTextPlatformType(), m_PlatformTypeList, ref m_currentSetting.PlatformType);
//             //ToggleButton("디버깅", ref current.UseDebug, Color.magenta);
//             ToggleButton(GetToggleButtonTextDebug(), ref m_currentSetting.UseDebug, Color.cyan);
//             ToggleButton(GetToggleButtonTextProfiler(), ref m_currentSetting.UseProfiler, Color.cyan);
//             ToggleButton(GetToggleButtonTextAABBuild(), ref m_currentSetting.UseAAB, Color.cyan);
//             ToggleButton(GetToggleButtonTextMonoBuild(), ref m_currentSetting.UseMono, Color.cyan);
//             EditorGUILayout.EndVertical();
//         }

//         private void ToggleButton(string label, ref bool isToggle, Color color)
//         {
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField(label, GUILayout.MinWidth(80f));
//             GUI.color = isToggle ? color : Color.white;
//             string str = isToggle ? GetToggleButtonTextEnable() : GetToggleButtonTextDisable();
//             isToggle = GUILayout.Toggle(isToggle, str, "button", GUILayout.Width(80f));
//             GUI.color = Color.white;
//             EditorGUILayout.EndHorizontal();
//         }

//         private void DropDownMenuGUI(string label, List<string> list, ref string selected, float width = 150f)
//         {
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField(label, GUILayout.MinWidth(80f));
//             int selectedIdx = list.IndexOf(selected);
//             List<string> listPopupText = list.ConvertAll(str => BuildEditorUtil.ConvertSlashToUnicodeSlash(str));
//             var idx = SelectPopupGUI(listPopupText, selectedIdx, width);
//             if (selectedIdx != idx)
//             {
//                 selected = list[idx];
//             }
//             EditorGUILayout.EndHorizontal();
//         }

//         private void LabelTextField(string str, ref string text, float width = 150f)
//         {
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.LabelField(str, GUILayout.MinWidth(80f));
//             text = EditorGUILayout.TextField(text, GUILayout.Width(width));
//             EditorGUILayout.EndHorizontal();
//         }

//         private int SelectPopupGUI(List<string> list, int index, float width = 0f)
//         {
//             var bundleTags = list;
//             int len = bundleTags.Count;
//             string[] nameTags = bundleTags.ToArray();

//             if (len <= 0)
//                 return 0;

//             if (index >= len)
//             {
//                 index = 0;
//             }
//             var ret = index;
//             if (width > 0)
//             {
//                 return EditorGUILayout.Popup(index, nameTags, GUILayout.Width(width));
//             }

//             return EditorGUILayout.Popup(index, nameTags);
//         }

//         #region Switch language

//         string GetBuildBtnText()
//         {
             
//             {
//                 return "构建";
//             }
//             return "빌드하기";
//         }

//         string GetBtnTextGenVersionFile()
//         {
             
//             {
//                 return "版本文件生成";
//             }
//             return "버전 파일 생성";
//         }

//         string GetBtnTextDeleteVersionFile()
//         {
             
//             {
//                 return "版本文件删除";
//             }
//             return "버전 파일 제거";
//         }

//         string GetDropMenuTextServerType()
//         {
             
//             {
//                 return "服务器类型选择";
//             }
//             return "서버 타입 변경";
//         }

//         string GetGetDropMenuTextABURL()
//         {
             
//             {
//                 return "资源服务器 URL";
//             }
//             return "번들 URL";
//         }

//         string GetLabelTextGameVersion()
//         {
             
//             {
//                 return "游戏版本";
//             }
//             return "게임 버전";
//         }

//         string GetLabelTextVersionCode()
//         {
             
//             {
//                 return "VersionCode";
//             }
//             return "버전 코드";
//         }

//         string GetLabelTextResVersion()
//         {
             
//             {
//                 return "资源版本";
//             }
//             return "번들 버전";
//         }

//         string GetLabelTextLoginType()
//         {
             
//             {
//                 return "登录类型";
//             }
//             return "로그인 타입";
//         }

//         string GetLabelTextGameMode()
//         {
             
//             {
//                 return "游戏模式";
//             }
//             return "게임 모드";
//         }

//         string GetLabelTextPlatformType()
//         {
             
//             {
//                 return "平台类型";
//             }
//             return "플랫폼 타입";
//         }

//         string GetToggleButtonTextDebug()
//         {
             
//             {
//                 return "日志";
//             }
//             return "디버그 로그";
//         }

//         string GetToggleButtonTextProfiler()
//         {
             
//             {
//                 return "调试";
//             }
//             return "프로파일링";
//         }

//         string GetToggleButtonTextAABBuild()
//         {
             
//             {
//                 return "AAB 构建";
//             }
//             return "AAB 빌드";
//         }

//         string GetToggleButtonTextMonoBuild()
//         {
             
//             {
//                 return "Mono 构建";
//             }
//             return "Mono 빌드";
//         }

//         string GetToggleButtonTextEnable()
//         {
             
//             {
//                 return "启用";
//             }
//             return "사 용";
//         }

//         string GetToggleButtonTextDisable()
//         {
             
//             {
//                 return "禁用";
//             }
//             return "사용 안함";
//         }

//         #endregion

//     }
// }