using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RDBuildEditor
{
    public class BuildEditorUserSetting
    {
        public List<UserSetting> UserSettings;
        public UserSetting this[string name]
        {
            get
            {
                return UserSettings?.Find(item => item.CustomName.Equals(name));
            }
        }

        //public UserSetting this[int index]
        //{
        //    get
        //    {
        //        if (index >= 0 && index < UserSettings.Count)
        //        {
        //            return UserSettings[index];
        //        }

        //        return null;
        //    }
        //}


        public BuildEditorUserSetting()
        {
            UserSettings = new List<UserSetting>();
        }


        public static BuildEditorUserSetting Load(string path)
        {
            var buildEditorUserSetting = new BuildEditorUserSetting();

            return buildEditorUserSetting;
        }


        public static List<string> GetNames(BuildEditorUserSetting userSetting)
        {
            if(userSetting.UserSettings.Count <= 0)
            {
                return new List<string>();
            }

            return userSetting.UserSettings.ConvertAll(user => user.CustomName);
        }
    }

    [System.Serializable]
    public class UserSetting
    {
        public string CustomName;
        //public string CustomPath;

        public string ServerType;
        public string GameMode;
        public string PatchGroup;
        public string BundleUrl;

        public string GameVersion;
        public string VersionCode;
        public string BundleVersion;
        // public PlatformManager.PlatformType PlatformType;

        public string LoginType;

        public bool UseProfiler;
        public bool UseDebug;
        public bool UseMono;
        public bool UseAAB;

        public UserSetting Clone()
        {

            return new UserSetting()
            {
                CustomName = CustomName,
                ServerType = ServerType,
                GameMode = GameMode,
                PatchGroup = PatchGroup,
                BundleUrl = BundleUrl,
                GameVersion = GameVersion,
                VersionCode = VersionCode,
                BundleVersion = BundleVersion,
                // PlatformType = PlatformType,
                LoginType = LoginType,
                UseProfiler = UseProfiler,
                UseDebug = UseDebug,
                UseMono = UseMono,
                UseAAB = UseAAB
            };
        }
    }
}
