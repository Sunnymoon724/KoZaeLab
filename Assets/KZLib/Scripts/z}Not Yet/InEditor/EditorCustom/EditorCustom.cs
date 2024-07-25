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

		[TitleGroup("옵션 설정",BoldTitle = false,Order = 0)]
		[VerticalGroup("옵션 설정/버튼",Order = 0),Button("커스텀 초기화",ButtonSizes.Large)]
		private void OnResetCustom()
		{
			DoResetCustom();
		}

		protected abstract void DoResetCustom();
	}
}
#endif