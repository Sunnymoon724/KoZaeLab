using System.Collections.Generic;
using KZLib.KZData;
using Newtonsoft.Json;

namespace ConfigData
{
	public class EditorConfig : IConfig
	{
		private Dictionary<string,object> SceneParamDict { get; set; }

		public string GetSceneParamText(string sceneName)
		{
			if(!SceneParamDict.TryGetValue(sceneName,out var param))
			{
				return null;
			}

			return JsonConvert.SerializeObject(param, new JsonSerializerSettings { FloatFormatHandling = FloatFormatHandling.DefaultValue, });
		}
	}
}