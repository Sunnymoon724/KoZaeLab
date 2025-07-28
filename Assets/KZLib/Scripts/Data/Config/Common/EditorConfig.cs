using System.Collections.Generic;
using KZLib.KZData;

namespace ConfigData
{
	/// <summary>
	/// EditorConfig is used to store scene parameters for test in editor.
	/// </summary>
	public class EditorConfig : IConfig
	{
		private Dictionary<string,string> ParamPathDict { get; set; }

		public string GetParamPath(string sceneName)
		{
			if(!ParamPathDict.TryGetValue(sceneName,out var paramPath))
			{
				return null;
			}

			return paramPath;
		}
	}
}