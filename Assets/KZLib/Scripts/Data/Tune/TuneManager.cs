using System;
using KZLib.Utilities;

namespace KZLib.Data
{
	public class TuneManager : Singleton<TuneManager>
	{
		private readonly LazyRegistry<Type,Tune> m_registry = new();

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_registry.Release();
			}

			base._Release(disposing);
		}

		public TTune FetchTune<TTune>() where TTune : Tune
		{
			var type = typeof(TTune);

			return FetchTune(type) as TTune;
		}

		public Tune FetchTune(Type type)
		{
			return m_registry.Fetch(type,_TryCreateTune);
		}

		private bool _TryCreateTune(Type type,out Tune tune)
		{
			tune = Activator.CreateInstance(type) as Tune;

			return tune != null;
		}
	}
}