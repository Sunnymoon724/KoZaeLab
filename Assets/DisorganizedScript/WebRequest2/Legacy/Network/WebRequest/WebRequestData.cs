// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;
// using System;
// using System.Collections.Generic;
// using System.IO;
// using UnityEngine;

// namespace KZLib.Custom
// {
//     public abstract class WebRequestData
//     {
//         public abstract bool IsInSide { get; }
//         public string ACCESS_TOKEN { get; set; }
//         public string REFRESH_TOKEN { get; set; }
//     }

//     #region KakaoTalk

//     [Serializable]
//     public class KakaoTalkWebRequestData : WebRequestData
//     {
//         public override bool IsInSide => false;

//         public string API_KEY { get; set; }
//         public string USER_CODE { get; set; }

//         public string TEMPLATE_MESSAGE { get; set; }
//     }

//     #endregion KakaoTalk

//     #region Google

//     [Serializable]
//     public class GoogleWebRequestData : WebRequestData
//     {
//         public override bool IsInSide => false;

//         public string CLIENT_ID { get; set; }
//         public string PROJECT_ID { get; set; }
//         public string AUTH_URI { get; set; }
//         public string TOKEN_URI { get; set; }
//         public string AUTH_PROVIDER_X509_CERT_URL { get; set; }
//         public string CLIENT_SECRET { get; set; }
//         public string FOLDER_ID { get; set; }
//         public List<string> REDIECT_URIS { get; set; }

//         public string ACCESS_CODE { get; set; }


//         public GoogleWebRequestData()
//         {

//         }

//         [JsonConstructor]
//         public GoogleWebRequestData(string client_id,string project_id,string auth_uri,string token_uri,string auth_provider_x509_cert_url,string client_secret,List<string> redirect_uris)
//         {
//             CLIENT_ID = client_id;
//             PROJECT_ID = project_id;
//             AUTH_URI = auth_uri;
//             TOKEN_URI = token_uri;
//             AUTH_PROVIDER_X509_CERT_URL = auth_provider_x509_cert_url;
//             CLIENT_SECRET = client_secret;
//             REDIECT_URIS = redirect_uris;
//         }
//     }

//     #endregion Google

//     #region Slack

//     [Serializable]
//     public class SlackWebRequestData : WebRequestData
//     {
//         public override bool IsInSide => true;

//         public string BOT_NAME { get; set; }
//     }

//     #endregion Slack
// }