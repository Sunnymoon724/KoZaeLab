using System;
using System.Collections.Generic;
using R3;

namespace KZLib.Data
{
	public abstract class Tune : IDisposable
	{
		private bool m_disposed = false;

		private readonly Dictionary<string,Subject<Unit>> m_subjectDict = new();

		protected Observable<Unit> OnChangedWithStart(string key) => _OnChanged(key).Prepend(Unit.Default);

		protected delegate bool TryParseDelegate<T>(string text,out T result);

		public Tune()
		{
			_LoadAll();
		}

		public void Dispose()
		{
			_Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected Observable<Unit> _OnChanged(string key)
		{
			if(!m_subjectDict.TryGetValue(key, out var subject))
			{
				subject = new Subject<Unit>();

				m_subjectDict[key] = subject;
			}

			return subject;
		}

		private void _Dispose(bool disposing)
		{
			if(m_disposed)
			{
				return;
			}

			if(disposing)
			{
				foreach(var pair in m_subjectDict)
				{
					var subject = pair.Value;

					subject.Dispose();
				}

				m_subjectDict.Clear();
			}

			m_disposed = true;
		}

		protected abstract void _LoadAll();

		protected TTune _LoadValue<TTune>(string nameKey,TryParseDelegate<TTune> parser,TTune defaultValue)
		{
			if(PlayerPrefsManager.In.TryGetString(nameKey,out var text))
			{
				if(parser(text,out var result))
				{
					return result;
				}
				else
				{
					LogChannel.Data.E($"{nameKey} parse failed from {text}.");

					return defaultValue;
				}
			}
			else
			{
				_SetStringPlayerPrefs(nameKey,defaultValue.ToString());

				return defaultValue;
			}
		}

		protected void _SetValue<TValue>(ref TValue oldValue,TValue newValue,string nameKey,Action onValueChanged)
		{
			if(oldValue.Equals(newValue))
			{
				return;
			}

			oldValue = newValue;

			_SetStringPlayerPrefs(nameKey,newValue.ToString());

			onValueChanged?.Invoke();

			if(m_subjectDict.TryGetValue(nameKey,out var subject))
			{
				subject.OnNext(Unit.Default);
			}
		}

		private void _SetStringPlayerPrefs(string key,string value)
		{
			PlayerPrefsManager.In.SetString(key,value);
		}
	}
}