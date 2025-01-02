#if UNITY_EDITOR
using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;

namespace KZLib
{
	[Serializable]
	public abstract class EditorCustom
	{
		protected static void SaveData<TData>(string key,TData data)
		{
			EditorPrefs.SetString(key,JsonConvert.SerializeObject(data));
		}

		protected static TData LoadData<TData>(string key) where TData : new()
		{
			var text = EditorPrefs.GetString(key,"");

			return text.IsEmpty() ? new TData() : JsonConvert.DeserializeObject<TData>(text);
		}

		[TitleGroup("Option",BoldTitle = false,Order = 0)]
		[VerticalGroup("Option/Button",Order = 0),Button("Reset Custom",ButtonSizes.Large)]
		protected void OnResetCustom()
		{
			_ResetCustom();
		}

		protected abstract void _ResetCustom();
	}
}
#endif