using System.Collections.Generic;
using Newtonsoft.Json;

namespace KZLib.KZData
{
	/// <summary>
	/// EditorConfig is test data in editor.
	/// </summary>
	public class EditorConfig : IConfig
	{
		private Dictionary<string,object> AffixDict { get; set; }

		private Dictionary<string,object> OptionDict { get; set; }

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

		public bool TryGetOption<TObject>(string optionKey,out TObject option)
		{
			if (OptionDict.TryGetValue(optionKey, out var result) && result is TObject castValue)
			{
				option = castValue;

				return true;
			}

			option = default;

			return false;
		}
	}
}