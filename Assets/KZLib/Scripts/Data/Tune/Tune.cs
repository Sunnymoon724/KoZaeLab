using System;
using System.Collections.Generic;
using R3;

namespace KZLib.Data
{
	/// <summary>
	/// Base for user settings stored in PlayerPrefs.
	/// Subclasses hold domain fields, expose setters and Observables; System managers subscribe and apply side effects.
	/// Create only through <see cref="TuneManager.Fetch{TTune}"/> (protected constructor).
	/// </summary>
	public abstract class Tune : IDisposable
	{
		private bool m_disposed = false;

		private readonly Dictionary<string,Subject<Unit>> m_subjectDict = new();

		/// <summary>Notifies on change and emits once immediately so subscribers get the current value.</summary>
		protected Observable<Unit> OnChangedWithStart(string key) => _OnChanged(key).Prepend(Unit.Default);

		protected delegate bool TryParseDelegate<T>(string text,out T result);

		protected Tune()
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
			_ThrowIfDisposed();

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

		/// <summary>Load all fields from PlayerPrefs. Called once from the constructor.</summary>
		protected abstract void _LoadAll();

		/// <summary>PlayerPrefs key: {TypeName}.{fieldName} (e.g. SoundTune.m_masterVolume).</summary>
		private string _BuildPlayerPrefsKey(string nameKey) => $"{GetType().Name}.{nameKey}";

		/// <summary>Read a value; missing key or parse failure writes and returns defaultValue.</summary>
		protected TTune _LoadValue<TTune>(string nameKey,TryParseDelegate<TTune> parser,TTune defaultValue)
		{
			var prefsKey = _BuildPlayerPrefsKey(nameKey);

			if(!PlayerPrefsManager.In.TryGetString(prefsKey,out var text) || text.IsEmpty())
			{
				_SetStringPlayerPrefs(nameKey,defaultValue.ToString());

				return defaultValue;
			}

			if(parser(text,out var result))
			{
				return result;
			}

			LogChannel.Data.E($"{prefsKey} parse failed from [{text}]. Reset to default [{defaultValue}].");

			_SetStringPlayerPrefs(nameKey,defaultValue.ToString());

			return defaultValue;
		}

		/// <summary>Persist, optional synchronous callback, then notify subscribers for nameKey.</summary>
		protected void _SetValue<TValue>(ref TValue oldValue,TValue newValue,string nameKey,Action onValueChanged)
		{
			_ThrowIfDisposed();

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

		private void _SetStringPlayerPrefs(string nameKey,string value)
		{
			PlayerPrefsManager.In.SetString(_BuildPlayerPrefsKey(nameKey),value);
		}

		private void _ThrowIfDisposed()
		{
			if(m_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}
	}
}