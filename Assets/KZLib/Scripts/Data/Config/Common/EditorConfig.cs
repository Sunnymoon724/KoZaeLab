using System;
using System.Collections.Generic;
using KZLib;
using KZLib.KZData;
using Newtonsoft.Json;

namespace ConfigData
{
	/// <summary>
	/// EditorConfig is used to store scene parameters for test in editor.
	/// </summary>
	public class EditorConfig : IConfig
	{
		private Dictionary<string,object> SceneParamDict { get; set; }

		public SceneState.StateParam GetSceneParam(string sceneName,Type targetType)
		{
			if(!SceneParamDict.TryGetValue(sceneName,out var param))
			{
				return null;
			}

			if(param == null)
			{
				return null;
			}

			var text = JsonConvert.SerializeObject(param);

			LogSvc.System.I($"{sceneName} - Param(JsonText) : {text}");

			return JsonConvert.DeserializeObject(text,targetType) as SceneState.StateParam;
		}
	}
}