// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;
// using Newtonsoft.Json.Serialization;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Text;
// using UnityEngine;
// using UnityEngine.Networking;

// namespace KZLib.Custom
// {
//     #region Origin
//     public static class GoogleRequest
//     {
//         // 스프레드 시트랑 구글 드라이브 전부 받아오기
//         public const string ACCESS_SCOPE = "https://www.googleapis.com/auth/drive https://www.googleapis.com/auth/drive.appdata https://spreadsheets.google.com/feeds https://docs.google.com/feeds";
//     }

//     public abstract class GoogleRequest<T> : CustomWebRequest<T>
//     {
//         protected override void GetAuth(Action<bool> _onComplete)
//         {
//             var requestData = WebRequestDataMgr.In.GetWebRequestData<GoogleWebRequestData>();

//             AuthController.InitAuthController(new GoogleAccessTokenProvider(),_onComplete,requestData.ACCESS_TOKEN,requestData.REFRESH_TOKEN);
//         }

//         protected string GenerateQueryString()
//         {
//             var properties = GetType().GetProperties().Where(p => p.IsDefined(typeof(QueryParameterAttribute),false) && p.CanRead && p.GetValue(this,null) != null).ToDictionary(p => ToFirstLower(p.Name),property => property.GetValue(this,null));

//             var propertyNames = properties.Where(p => !(p.Value is string) && p.Value is IEnumerable).Select(p => p.Key).ToList();

//             foreach(var propertyName in propertyNames)
//             {
//                 var valueType = properties[propertyName].GetType();
//                 var valueElemType = valueType.IsGenericType ? valueType.GetGenericArguments()[0] : valueType.GetElementType();

//                 if(valueElemType.IsPrimitive || valueElemType == typeof(string))
//                 {
//                     var enumerable = properties[propertyName] as IEnumerable;
//                     properties[propertyName] = string.Join(",",enumerable.Cast<string>().ToArray());
//                 }
//             }

//             return string.Join("&",properties.Select(x => string.Concat(Uri.EscapeDataString(x.Key),"=",Uri.EscapeDataString(x.Value.ToString()))).ToArray());
//         }

//         private static string ToFirstLower(string _content)
//         {
//             var firstChar = char.ToLowerInvariant(_content[0]);

//             return _content.Length > 1 ? string.Concat(firstChar,_content.Substring(1)) : firstChar.ToString();
//         }        
//     }

//     #endregion Origin

//     #region Google Drive Upload

//     public class GoogleDriveUploadRequest : GoogleRequest<ResourceFile>
//     {
//         private readonly string name;
//         private readonly byte[] content;

//         public byte[] RequestPayload { get; protected set; }
//         public bool HasPayload => RequestPayload != null;

//         [QueryParameter] public virtual string UploadType => HasPayload ? "multipart" : null;

//         public static GoogleDriveUploadRequest Create(string _name,byte[] _content)
//         {
//             return new GoogleDriveUploadRequest(_name,_content);
//         }

//         public GoogleDriveUploadRequest(string _name,byte[] _content)
//         {
//             name = _name;
//             content = _content;
//         }

//         protected override UnityWebRequest GetWebRequest()
//         {
//             var requestData = WebRequestDataMgr.In.GetWebRequestData<GoogleWebRequestData>();
//             var boundary = Encoding.ASCII.GetString(UnityWebRequest.GenerateBoundary());
//             var boundaryDelimeter = string.Concat("\r\n\r\n","--",boundary);
//             var dataList = new List<byte>();
//             var data = Tools.ToJsonPrivateCamel(new ResourceFile(name,new List<string> { requestData.FOLDER_ID }));


//             dataList.AddRange(Encoding.UTF8.GetBytes(string.Concat(boundaryDelimeter,"\r\nContent-Type: application/json; charset=UTF-8\r\n\r\n",data,boundaryDelimeter,"\r\nContent-Type: application/octet-stream\r\n\r\n")));
//             dataList.AddRange(content);
//             dataList.AddRange(Encoding.UTF8.GetBytes(string.Concat("\r\n--",boundary,"--")));

//             // https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart&fields=id%2Cname%2Csize%2CcreatedTime

//             webRequest = new UnityWebRequest(string.Concat(@"https://www.googleapis.com/upload/drive/v3/files?",GenerateQueryString()),UnityWebRequest.kHttpVerbPOST);

//             webRequest.SetRequestHeader("Authorization",string.Concat("Bearer ",requestData.ACCESS_TOKEN));
//             webRequest.SetRequestHeader("Content-Type",string.Concat("multipart/related; boundary=",boundary));

//             webRequest.uploadHandler = new UploadHandlerRaw(dataList.ToArray());
//             webRequest.downloadHandler = new DownloadHandlerBuffer();

//             return webRequest;
//         }
//     }

//     #endregion Upload

//     #region Spread Sheet Get

//     public class SpreadSheetGetRequest : GoogleRequest<GoogleSpreadSheet>
//     {
//         public byte[] RequestPayload { get; protected set; }
//         public string PayloadMimeType { get; protected set; }
//         public bool HasPayload => RequestPayload != null;

//         private readonly string spreadSheetID;
//         private readonly string sheetName;
//         private readonly string range;

//         public static SpreadSheetGetRequest Create(string _spreadSheetID,string _sheetName,string _range = "A1:Z1000")
//         {
//             return new SpreadSheetGetRequest(_spreadSheetID,_sheetName,_range);
//         }

//         public SpreadSheetGetRequest(string _spreadSheetID,string _sheetName,string _range)
//         {
//             spreadSheetID = _spreadSheetID;
//             sheetName = _sheetName;
//             range = _range;
//         }

//         protected override UnityWebRequest GetWebRequest()
//         {
//             var requestData = WebRequestDataMgr.In.GetWebRequestData<GoogleWebRequestData>();

//             webRequest = new UnityWebRequest(string.Concat("https://sheets.googleapis.com/v4/spreadsheets/",spreadSheetID,"/values/",sheetName,"!",range),UnityWebRequest.kHttpVerbGET);
//             webRequest.SetRequestHeader("Authorization",string.Concat("bearer ",requestData.ACCESS_TOKEN));
//             webRequest.SetRequestHeader("Content-Type","application/json; charset=UTF-8");
//             webRequest.downloadHandler = new DownloadHandlerBuffer();

//             return webRequest;
//         }
//     }

//     #endregion Upload
// }