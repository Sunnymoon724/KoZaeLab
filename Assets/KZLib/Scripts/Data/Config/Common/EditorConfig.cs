using System.Collections.Generic;
using KZLib.KZData;

namespace ConfigData
{
	public class EditorConfig : IConfig
	{
		private Dictionary<string,object> SceneParamDict { get; set; }

		public object GetSceneParamText(string sceneName)
		{
			if(!SceneParamDict.TryGetValue(sceneName,out var param))
			{
				return null;
			}

			return param;
		}
	}
}