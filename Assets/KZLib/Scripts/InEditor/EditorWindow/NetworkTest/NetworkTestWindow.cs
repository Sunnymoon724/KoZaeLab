#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;

namespace KZLib.KZWindow
{
	public partial class NetworkTestWindow : OdinEditorWindow
	{
		[Serializable]
		private record ResultData
		{
			[HorizontalGroup("Name"),HideLabel,ShowInInspector]
			public string Name { get; init; }
			[HorizontalGroup("Id"),HideLabel,ShowInInspector]
			public string Id { get; init; }

			public ResultData(string name,string id) { Name = name; Id = id; }
		}
	}
}
#endif