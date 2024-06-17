using KZLib;
using KZLib.KZDevelop;
using Newtonsoft.Json;
using UnityEngine;

namespace GameData
{
	public partial class Option : IGameData
	{
		private const string NATIVE_OPTION = "Native Option";

		public partial class Native
		{
			[SerializeField,JsonProperty("UseVibration")]
			private bool m_UseVibration = true;
			[JsonIgnore]
			public bool UseVibration
			{
				get => m_UseVibration;
				set
				{
					if(m_UseVibration == value)
					{
						return;
					}

					m_UseVibration = value;

					SaveNative();
				}
			}

			public Native()
			{
				m_UseVibration = true;
			}

			private void SaveNative()
			{
				s_SaveHandler.SetObject(NATIVE_OPTION,this);

				Broadcaster.SendEvent(EventTag.ChangeNativeOption);
			}
		}

		public Native NativeOption { get; private set; }

		private void InitializeNative()
		{
			NativeOption = s_SaveHandler.GetObject(NATIVE_OPTION,new Native());
		}

		private void ReleaseNative() { }
	}
}