// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class BuildEditorKey
// {
//     public class KeyInfo
//     {
//         public string BundleNameAPK;
//         public string BundleNameIPA;
//         public string KeyStoreName;
//         public string KeyStorePass;
//         public string KeyAliasName;
//         public string KeyAliasPass;
//     }

//     public static KeyInfo GetKeyInfo(PlatformManager.PlatformType platformType)
//     {
//         KeyInfo ki = null;
//         switch (platformType)
//         {
//             case PlatformManager.PlatformType.wod:
//                 ki = new KeyInfo()
//                 {
//                     BundleNameAPK = "com.nxth.wodn",
//                     BundleNameIPA = "com.nxth.wod",
//                     KeyStoreName = "wod.keystore",
//                     KeyStorePass = "WODN!@321",
//                     KeyAliasName = "wod",
//                     KeyAliasPass = "WODN!@321"
//                     //BundleNameAPK = "com.actoz.wod",
//                     //BundleNameIPA = "com.actoz.wod",
//                     //KeyStoreName = "epic.keystore",
//                     //KeyStorePass = "dkdlepsxlxl!@#",
//                     //KeyAliasName = "epic",
//                     //KeyAliasPass = "dkdlepsxlxl!@#"
//                 };
//                 break;
//             case PlatformManager.PlatformType.gamania:
//                 ki = new KeyInfo()
//                 {
//                     BundleNameAPK = "com.gamania.twwod",
//                     BundleNameIPA = "com.gamania.twwod.newworld",
//                     KeyStoreName = "twwod.keystore",
//                     KeyStorePass = "twwodpass",
//                     KeyAliasName = "twwod",
//                     KeyAliasPass = "twwodpass"
//                 };
//                 break;
//             case PlatformManager.PlatformType.tencent:
//                 ki = new KeyInfo()
//                 {
//                     BundleNameAPK = "com.actoz.wod",
//                     BundleNameIPA = "com.actoz.wod",
//                     KeyStoreName = "epic.keystore",
//                     KeyStorePass = "dkdlepsxlxl!@#",
//                     KeyAliasName = "epic",
//                     KeyAliasPass = "dkdlepsxlxl!@#"
//                 };
//                 break;
//             case PlatformManager.PlatformType.korea:
//                 ki = new KeyInfo()
//                 {
//                     BundleNameAPK = "com.ENP.wodk.kr.googleplay",
//                     BundleNameIPA = "com.eye.wod",
//                     KeyStoreName = "EnpWork.keystore",
//                     KeyStorePass = "123wod123",
//                     KeyAliasName = "enpwork",
//                     KeyAliasPass = "123wod123"
//                 };
//                 break;
//             case PlatformManager.PlatformType.nexon:
//                 ki = new KeyInfo()
//                 {
//                     BundleNameAPK = "com.nxth.wodn",
//                     BundleNameIPA = "com.nxth.wod",
//                     KeyStoreName = "wod.keystore",
//                     KeyStorePass = "WODN!@321",
//                     KeyAliasName = "wod",
//                     KeyAliasPass = "WODN!@321"
//                 };
//                 break;
//             case PlatformManager.PlatformType.vietnam:
//                 ki = new KeyInfo()
//                 {
//                     BundleNameAPK = "com.funtap.wodn",
//                     BundleNameIPA = "com.funtap.wodn",
//                     KeyStoreName = "wod.keystore",
//                     KeyStorePass = "WODN!@321",
//                     KeyAliasName = "wod",
//                     KeyAliasPass = "WODN!@321"
//                     //KeyStoreName = "com.funtap.wodn.jks",
//                     //KeyStorePass = "5FC34ACB8D74FB70EA93615E1BA9A6A0",
//                     //KeyAliasName = "cp03vn",
//                     //KeyAliasPass = "5FC34ACB8D74FB70EA93615E1BA9A6A0"
//                 };
//                 break;
//             case PlatformManager.PlatformType.korea_onestore:
//                 ki = new KeyInfo()
//                 {
//                     BundleNameAPK = "com.ENP.wodk.kr.googleplay",
//                     BundleNameIPA = "com.eye.wod",
//                     KeyStoreName = "EnpWork.keystore",
//                     KeyStorePass = "123wod123",
//                     KeyAliasName = "enpwork",
//                     KeyAliasPass = "123wod123"
//                 };
//                 break;
//             case PlatformManager.PlatformType.gamania_shengqu:
//                 ki = new KeyInfo()
//                 {
//                     BundleNameAPK = "com.gamania.twwod",
//                     BundleNameIPA = "com.gamania.twwod.newworld",
//                     KeyStoreName = "twwod.keystore",
//                     KeyStorePass = "twwodpass",
//                     KeyAliasName = "twwod",
//                     KeyAliasPass = "twwodpass"
//                 };
//                 break;
//             case PlatformManager.PlatformType.SqIntDevEnv:
//             case PlatformManager.PlatformType.SqIntDevEnvIl2Cpp:
//             case PlatformManager.PlatformType.SqExtDevEnv:
//             case PlatformManager.PlatformType.SqQaEnv:
//             case PlatformManager.PlatformType.SqFormalEnv:
//             case PlatformManager.PlatformType.SqIosArraignmentEnv:
//                 ki = new KeyInfo()
//                 {
//                     BundleNameAPK = "com.shengqugames.wod",
//                     BundleNameIPA = "com.shengqugames.wod",
//                     KeyStoreName = "ghome_wod.keystore",
//                     KeyStorePass = "111111",
//                     KeyAliasName = "ghome_wod",
//                     KeyAliasPass = "111111"
//                 };
//                 break;
//             default:
//                 UnityEngine.Debug.LogError("GetKeyInfo, Unprocessed platformType: " + platformType);
//                 break;
//         }
//         return ki;
//     }
// }
