using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using KZLib.Network;
using Twity;
using Twity.Helpers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TwitterTest1 : MonoBehaviour
{
	[SerializeField]
	private Button m_Button;

	[SerializeField]
	private WebViewObject m_WebView;
	private RectTransform m_RectTransform;

	[SerializeField]
	private Text m_Text;

	private void Awake()
	{
		m_WebView.Init(
            null,false,"",
            (msg) =>
            {
                Debug.Log(string.Format("CallOnError[{0}]", msg));
                m_Text.text = msg;
            });

		m_WebView.SetMargins((int)m_RectTransform.offsetMin.x,(int)m_RectTransform.offsetMax.y,-(int)m_RectTransform.offsetMax.x,-(int)m_RectTransform.offsetMin.y);

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

			m_WebView.LoadHTML(text,null);

			// m_UniWebView.OnPageFinished += (view, statusCode, url) =>
			// {
			// 	m_Text.text = "code :" +statusCode+" url : "+url;
			// };

			// m_UniWebView.OnMessageReceived += (view, message) =>
			// {
			// 	m_Text.text = message.RawMessage;
			// };

			// m_UniWebView.OnPageErrorReceived  += (view, errorCode, message) =>
			// {
			// 	m_Text.text = $"Error Code: {errorCode}, Message: {message}";
			// };

			m_WebView.SetVisibility(true);

			callback(true);
		}
		else
		{
			callback(false);
		}
	}
}