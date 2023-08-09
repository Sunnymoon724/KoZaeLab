// using System.Collections.Generic;
// using System;
// using Newtonsoft.Json.Linq;

// namespace KZLib.Auth
// {
// 	[Serializable]
// 	public abstract class AuthData
// 	{
// 		public string Access_Token { get; private set; }
// 		public string Refresh_Token { get; private set; }

// 		public virtual void SetAuthData(JToken _token)
// 		{
// 			Access_Token = GetTokenToString(_token,"Access_Token");
// 			Refresh_Token = GetTokenToString(_token,"Refresh_Token");
// 		}

// 		public void SetToken(string _accessToken,string _refreshToken)
// 		{
// 			Access_Token = _accessToken;
// 			Refresh_Token = _refreshToken;
// 		}

// 		public void SetToken(string _accessToken)
// 		{
// 			Access_Token = _accessToken;
// 		}

// 		protected string GetTokenToString(JToken _token,string _key)
// 		{
// 			var data = _token[_key];

// 			return data == null ? string.Empty : data.ToString();
// 		}
// 	}

// 	[Serializable]
// 	public class GoogleAuthData : AuthData
// 	{
// 		public string Client_Id { get; set; }
// 		public string Client_Secret { get; set; }

// 		public string Project_Id { get; set; }

// 		public string Auth_URI { get; private set; }
// 		public string Token_URI { get; private set; }
// 		public string Auth_Provider_X509_Cert_URI { get; private set; }
// 		public List<string> Redirect_URIList { get; private set; }

// 		// public string Access_Code { get; private set; }

// 		// Drive
// 		// public string FOLDER_ID { get; set; }

// 		public override void SetAuthData(JToken _token)
// 		{
// 			base.SetAuthData(_token);

// 			Client_Id = GetTokenToString(_token,"Client_Id");
// 			Client_Secret = GetTokenToString(_token,"Client_Secret");

// 			Project_Id = GetTokenToString(_token,"Project_Id");

// 			Auth_URI = GetTokenToString(_token,"Auth_URI");
// 			Token_URI = GetTokenToString(_token,"Token_URI");

// 			Auth_Provider_X509_Cert_URI = GetTokenToString(_token,"Auth_Provider_X509_Cert_URI");

// 			Redirect_URIList = GetTokenToList(_token,"Redirect_URIList");
// 		}

// 		public void SetGoogleData(JToken _token)
// 		{
// 			base.SetAuthData(_token);

// 			Client_Id = GetTokenToString(_token,"client_id");
// 			Client_Secret = GetTokenToString(_token,"client_secret");

// 			Project_Id = GetTokenToString(_token,"project_id");

// 			Auth_URI = GetTokenToString(_token,"auth_uri");
// 			Token_URI = GetTokenToString(_token,"token_uri");

// 			Auth_Provider_X509_Cert_URI = GetTokenToString(_token,"auth_provider_x509_cert_url");

// 			Redirect_URIList = GetTokenToList(_token,"redirect_uris");
// 		}

// 		protected List<string> GetTokenToList(JToken _token,string _key)
// 		{
// 			var data = _token[_key];

// 			return data == null ? null : data.ToObject<List<string>>();
// 		}
		
// 		public bool IsExistAuthData => string.Concat(Client_Id,Project_Id,Client_Secret).IsEmpty() == false;
// 	}
// }