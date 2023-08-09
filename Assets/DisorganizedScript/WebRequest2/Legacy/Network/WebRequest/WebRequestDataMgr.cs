// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using UnityEngine;

// namespace KZLib.Custom
// {
//     public class WebRequestDataMgr : Singleton<WebRequestDataMgr>
//     {
//         private readonly Dictionary<string,WebRequestData> m_WebRequestDataDict = new Dictionary<string,WebRequestData>();

//         protected override void Initialize()
//         {
//             var textAsset = Resources.Load<TextAsset>("WebRequestData");

//             if(textAsset != null)
//             {
//                 AddWebRequestData(textAsset.text);
//             }

//             var path = Tools.PathCombine(Application.dataPath,"CustomSettings/WebRequestData.txt");

//             if(File.Exists(path))
//             {
//                 AddWebRequestData(File.ReadAllText(path));
//             }
//         }

//         ~WebRequestDataMgr()
//         {
//             m_WebRequestDataDict.Clear();
//         }

//         private void AddWebRequestData(string _text)
//         {
//             var data = JObject.Parse(Tools.AESDecryptData("CustomSettings",_text));

//             if(data != null)
//             {
//                 foreach(var property in data.Properties())
//                 {
//                     var type = Type.GetType(typeof(WebRequestData).FullName.Replace("WebRequestData",property.Name));

//                     if(type != null && m_WebRequestDataDict.ContainsKey(property.Name) == false)
//                     {
//                         m_WebRequestDataDict.Add(property.Name,(WebRequestData) property.Value.ToObject(type));
//                     }
//                 }
//             }
//         }

//         public T GetWebRequestData<T>() where T : WebRequestData
//         {
//             var name = typeof(T).Name;

//             if(m_WebRequestDataDict.ContainsKey(name) == false)
//             {
//                 m_WebRequestDataDict.Add(name,Activator.CreateInstance<T>());
//             }

//             return (T) m_WebRequestDataDict[name];
//         }

//         public void SaveWebData(bool _inside)
//         {
//             var path = _inside ? Tools.PathCombine(Application.dataPath,"Resources") : Tools.PathCombine(Application.dataPath,"CustomSettings");

//             Write(m_WebRequestDataDict.Where(x=>x.Value.IsInSide.Equals(_inside)).ToDictionary(x=>x.Key,y=>y.Value),path);
//         }

//         private void Write(Dictionary<string,WebRequestData> _dataDict,string _path)
//         {
// #if UNITY_EDITOR

//             var data = JObject.FromObject(_dataDict);

//             if(Directory.Exists(_path) == false)
//             {
//                 Directory.CreateDirectory(_path);
//             }

//             File.WriteAllText(Tools.PathCombine(_path,"WebRequestData.txt"),Tools.AESEncryptData("CustomSettings",data.ToString()));

//             UnityEditor.AssetDatabase.Refresh();
// #endif
//         }

//         public void ClearData(string _name,bool _inside)
//         {
//             m_WebRequestDataDict.RemoveSafe(_name);

//             SaveWebData(_inside);
//         }

//         public void SwitchData(string _key,WebRequestData _data)
//         {
//             if(m_WebRequestDataDict.ContainsKey(_key))
//             {
//                 m_WebRequestDataDict[_key] = _data;
//             }
//             else
//             {
//                 m_WebRequestDataDict.Add(_key,_data);
//             }
//         }
//     }
// }
