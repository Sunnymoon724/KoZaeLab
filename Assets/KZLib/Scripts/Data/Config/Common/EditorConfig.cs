using System;
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

		public List<IAffix> GetAffixList()
		{
			var affixList = new List<IAffix>();

			foreach(var pair in AffixDict)
			{
				var typeText = pair.Key;
				var affixObject = pair.Value;
				var type = Type.GetType($"{typeText}, Assembly-CSharp") ?? throw new NullReferenceException($"{typeText} is not found");
				var affixText = JsonConvert.SerializeObject(affixObject);

				var newAffix = JsonConvert.DeserializeObject(affixText,type) as IAffix ?? throw new NullReferenceException($"{affixText} is not {type} type");

				affixList.Add(newAffix);
			}

			return affixList;
		}
	}
}