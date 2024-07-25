
namespace GameData
{
	public class NativeOption : Option
	{
		protected override string OPTION_KEY => "Native Option";
		protected override EventTag Tag => EventTag.ChangeNativeOption;

		private class Native
		{
			public bool UseVibration { get; set; }
		}

		private Native m_Native = null;

		public override void Initialize()
		{
			m_Native = GetOption(new Native());
		}

		public override void Release()
		{

		}

		public bool UseVibration
		{
			get => m_Native.UseVibration;
			set
			{
				if(m_Native.UseVibration == value)
				{
					return;
				}

				m_Native.UseVibration = value;

				SaveOption(m_Native);
			}
		}
	}
}