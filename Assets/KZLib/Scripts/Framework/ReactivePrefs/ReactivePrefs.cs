using System;
using R3;

namespace KZLib.Utilities
{
	/// <summary>
	/// PlayerPrefs-backed reactive value. Loads via <see cref="TryParseDelegate"/>; persists with <see cref="object.ToString"/>.
	/// <see cref="TryParseDelegate"/> must round-trip with <typeparamref name="TValue"/>.ToString().
	/// On first load, a missing key, empty value, or parse failure writes the ctor default to storage.
	/// </summary>
	public class ReactivePrefs<TValue> : IDisposable
	{
		private bool m_disposed = false;

		private readonly ReactiveProperty<TValue> m_property = null;
		private readonly string m_nameKey = string.Empty;

		/// <summary>Parses the string stored in PlayerPrefs. Return false when the text is invalid.</summary>
		public delegate bool TryParseDelegate(string text,out TValue result);

		/// <summary>Current value. Throws after <see cref="Dispose"/>.</summary>
		public TValue Value
		{
			get
			{
				_ThrowIfDisposed();

				return m_property.Value;
			}
		}

		/// <summary>Emits the current value on subscribe, then on every subsequent change.</summary>
		public Observable<TValue> OnChanged => m_property;

		/// <summary>Loads from PlayerPrefs; falls back to <paramref name="defaultValue"/> when missing or invalid.</summary>
		/// <param name="nameKey">PlayerPrefs key (caller-defined).</param>
		/// <param name="onTryParse">Parser for the stored string.</param>
		/// <param name="defaultValue">Written to storage when load or parse fails.</param>
		public ReactivePrefs(string nameKey,TryParseDelegate onTryParse,TValue defaultValue)
		{
			if(nameKey.IsEmpty())
			{
				throw new ArgumentException("Name key cannot be empty.",nameof(nameKey));
			}

			if(onTryParse == null)
			{
				throw new ArgumentNullException(nameof(onTryParse));
			}

			m_nameKey = nameKey;
			m_property = new ReactiveProperty<TValue>(_LoadValue(onTryParse,defaultValue));
		}

		/// <summary>Reads and parses storage; resets to <paramref name="defaultValue"/> when load or parse fails.</summary>
		private TValue _LoadValue(TryParseDelegate onTryParse,TValue defaultValue)
		{
			if(PlayerPrefsManager.In.TryGetString(m_nameKey,out var text) && !text.IsEmpty())
			{
				if(onTryParse(text,out var result))
				{
					return result;
				}

				LogChannel.Storage.E($"{m_nameKey} parse failed from [{text}]. Reset to default [{defaultValue}].");
			}

			_SetStringPlayerPrefs(defaultValue);

			return defaultValue;
		}

		/// <summary>Persists when the value differs.</summary>
		public bool TrySetValue(TValue newValue)
		{
			_ThrowIfDisposed();

			if(m_property.Value.Equals(newValue))
			{
				return false;
			}

			m_property.Value = newValue;

			_SetStringPlayerPrefs(newValue);

			return true;
		}

		/// <summary>Writes <paramref name="value"/>.ToString().</summary>
		private void _SetStringPlayerPrefs(TValue value)
		{
			PlayerPrefsManager.In.SetString(m_nameKey,value.ToString());
		}

		public void Dispose()
		{
			if(m_disposed)
			{
				return;
			}

			m_disposed = true;
			m_property.Dispose();
		}

		/// <summary>Resets to <paramref name="defaultValue"/> (persists when the value changes).</summary>
		public void Reset(TValue defaultValue)
		{
			TrySetValue(defaultValue);
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