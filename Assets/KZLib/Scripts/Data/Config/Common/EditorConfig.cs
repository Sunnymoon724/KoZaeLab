using System.Collections.Generic;
using KZLib.KZData;

namespace ConfigData
{
	/// <summary>
	/// EditorConfig is used to store scene parameters for test in editor.
	/// </summary>
	public class EditorConfig : IConfig
	{
		private Dictionary<string,string> SceneParamPathDict { get; set; }

		public string GetSceneParamPath(string sceneName)
		{
			if(!SceneParamPathDict.TryGetValue(sceneName,out var paramPath))
			{
				return null;
			}

			return paramPath;
		}
	}
}