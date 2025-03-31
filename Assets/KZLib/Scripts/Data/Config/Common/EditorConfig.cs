using System.Collections.Generic;
using KZLib;
using KZLib.KZData;

namespace ConfigData
{
	public class EditorConfig : IConfig
	{
		private Dictionary<string,object> SceneParamDict { get; set; }

		public SceneState.StateParam GetSceneParam(string sceneName)
		{
			if(!SceneParamDict.TryGetValue(sceneName,out var param))
			{
				return null;
			}

			return param as SceneState.StateParam;
		}
	}
}