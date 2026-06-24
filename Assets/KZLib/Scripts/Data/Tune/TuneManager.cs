using System;
using KZLib.Utilities;

namespace KZLib.Data
{
	/// <summary>
	/// Lazy cache of <see cref="Tune"/> instances. Sole entry point for creating tunes.
	/// Release disposes all cached tunes via <see cref="LazyRegistry{TKey,TValue}.Release"/>.
	/// </summary>
	public class TuneManager : Singleton<TuneManager>
	{
		private readonly LazyRegistry<Type,Tune> m_registry = new();

		protected override void _Initialize()
		{
			base._Initialize();

			// Ensure PlayerPrefs is ready before any Tune constructor runs _LoadAll.
			_ = PlayerPrefsManager.In;
		}

		protected override void _Release(bool disposing)
		{
			if(disposing)
			{
				m_registry.Release();
			}

			base._Release(disposing);
		}

		public TTune Fetch<TTune>() where TTune : Tune
		{
			var tne = m_registry.Fetch(typeof(TTune),_TryCreate);

			if(tne is TTune result)
			{
				return result;
			}

			throw new InvalidOperationException($"Created tune type [{tne.GetType().Name}] does not match requested type [{typeof(TTune).Name}].");
		}

		/// <summary>nonPublic:true reaches protected Tune subclass constructors from outside the hierarchy.</summary>
		private bool _TryCreate(Type type,out Tune tne)
		{
			tne = Activator.CreateInstance(type,true) as Tune;

			return tne != null;
		}
	}
}
