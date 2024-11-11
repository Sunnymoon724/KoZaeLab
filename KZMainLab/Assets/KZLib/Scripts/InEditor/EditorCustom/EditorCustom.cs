#if UNITY_EDITOR
using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;

namespace KZLib.KZEditor
{
	[Serializable]
	public abstract class EditorCustom
	{
		protected static void SaveData<TData>(string _key,TData _data)
		{
			EditorPrefs.SetString(_key,JsonConvert.SerializeObject(_data));
		}

		protected static TData LoadData<TData>(string _key) where TData : new()
		{
			var text = EditorPrefs.GetString(_key,"");

			return text.IsEmpty() ? new TData() : JsonConvert.DeserializeObject<TData>(text);
		}

		[TitleGroup("Option",BoldTitle = false,Order = 0)]
		[VerticalGroup("Option/Button",Order = 0),Button("Reset Custom",ButtonSizes.Large)]
		private void OnResetCustom()
		{
			DoResetCustom();
		}

		protected abstract void DoResetCustom();
	}
}
#endif