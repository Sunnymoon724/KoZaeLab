// using Newtonsoft.Json.Linq;
// using System;
// using System.Net;
// using System.Net.Sockets;
// using System.Security.Cryptography;
// using System.Text;
// using System.Threading;
// using UnityEngine;
// using UnityEngine.Networking;

// namespace KZLib.Custom
// {
//     #region AccessTokenProvider

//     public abstract class AccessTokenProvider
//     {
//         private Action<AccessTokenProvider> onComplete;

//         public bool IsDone { get; protected set; }
//         public bool IsError { get; protected set; }

//         protected abstract void ExecuteFullAuth();

//         protected abstract void GetRefreshAccessToken();

//         protected abstract void OnHttpCallBack(object _state);

//         protected abstract void SetTokenGroup(string _access,string _refresh);

//         protected abstract void GetAccessToken(string _code,string codeVerifier);

//         protected UnityWebRequest webRequest;

//         private SynchronizationContext unitySyncContext;

//         public AccessTokenProvider()
//         {
//             unitySyncContext = SynchronizationContext.Current;
//         }

//         public void InitProvider(Action<AccessTokenProvider> _onComplete,string _refreshToken)
//         {
//             onComplete = _onComplete;

//             if(_refreshToken.IsEmpty() == true)
//             {
//                 ExecuteFullAuth();
//             }
//             else
//             {
//                 GetRefreshAccessToken();
//             }
//         }

//         protected void SetHttpListener(HttpListener _listener)
//         {
//             var data = _listener.BeginGetContext((result) =>
//             {
//                 unitySyncContext.Send(OnHttpCallBack,result);
//             },_listener);

//             if(Application.runInBackground == false)
//             {
//                 data.AsyncWaitHandle.WaitOne();
//             }
//         }

//         protected void OnAccessComplete(AsyncOperation _requestYield)
//         {
//             if(CheckRequestErrors(webRequest,out var data))
//             {
//                 SetTokenGroup(data["access_token"].ToObject<string>(),data["refresh_token"].ToObject<string>());

//                 ProvideComplete();
//             }
//             else
//             {
//                 Debug.LogError("Failed to exchange authorization code.");

//                 ProvideComplete(true);
//             }
//         }

//         protected void OnRefreshComplete(AsyncOperation _requestYield)
//         {
//             if(CheckRequestErrors(webRequest,out var data))
//             {
//                 SetTokenGroup(data["access_token"].ToObject<string>(),null);

//                 ProvideComplete();
//             }
//             else
//             {
//                 ExecuteFullAuth();
//             }
//         }

//         private bool CheckRequestErrors(UnityWebRequest _request,out JObject _object)
//         {
//             _object = null;

//             if(_request == null)
//             {
//                 Debug.LogError("Refresh token request failed. Request object is null.");
//                 return false;
//             }

//             var errorBuilder = new StringBuilder();

//             if(_request.error.IsEmpty() == false)
//             {
//                 errorBuilder.Append("HTTP Error: ").AppendLine(_request.error);
//             }

//             if(_request.downloadHandler != null && !string.IsNullOrEmpty(_request.downloadHandler.text))
//             {
//                 _object = JObject.Parse(_request.downloadHandler.text);
//                 var error = _object["error"];

//                 if(error != null)
//                 {
//                     errorBuilder.Append("HTTP Error : ").AppendLine(_object["error"].ToObject<string>());
//                     errorBuilder.Append("HTTP Error Description : ").AppendLine(_object["error_description"].ToObject<string>());
//                 }
//             }

//             var isError = errorBuilder.Length > 0;

//             if(isError)
//             {
//                 Debug.LogError(string.Concat(" Refresh token code request failed.\n",errorBuilder));
//             }

//             return isError == false;
//         }

//         protected void ProvideComplete(bool error = false)
//         {
//             IsError = error;
//             IsDone = true;

//             onComplete?.Invoke(this);
//         }
//     }

//     #endregion AccessTokenProvider

//     #region KakaoTalk

//     public class KakaoTalkAccessTokenProvider : AccessTokenProvider
//     {
//         protected override void ExecuteFullAuth()
//         {
//             var httpListener = new HttpListener();

//             httpListener.Prefixes.Add(string.Concat(CustomWebRequest.REDIRECT_URL,'/'));
//             httpListener.Start();

//             var requestData = WebRequestDataMgr.In.GetWebRequestData<KakaoTalkWebRequestData>();

//             Application.OpenURL(string.Concat("https://kauth.kakao.com/oauth/authorize?response_type=code&client_id=",requestData.API_KEY,"&redirect_uri=",CustomWebRequest.REDIRECT_URL,"&response_type=code"));

//             SetHttpListener(httpListener);
//         }

//         protected override void OnHttpCallBack(object _state)
//         {
//             var result = (IAsyncResult) _state;
//             var httpListener = (HttpListener) result.AsyncState;
//             var context = httpListener.EndGetContext(result);

//             var response = context.Response;
//             var buffer = Encoding.UTF8.GetBytes(CustomWebRequest.LOOPBACK_RESPONSE_HTML);
//             response.ContentLength64 = buffer.Length;
//             var responseOutput = response.OutputStream;
//             responseOutput.Write(buffer,0,buffer.Length);
//             responseOutput.Close();
//             httpListener.Close();

//             if(context.Request.QueryString.Get("error") != null)
//             {
//                 Debug.LogError($"OAuth authorization error: {context.Request.QueryString.Get("error")}.");

//                 ProvideComplete(true);

//                 return;
//             }
//             if(context.Request.QueryString.Get("code") == null)
//             {
//                 Debug.LogError($"Malformed authorization response. {context.Request.QueryString}");

//                 ProvideComplete(true);

//                 return;
//             }

//             var code = context.Request.QueryString.Get("code");

//             // TODO 번들!!
//             // BuildSetting.In.SetKakaoTalkUserCode(code);

//             GetAccessToken(code,"");
//         }

//         protected override void GetAccessToken(string _code,string codeVerifier)
//         {
//             var requestData = WebRequestDataMgr.In.GetWebRequestData<KakaoTalkWebRequestData>();

//             var form = new WWWForm();
//             form.AddField("grant_type","authorization_code");
//             form.AddField("client_id",requestData.API_KEY);
//             form.AddField("redirect_uri",CustomWebRequest.REDIRECT_URL);
//             form.AddField("code",_code);

//             webRequest = UnityWebRequest.Post("https://kauth.kakao.com/oauth/token",form);

//             webRequest.SendWebRequest().completed += OnAccessComplete;
//         }

//         protected override void SetTokenGroup(string _access,string _refresh)
//         {
//             // TODO 번들!!
//             // if(string.IsNullOrEmpty(_refresh))
//             // {
//             //     BuildSetting.In.SetKakaoTalkAccessToken(_access);
//             // }
//             // else
//             // {
//             //     BuildSetting.In.SetKakaoTalkTokenGroup(_access,_refresh);
//             // }
//         }

//         protected override void GetRefreshAccessToken()
//         {
//             var requestData = WebRequestDataMgr.In.GetWebRequestData<KakaoTalkWebRequestData>();

//             var form = new WWWForm();
//             form.AddField("client_id",requestData.API_KEY);
//             form.AddField("grant_type","refresh_token");
//             form.AddField("refresh_token",requestData.REFRESH_TOKEN);

//             webRequest = UnityWebRequest.Post("https://kauth.kakao.com/oauth/token",form);
//             webRequest.SetRequestHeader("Content-type","application/x-www-form-urlencoded;charset=utf-8");

//             webRequest.SendWebRequest().completed += OnRefreshComplete;
//         }
//     }

//     #endregion KakaoTalk

//     #region Google

//     public class GoogleAccessTokenProvider : AccessTokenProvider
//     {
//         private string expectedState;
//         private string codeVerifier;
//         private string redirectUri;

//         protected override void ExecuteFullAuth()
//         {
//             expectedState = RandomDataBase64Uri(32);
//             codeVerifier = RandomDataBase64Uri(32);
//             var codeVerifierHash = Sha256(codeVerifier);
//             var codeChallenge = Base64UriEncodeNoPadding(codeVerifierHash);

//             redirectUri = string.Concat("http://localhost:",GetRandomUnusedPort());

//             //redirectUri = CustomWebRequest.REDIRECT_URL;

//             // Listen for requests on the redirect URI.
//             var httpListener = new HttpListener();

//             httpListener.Prefixes.Add(string.Concat(redirectUri,'/'));
//             httpListener.Start();

//             var requestData = WebRequestDataMgr.In.GetWebRequestData<GoogleWebRequestData>();

//             var authURL = new StringBuilder();
//             authURL.Append(requestData.AUTH_URI);
//             authURL.Append("?response_type=code&scope=");
//             authURL.Append(Uri.EscapeDataString(GoogleRequest.ACCESS_SCOPE));
//             authURL.Append("&redirect_uri=");
//             authURL.Append(Uri.EscapeDataString(redirectUri));
//             authURL.Append("&client_id=");
//             authURL.Append(requestData.CLIENT_ID);
//             authURL.Append("&state=");
//             authURL.Append(expectedState);
//             authURL.Append("&code_challenge=");
//             authURL.Append(codeChallenge);
//             authURL.Append("&code_challenge_method=S256&access_type=offline&approval_prompt=force");

//             // Create the OAuth 2.0 authorization request.
//             // https://developers.google.com/identity/protocols/OAuth2WebServer#creatingclient
//             //var authRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}&state={4}&code_challenge={5}&code_challenge_method={6}"+"&access_type=offline"+"&approval_prompt=force",BuildSetting.In.AuthUri,Uri.EscapeDataString(BuildSetting.AccessScope), Uri.EscapeDataString(redirectUri), BuildSetting.In.ClientId, expectedState, codeChallenge, "S256");

//             // Open request in the browser.
//             Application.OpenURL(authURL.ToString());

//             SetHttpListener(httpListener);
//         }

//         protected override void OnHttpCallBack(object _state)
//         {
//             var requestData = WebRequestDataMgr.In.GetWebRequestData<GoogleWebRequestData>();

//             var result = (IAsyncResult) _state;
//             var httpListener = (HttpListener) result.AsyncState;
//             var context = httpListener.EndGetContext(result);

//             var response = context.Response;
//             var buffer = Encoding.UTF8.GetBytes(CustomWebRequest.LOOPBACK_RESPONSE_HTML);
//             response.ContentLength64 = buffer.Length;
//             var responseOutput = response.OutputStream;
//             responseOutput.Write(buffer,0,buffer.Length);
//             responseOutput.Close();
//             httpListener.Close();

//             if(context.Request.QueryString.Get("error") != null)
//             {
//                 Debug.LogError($"OAuth authorization error: {context.Request.QueryString.Get("error")}.");

//                 ProvideComplete(true);

//                 return;
//             }
//             if(context.Request.QueryString.Get("code") == null || context.Request.QueryString.Get("state") == null)
//             {
//                 Debug.LogError($"Malformed authorization response. {context.Request.QueryString}");

//                 ProvideComplete(true);

//                 return;
//             }

//             requestData.ACCESS_CODE = context.Request.QueryString.Get("code");
//             var incomingState = context.Request.QueryString.Get("state");

//             if(incomingState != expectedState)
//             {
//                 Debug.LogError($"UnityGoogleDrive: Received request with invalid state ({incomingState}).");

//                 ProvideComplete(true);

//                 return;
//             }

//             GetAccessToken(requestData.ACCESS_CODE,codeVerifier);
//         }

//         protected override void GetAccessToken(string _code,string codeVerifier)
//         {
//             var requestData = WebRequestDataMgr.In.GetWebRequestData<GoogleWebRequestData>();

//             var form = new WWWForm();
//             form.AddField("code",requestData.ACCESS_CODE);
//             form.AddField("redirect_uri",redirectUri);
//             form.AddField("client_id",requestData.CLIENT_ID);
//             form.AddField("code_verifier",codeVerifier);

//             if(requestData.CLIENT_SECRET.IsEmpty() == false)
//             {
//                 form.AddField("client_secret",requestData.CLIENT_SECRET);
//             }

//             form.AddField("scope",GoogleRequest.ACCESS_SCOPE);
//             form.AddField("grant_type","authorization_code");

//             webRequest = UnityWebRequest.Post(requestData.TOKEN_URI,form);
//             webRequest.SetRequestHeader("Content-Type","application/x-www-form-urlencoded");
//             webRequest.SetRequestHeader("Accept","Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
//             webRequest.SendWebRequest().completed += OnAccessComplete;
//         }

//         protected override void SetTokenGroup(string _access,string _refresh)
//         {
//             // if(string.IsNullOrEmpty(_refresh))
//             // {
//             //     BuildSetting.In.SetGoogleDriveAccessToken(_access);
//             // }
//             // else
//             // {
//             //     BuildSetting.In.SetGoogleDriveTokenGroup(_access,_refresh);
//             // }
//         }

//         protected override void GetRefreshAccessToken()
//         {
//             var requestData = WebRequestDataMgr.In.GetWebRequestData<GoogleWebRequestData>();

//             var form = new WWWForm();
//             form.AddField("client_id",requestData.CLIENT_ID);

//             if(requestData.CLIENT_SECRET.IsEmpty() == false)
//             {
//                 form.AddField("client_secret",requestData.CLIENT_SECRET);
//             }

//             form.AddField("refresh_token",requestData.REFRESH_TOKEN);
//             form.AddField("grant_type","refresh_token");

//             webRequest = UnityWebRequest.Post(requestData.TOKEN_URI,form);
//             webRequest.SetRequestHeader("Content-Type","application/x-www-form-urlencoded");
//             webRequest.SetRequestHeader("Accept","Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
//             webRequest.SendWebRequest().completed += OnRefreshComplete;
//         }

//         #region Etc.

//         private int GetRandomUnusedPort()
//         {
//             // Based on: http://stackoverflow.com/a/3978040.
//             var listener = new TcpListener(IPAddress.Loopback,0);
//             listener.Start();

//             var port = ((IPEndPoint) listener.LocalEndpoint).Port;
//             listener.Stop();

//             return port;
//         }

//         private string Base64UriEncodeNoPadding(byte[] buffer)
//         {
//             var base64 = Convert.ToBase64String(buffer);
//             base64 = base64.Replace("+","-");
//             base64 = base64.Replace("/","_");
//             base64 = base64.Replace("=","");

//             return base64;
//         }

//         private byte[] Sha256(string inputString)
//         {
//             var bytes = Encoding.ASCII.GetBytes(inputString);
//             var sha256 = new SHA256Managed();
//             return sha256.ComputeHash(bytes);
//         }

//         private string RandomDataBase64Uri(uint length)
//         {
//             var cryptoProvider = new RNGCryptoServiceProvider();
//             var bytes = new byte[length];
//             cryptoProvider.GetBytes(bytes);

//             return Base64UriEncodeNoPadding(bytes);
//         }

//         #endregion Etc.
//     }

//     #endregion Google
// }