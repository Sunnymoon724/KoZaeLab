#if UNITY_EDITOR
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using UnityEditor;

namespace KZLib.Windows
{
	/// <summary>
	/// Editor window for manually exercising Discord, Google, and Trello webhook APIs.
	/// Shared helpers and result parsing live in this partial.
	/// </summary>
	public partial class WebhookTestWindow : OdinEditorWindow
	{
		private const string c_emptyResultName = "(No results)";
		private const string c_emptyResultId = "-";

		/// <summary>
		/// Name/id pair displayed in read-only Odin table lists.
		/// </summary>
		[Serializable]
		private record ResultInfo
		{
			[HorizontalGroup("Row"),HideLabel,ShowInInspector]
			public string Name { get; init; }
			[HorizontalGroup("Row"),HideLabel,ShowInInspector]
			public string Id { get; init; }

			public ResultInfo(string name,string id) { Name = name; Id = id; }

			public static ResultInfo CreatePlaceholder(string message) => new(message,c_emptyResultId);
		}

		/// <summary>
		/// Builds the default embed payload used by Discord test buttons.
		/// </summary>
		private MessageInfo[] _CreateTestMessageInfo()
		{
			return new MessageInfo[] { new("Test","Hello World") };
		}

		/// <summary>
		/// Loads the Ostrich.png template used by image upload tests.
		/// </summary>
		private bool _TryReadTemplateTestImage(out byte[] imageBytes)
		{
			imageBytes = KZEditorKit.ReadTemplateTestImage();

			if(!imageBytes.IsNullOrEmpty())
			{
				return true;
			}

			KZEditorKit.DisplayInfo("Ostrich.png was not found in the KZLib template folder.");

			return false;
		}

		/// <summary>
		/// Marshals webhook callbacks onto the editor main thread and repaints the window.
		/// </summary>
		private void _RunOnEditorMainThread(Action action)
		{
			if(action == null)
			{
				return;
			}

			EditorApplication.delayCall += () =>
			{
				action();
				Repaint();
			};
		}

		/// <summary>
		/// Converts a Trello/Drive JSON item into a table row when name and id are present.
		/// </summary>
		private bool _TryCreateResultInfo(string text,out ResultInfo resultInfo)
		{
			resultInfo = null;

			try
			{
				var json = JObject.Parse(text);

				if(!json.TryGetValue("name",out var nameValue))
				{
					return false;
				}

				if(!json.TryGetValue("id",out var idValue))
				{
					return false;
				}

				resultInfo = new ResultInfo(nameValue.ToString(),idValue.ToString());

				return true;
			}
			catch(Exception)
			{
				return false;
			}
		}
	}
}
#endif
