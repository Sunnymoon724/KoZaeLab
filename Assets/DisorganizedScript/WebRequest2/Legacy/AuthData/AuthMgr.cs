// #if UNITY_EDITOR
// using System.Collections.Generic;
// using System;
// using Newtonsoft.Json.Linq;
// using UnityEditor;
// using Newtonsoft.Json;

// namespace KZLib.Auth
// {
// 	/// <summary>
// 	/// Assets 폴더 밖에 저장되므로 따로 암호화 하지 않는다.
// 	/// </summary>
// 	public class AuthMgr : Singleton<AuthMgr>
// 	{
// 		private Dictionary<string,AuthData> m_AuthDataDict;

// 		private Dictionary<string,AccessTokenProvider> m_AccessTokenProviderDict;

// 		// private static string FILE_PATH => Tools.PathCombine(Global.NOT_SHARED_FOLDER_PATH,"AuthData.txt");

// 		public AuthMgr()
// 		{
// 			// m_AuthDataDict = new Dictionary<string,AuthData>();

// 			// if(Tools.TryReadDataFromFile(FILE_PATH,out var outText) == true)
// 			// {
// 			// 	AddWebRequestData(outText);
// 			// }
// 		}

// 		~AuthMgr() => Release(false);

// 		protected override void Release(bool _disposing)
// 		{
// 			if(IsDispose == true)
// 			{
// 				return;
// 			}

// 			if(_disposing == true)
// 			{
// 				m_AuthDataDict.Clear();
// 			}
			
// 			base.Release(_disposing);
// 		}

// 		private void AddWebRequestData(string _text)
// 		{
// 			if(_text.IsEmpty() == true)
// 			{
// 				return;
// 			}

// 			try
// 			{
// 				var data = JObject.Parse(_text);

// 				foreach(var property in data.Properties())
// 				{
// 					if(m_AuthDataDict.ContainsKey(property.Name) == true)
// 					{
// 						continue;
// 					}

// 					var authData = CreateAuthData(property.Name);

// 					authData.SetAuthData(property.Value);

// 					m_AuthDataDict.Add(property.Name,authData);
// 				}
// 			}
// 			catch(JsonException _ex)
// 			{
// 				Log.Library.E(_ex);
// 			}
// 		}

// 		private AuthData CreateAuthData(string _authType)
// 		{
// 			switch(_authType)
// 			{
// 				case Global.GOOGLE_AUTH:
// 					return new GoogleAuthData();
// 				// case Global.KAKAO_AUTH:
// 				// 	return new KakaoAuthData();
// 				default:
// 					return null;
// 			}
// 		}

// 		public T GetAuthData<T>(string _authType) where T : AuthData
// 		{
// 			if(m_AuthDataDict.ContainsKey(_authType) == false)
// 			{
// 				m_AuthDataDict.Add(_authType,Activator.CreateInstance<T>());
// 			}

// 			return (T) m_AuthDataDict[_authType];
// 		}

// 		public void WriteAllAuthData()
// 		{
// 			// var data = JObject.FromObject(m_AuthDataDict);
			
// 			// Tools.WriteDataToFile(FILE_PATH,data.ToString());

// 			// AssetDatabase.Refresh();
// 		}

// 		public void ClearAuthData(string _authType)
// 		{
// 			m_AuthDataDict.RemoveSafe(_authType);

// 			WriteAllAuthData();
// 		}

// 		public T GetAccessTokenProvider<T>(string _authType) where T : AccessTokenProvider
// 		{
// 			if(m_AccessTokenProviderDict.ContainsKey(_authType) == false)
// 			{
// 				m_AccessTokenProviderDict.Add(_authType,Activator.CreateInstance<T>());
// 			}

// 			return (T) m_AccessTokenProviderDict[_authType];
// 		}
// 	}
// }
// #endif