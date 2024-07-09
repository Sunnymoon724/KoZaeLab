
namespace GameData
{
	public class NativeOption : Option
	{
		protected override string OPTION_KEY => "Native Option";
		protected override EventTag Tag => EventTag.ChangeNativeOption;

		private class NativeData
		{
			public bool UseVibration { get; set; }
		}

		private NativeData m_NativeData = null;

		public override void Initialize()
		{
			m_NativeData = GetOption(new NativeData());
		}

		public override void Release()
		{

		}

		public bool UseVibration
		{
			get => m_NativeData.UseVibration;
			set
			{
				if(m_NativeData.UseVibration == value)
				{
					return;
				}

				m_NativeData.UseVibration = value;

				SaveOption(m_NativeData);
			}
		}
	}
}