#if UNITY_EDITOR
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;

namespace KZLib.Windows
{
	public partial class WebhookTestWindow : OdinEditorWindow
	{
		[Serializable]
		private record ResultInfo
		{
			[HorizontalGroup("Name"),HideLabel,ShowInInspector]
			public string Name { get; init; }
			[HorizontalGroup("Id"),HideLabel,ShowInInspector]
			public string Id { get; init; }

			public ResultInfo(string name,string id) { Name = name; Id = id; }
		}

		private MessageInfo[] _CreateTestMessageInfo()
		{
			return new MessageInfo[] { new("Test","Hello World") };
		}

		private bool _TryCreateResultInfo(string text,out ResultInfo resultInfo)
		{
			var json = JObject.Parse(text);

			resultInfo = null;

			if(!json.TryGetValue("name",out var nameValue))
			{
				return false;
			}

			if(json.TryGetValue("id",out var idValue))
			{
				return false;
			}

			resultInfo = new ResultInfo(nameValue.ToString(),idValue.ToString());

			return true;
		}
	}
}
#endif