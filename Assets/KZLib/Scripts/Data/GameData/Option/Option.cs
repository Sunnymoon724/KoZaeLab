using KZLib;
using KZLib.KZDevelop;

namespace GameData
{
	/// <summary>
	/// 디폴트 옵션은 게임세팅에서 가져온다.
	/// 여기 저장되는건 유저 옵션들
	/// </summary>
	public abstract class Option : IGameData
	{
		protected abstract string OPTION_KEY { get; }
		protected abstract EventTag Tag { get; }

		private class Handler : SaveDataHandler
		{
			protected override string TABLE_NAME => "Option_Table";

			protected override bool NewSave => true;
		}

		private readonly Handler m_SaveHandler = new();

		public abstract void Initialize();

		public abstract void Release();

		protected TData GetOption<TData>(TData _default)
		{
			return m_SaveHandler.GetObject(OPTION_KEY,_default);
		}

		protected void SaveOption(object _object)
		{
			m_SaveHandler.SetObject(OPTION_KEY,_object);

			Broadcaster.SendEvent(Tag);
		}
	}
}