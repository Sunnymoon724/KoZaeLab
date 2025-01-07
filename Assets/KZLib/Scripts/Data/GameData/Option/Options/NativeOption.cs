using KZLib.KZUtility;
using Newtonsoft.Json;

namespace GameData
{
	public class NativeOption : Option
	{
		protected override string OptionKey => "Native Option";
		protected override EventTag OptionTag => EventTag.ChangeNativeOption;

		private class NativeData : IOptionData
		{
			[JsonProperty("UseVibration")]
			private bool m_UseVibration = true;

			[JsonIgnore]
			public bool UseVibration => m_UseVibration;

			public bool TrySetUseVibration(bool useVibration)
			{
				if(m_UseVibration == useVibration)
				{
					return false;
				}

				m_UseVibration = useVibration;

				return true;
			}
		}

		private NativeData m_nativeData = null;

		public override void Initialize()
		{
			base.Initialize();

			m_nativeData = LoadOptionData<NativeData>();
		}

		public override void Release()
		{
			SaveOptionData(m_nativeData,false);
		}

		public bool UseVibration
		{
			get => m_nativeData.UseVibration;
			set
			{
				if(m_nativeData.TrySetUseVibration(value))
				{
					return;
				}

				LogTag.System.I($"Vibration is changed. [{value}]");

				SaveOptionData(m_nativeData,true);
			}
		}
	}
}