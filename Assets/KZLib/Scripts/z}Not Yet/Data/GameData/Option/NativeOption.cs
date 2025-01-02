using KZLib.KZUtility;
using Newtonsoft.Json;

namespace GameData
{
	public class NativeOption : Option
	{
		protected override string OPTION_KEY => "Native Option";
		protected override EventTag Tag => EventTag.ChangeLanguageOption;

		private class NativeData
		{
			[JsonProperty("UseVibration")]
			private bool m_UseVibration = true;

			[JsonIgnore]
			public bool UseVibration => m_UseVibration;

			public bool SetUseVibration(bool _vibration)
			{
				if(m_UseVibration == _vibration)
				{
					return false;
				}

				m_UseVibration = _vibration;

				return true;
			}
		}

		private NativeData m_NativeData = null;

		public override void Initialize()
		{
			base.Initialize();

			LoadOption(ref m_NativeData);
		}

		public override void Release()
		{
			SaveOption(m_NativeData,false);
		}

		public bool UseVibration
		{
			get => m_NativeData.UseVibration;
			set
			{
				if(m_NativeData.SetUseVibration(value))
				{
					return;
				}

				LogTag.System.I($"Vibration is changed. [{value}]");

				SaveOption(m_NativeData,true);
			}
		}
	}
}