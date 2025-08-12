using System.Collections.Generic;
using KZLib.KZData;
using Newtonsoft.Json;

namespace ConfigData
{
	/// <summary>
	/// EditorConfig is test data in editor.
	/// </summary>
	public class EditorConfig : IConfig
	{
		private Dictionary<string,object> AffixDict { get; set; }

		public TAffix GetAffix<TAffix>() where TAffix : class,IAffix
		{
			var type = typeof(TAffix);

			if(!AffixDict.TryGetValue(type.Name,out var result))
			{
				return null;
			}

			var text = JsonConvert.SerializeObject(result);

			return JsonConvert.DeserializeObject(text,type) as TAffix;
		}
	}
}