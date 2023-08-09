// #if UNITY_EDITOR
// using System;
// using System.Net;
// using System.Net.Sockets;
// using System.Security.Cryptography;
// using System.Text;
// using UnityEngine;
// using UnityEngine.Networking;

// namespace KZLib.Auth
// {
// 	public class GoogleAccessTokenProvider : AccessTokenProvider
// 	{
// 		private const string ACCESS_SCOPE = "https://www.googleapis.com/auth/drive https://www.googleapis.com/auth/drive.appdata https://spreadsheets.google.com/feeds";

// 		private string m_ExpectedState;
// 		private string m_CodeVerifier;
// 		private string m_RedirectUri;

// 		protected override bool IsExistAuthData => AuthSettings.In.IsExistGoogleAuthData;

// 		protected override string CachedAccessToken =>AuthSettings.In.GoogleAccessToken;
// 		protected override string CachedRefreshToken =>AuthSettings.In.GoogleRefreshToken;

// 		protected override void ExecuteFullAuth()
// 		{
// 			m_ExpectedState = RandomDataBase64Uri(32);
// 			m_CodeVerifier = RandomDataBase64Uri(32);

// 			var codeChallenge = Base64UriEncodeNoPadding(ToSha256(m_CodeVerifier));

// 			m_RedirectUri = string.Format("http://localhost:{0}",GetRandomUnusedPort());

// 			// Listen for requests on the redirect URI.
// 			var httpListener = new HttpListener();

// 			httpListener.Prefixes.Add(string.Concat(m_RedirectUri,'/'));
// 			httpListener.Start();

// 			var request = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}&state={4}&code_challenge={5}&code_challenge_method={6}&code_challenge_method=S256&access_type=offline&approval_prompt=force",AuthSettings.In.GoogleAuthUri,Uri.EscapeDataString(ACCESS_SCOPE),Uri.EscapeDataString(m_RedirectUri),AuthSettings.In.GoogleClientId,m_ExpectedState,codeChallenge);

// 			// Open request in the browser.
// 			Application.OpenURL(request);

// 			SetHttpListener(httpListener);
// 		}

// 		protected override void OnHttpCallBack(object _state)
// 		{
// 			var result = (IAsyncResult) _state;
// 			var httpListener = (HttpListener) result.AsyncState;
// 			var context = httpListener.EndGetContext(result);

// 			var response = context.Response;
// 			var buffer = Encoding.UTF8.GetBytes(LOOPBACK_RESPONSE_HTML);
// 			response.ContentLength64 = buffer.Length;
// 			var responseOutput = response.OutputStream;
// 			responseOutput.Write(buffer,0,buffer.Length);
// 			responseOutput.Close();
// 			httpListener.Close();

// 			var query = context.Request.QueryString;

// 			var errorText = query.Get("error");

// 			if(errorText != null)
// 			{
// 				Log.Library.E("OAuth authorization error: {0}.",errorText);

// 				ProvideAccessTokenComplete(true);

// 				return;
// 			}

// 			if(query.Get("code") == null || query.Get("state") == null)
// 			{
// 				Log.Library.E("Malformed authorization response. {0}.",query);

// 				ProvideAccessTokenComplete(true);

// 				return;
// 			}

// 			var code = query.Get("code");
// 			var incomingState = query.Get("state");

// 			if(incomingState != m_ExpectedState)
// 			{
// 				Log.Library.E("UnityGoogleDrive: Received request with invalid state ({0}).",incomingState);

// 				ProvideAccessTokenComplete(true);

// 				return;
// 			}

// 			m_AccessTokenExchanger.GetAuthCode(GetExchangeAuthCode(code));
// 		}

// 		protected override UnityWebRequest GetExchangeAuthCode(string _code)
// 		{
// 			var form = new WWWForm();
// 			form.AddField("code",_code);
// 			form.AddField("redirect_uri",m_RedirectUri);
// 			form.AddField("client_id",AuthSettings.In.GoogleClientId);
// 			form.AddField("code_verifier",m_CodeVerifier);

// 			if(AuthSettings.In.GoogleClientSecret.IsEmpty() == false)
// 			{
// 				form.AddField("client_secret",AuthSettings.In.GoogleClientSecret);
// 			}

// 			form.AddField("scope",ACCESS_SCOPE);
// 			form.AddField("grant_type","authorization_code");

// 			return SetRequestForm(form);
// 		}

// 		protected override UnityWebRequest GetRefreshAccessToken()
// 		{
// 			var form = new WWWForm();
// 			form.AddField("client_id",AuthSettings.In.GoogleClientId);

// 			if(AuthSettings.In.GoogleClientSecret.IsEmpty() == false)
// 			{
// 				form.AddField("client_secret",AuthSettings.In.GoogleClientSecret);
// 			}

// 			form.AddField("refresh_token",AuthSettings.In.GoogleRefreshToken);
// 			form.AddField("grant_type","refresh_token");

// 			return SetRequestForm(form);
// 		}

// 		private UnityWebRequest SetRequestForm(WWWForm _form)
// 		{
// 			var request = UnityWebRequest.Post(AuthSettings.In.GoogleTokenUri,_form);
// 			request.SetRequestHeader("Content-Type","application/x-www-form-urlencoded");
// 			request.SetRequestHeader("Accept", "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

// 			return request;
// 		}

// 		protected override void OnExchangeToken(string _accessToken,string _refreshToken)
// 		{
// 			AuthSettings.In.SetGoogleToken(false,_accessToken,_refreshToken);
// 		}

// 		protected override void OnRefreshToken(string _accessToken,string _refreshToken)
// 		{
// 			AuthSettings.In.SetGoogleToken(true,_accessToken,_refreshToken);
// 		}

// 		private string RandomDataBase64Uri(uint _length)
// 		{
// 			var cryptoProvider = new RNGCryptoServiceProvider();
// 			var bytes = new byte[_length];

// 			cryptoProvider.GetBytes(bytes);

// 			return Base64UriEncodeNoPadding(bytes);
// 		}

// 		private string Base64UriEncodeNoPadding(byte[] _buffer)
// 		{
// 			var base64 = Convert.ToBase64String(_buffer);
// 			base64 = base64.Replace("+","-");
// 			base64 = base64.Replace("/","_");
// 			base64 = base64.Replace("=","");

// 			return base64;
// 		}

// 		private byte[] ToSha256(string _input)
// 		{
// 			var sha256 = new SHA256Managed();

// 			return sha256.ComputeHash(Encoding.ASCII.GetBytes(_input));
// 		}

// 		private int GetRandomUnusedPort()
// 		{
// 			// Based on: http://stackoverflow.com/a/3978040.
// 			var listener = new TcpListener(IPAddress.Loopback,0);
// 			listener.Start();

// 			var port = ((IPEndPoint) listener.LocalEndpoint).Port;
// 			listener.Stop();

// 			return port;
// 		}
// 	}
// }
// #endif