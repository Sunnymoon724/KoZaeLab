using System;
using System.Collections.Generic;
using KZLib;
using KZLib.KZData;
using Newtonsoft.Json;

namespace ConfigData
{
	public class EditorConfig : IConfig
	{
		private Dictionary<string,string> SceneParamDict { get; set; }

		public SceneState.StateParam GetSceneParam(string sceneName,Type targetType)
		{
			if(!SceneParamDict.TryGetValue(sceneName,out var text))
			{
				return null;
			}

			if(text.IsEmpty())
			{
				return null;
			}
	
			return JsonConvert.DeserializeObject(text,targetType) as SceneState.StateParam;
		}
	}
}