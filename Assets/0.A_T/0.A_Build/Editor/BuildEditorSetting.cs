using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RDBuildEditor
{
    public class BuildEditorSetting : ScriptableObject
    {
        [HideInInspector]
        public static string PATH_SETTING = "Assets/Epic/Editor/BuildEditorSetting.asset";
        [HideInInspector]
        public static string DIR_USER_SETTING = "Assets/Epic/Editor/";

        public List<string> ServerTypes = new List<string>();
        public List<string> GameModes = new List<string>();
        public List<string> PatchGroups = new List<string>();
        public List<string> BundleUrls = new List<string>();
        [HideInInspector]
        public List<string> UserSettingNames = new List<string>();
        [HideInInspector]
        public int UserSettingIdx = 0;
        [HideInInspector]
        public UserSetting Current = null;
        [HideInInspector]
        public List<UserSetting> UserSettings = new List<UserSetting>();

        public static BuildEditorSetting Load()
        {
            string path = $"{PATH_SETTING}";

            if (File.Exists(path))
            {
                var buildSetting = (BuildEditorSetting)AssetDatabase.LoadAssetAtPath(path, typeof(BuildEditorSetting));
                return buildSetting;
            }

            BuildEditorSetting asset = CreateInstance<BuildEditorSetting>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            if(asset.UserSettings.Count <= 0)
            {
                asset.AddUserSetting();
            }

            return asset;
        }

        public bool AddUserSetting()
        {
            if (UserSettingNames.Contains(Current.CustomName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(Current.CustomName))
            {
                Current.CustomName = "Default";
            }

            UserSettings.Add(Current.Clone());
            UserSettingNames.Add(Current.CustomName);

            SelectUserSetting(UserSettingNames.IndexOf(Current.CustomName));

            return true;
        }

        public bool RemoveUserSetting()
        {
            //UserSettingNames.Clear();
            //UserSettings.Clear();
            if (UserSettingNames.Count <= 0)
            {
                return false;
            }

            string removeName = Current.CustomName;
            if (string.IsNullOrEmpty(removeName))
            {
                return false;
            }

            int userSettingIdx = UserSettingNames.FindIndex(name => name.Equals(removeName));
            if (userSettingIdx < 0)
            {
                return false;
            }

            UserSettings.RemoveAt(userSettingIdx);
            UserSettingNames.RemoveAt(userSettingIdx);

            UserSettingIdx = UserSettingIdx > 0 ? UserSettingIdx - 1 : UserSettingIdx;
            SelectUserSetting(UserSettingIdx);

            return true;
        }


        public bool EditUserSetting()
        {
            // Debug.Log("[test] UserSettings : ", UserSettings.Count);
            if(UserSettingNames.Count <= 0)
            {
                AddUserSetting();
                return false;
            }

            if(string.IsNullOrEmpty(Current.CustomName))
            {
                return false;
            }

            // 현재 이름이 변경 됐고 기존의 이름과 겹치면 취소
            if (!Current.CustomName.Equals(UserSettingNames[UserSettingIdx]) &&
                UserSettingNames.Contains(Current.CustomName))
            {
                return false;
            }

            UserSettingNames[UserSettingIdx] = Current.CustomName;
            UserSettings[UserSettingIdx] = Current;
            return true;
        }

        public void SelectUserSetting(int index)
        {
            // Debug.Log("[test] SelectUserSetting : ", index, "||" , UserSettings.Count);
            UserSettingIdx = index;
            Current = UserSettings[UserSettingIdx];
        }

        public static BuildParam ConvertUserSetting(UserSetting userSetting)
        {
            return new BuildParam()
            {
                GameVersion = userSetting.GameVersion,
                AssetBundleVersion = userSetting.BundleVersion,
                VersionCode = int.Parse(userSetting.VersionCode),
                ServerURL = userSetting.BundleUrl,
                UseServerType = userSetting.ServerType,
                PatchGroup = userSetting.PatchGroup,
                LoginType = int.Parse(userSetting.LoginType),
                BuildTimeStamp = "",
                // PlatformType = userSetting.PlatformType,

                GameMode = userSetting.GameMode,
                UseDebug = userSetting.UseDebug,
                UseProfiler = userSetting.UseProfiler,
                UseMono = userSetting.UseMono,
            };
        }
    }
}