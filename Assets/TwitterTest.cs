using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Twity;
using Twity.Helpers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TwitterTest : MonoBehaviour
{
	[SerializeField]
	private Button m_Button;

	[SerializeField]
	private UniWebView m_UniWebView;

	[SerializeField]
	private Text m_Text;

	private void Awake()
	{
		m_Button.SetOnClickListener(()=>
		{
			StartCoroutine(Twity.Client.GenerateRequestToken((result)=>
			{
				if(result)
				{
					StartCoroutine(GetLogin((result)=>
					{
						Debug.Log("end");

					}));
				}
			}));
		});
	}

	public IEnumerator GetLogin(Action<bool> callback)
	{
		Debug.Log(Oauth.authorizeURL);

		UnityWebRequest request = UnityWebRequest.Get(Oauth.authorizeURL);
		// request.SetRequestHeader("Authorization","");

		yield return request.SendWebRequest();

		Debug.Log(request.responseCode);
		// Debug.Log(request.error);

		if (request.isNetworkError) callback(false);

		if (request.responseCode == 200 || request.responseCode == 201)
		{
			// Helper.UrlEncode(param.Key)

			var text = request.downloadHandler.text.Replace(Oauth.authorizeURL,"http://127.0.0.1");

			//  <input name="redirect_after_login" type="hidden" value="https://api.twitter.com/oauth/authenticate?oauth_token=CnfNqwAAAAABpH3tAAABibnk9Ug">

			m_Text.text = text;

			Debug.Log(text);

			m_UniWebView.LoadHTMLString(text,null,true);

			m_UniWebView.OnPageFinished += (view, statusCode, url) =>
			{
				m_Text.text = "code :" +statusCode+" url : "+url;
			};

			m_UniWebView.OnMessageReceived += (view, message) =>
			{
				m_Text.text = message.RawMessage;
			};

			// m_UniWebView.OnPageErrorReceived  += (view, errorCode, message) =>
			// {
			// 	m_Text.text = $"Error Code: {errorCode}, Message: {message}";
			// };

			m_UniWebView.Show();

			callback(true);
		}
		else
		{
			callback(false);
		}
	}
}