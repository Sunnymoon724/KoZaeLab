using KZLib;
using KZLib.KZUtility;

namespace GameData
{
	public abstract class Option : IGameData
	{
		protected abstract string OPTION_KEY { get; }
		protected abstract EventTag Tag { get; }

		protected SaveDataHandler m_SaveHandler = null;

		public virtual void Initialize()
		{
			m_SaveHandler = new("Option_Table",false);
		}

		protected void LoadOption<TData>(ref TData _data) where TData : class,new()
		{
			if(m_SaveHandler.HasKey(OPTION_KEY))
			{
				_data = GetOption<TData>();
			}
			else
			{
				_data = new TData();

				SaveOption(_data,false);
			}
		}

		public abstract void Release();

		protected TData GetOption<TData>(TData _default)
		{
			return m_SaveHandler.GetObject(OPTION_KEY,_default);
		}

		protected TData GetOption<TData>() where TData : class,new()
		{
			return m_SaveHandler.GetObject<TData>(OPTION_KEY,null);
		}

		protected void SaveOption(object _object,bool _notify)
		{
			m_SaveHandler.SetObject(OPTION_KEY,_object);

			if(_notify)
			{
				EventMgr.In.SendEvent(Tag);
			}
		}
	}
}