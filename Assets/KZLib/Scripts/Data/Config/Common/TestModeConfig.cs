using System;
using System.Collections.Generic;
using KZLib.Scenes;
using Newtonsoft.Json;

namespace KZLib.Data
{
	/// <summary>
	/// Editor test-mode bootstrap data. Scene name → <see cref="ISceneTransient"/> payload in a single YAML file.
	/// Stored under <c>workRes:config/TestMode.yaml</c> (see <c>KZMenu/Config/TestMode</c>).
	/// Replaces the former <c>EditorConfig</c> / <c>Editor.yaml</c> naming.
	/// </summary>
	/// <remarks>
	/// Custom override: <c>CustomConfig/CustomTestMode.yaml</c> (editor only, highest priority).
	/// Consumed from <see cref="KZLib.BaseMain.ApplyTestModeSceneTransient"/> in game <c>Main</c> subclasses.
	/// </remarks>
	public class TestModeConfig : IConfig
	{
		private Dictionary<string,object> SceneTransientDict { get; set; }

		/// <summary>Returns scene transient data deserialized to <typeparamref name="TTransient"/>, or false when missing or invalid.</summary>
		public bool TryGetSceneTransient<TTransient>(string sceneName,out TTransient transient) where TTransient : class,ISceneTransient
		{
			if(TryGetSceneTransient(sceneName,typeof(TTransient),out var result) && result is TTransient castValue)
			{
				transient = castValue;

				return true;
			}

			transient = null;

			return false;
		}

		/// <summary>Returns scene transient data deserialized to <paramref name="transientType"/>, or false when missing or invalid.</summary>
		public bool TryGetSceneTransient(string sceneName,Type transientType,out ISceneTransient transient)
		{
			transient = null;

			if(string.IsNullOrEmpty(sceneName) || transientType == null || SceneTransientDict == null || !SceneTransientDict.TryGetValue(sceneName,out var result))
			{
				return false;
			}

			if(!typeof(ISceneTransient).IsAssignableFrom(transientType))
			{
				return false;
			}

			var text = JsonConvert.SerializeObject(result);

			transient = JsonConvert.DeserializeObject(text,transientType) as ISceneTransient;

			return transient != null;
		}
	}
}
