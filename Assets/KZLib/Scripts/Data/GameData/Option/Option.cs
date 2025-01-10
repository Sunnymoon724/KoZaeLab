using System;
using KZLib;
using KZLib.KZUtility;

namespace GameData
{
	public abstract class Option : IGameData
	{
		protected interface IOptionData { }

		private const string c_table_name = "Option_Table";

		protected abstract string OptionKey { get; }
		protected abstract EventTag OptionTag { get; }

		public virtual void Initialize() { }

		protected TData LoadOptionData<TData>() where TData : IOptionData
		{
			var defaultData = Activator.CreateInstance<TData>();

			if(LocalStorageMgr.In.HasKey(c_table_name,OptionKey))
			{
				return LocalStorageMgr.In.GetObject(c_table_name,OptionKey,defaultData);
			}
			else
			{
				SaveOptionData(defaultData,false);

				return defaultData;
			}
		}

		public abstract void Release();

		protected void SaveOptionData(IOptionData optionData,bool isNotify)
		{
			LocalStorageMgr.In.SetObject(c_table_name,OptionKey,optionData);

			if(isNotify)
			{
				EventMgr.In.SendEvent(OptionTag);
			}
		}
	}
}