using System;
using UnityEngine;

namespace KZLib.Utilities
{
	/// <summary>
	/// Invokes registered callbacks when the host <see cref="GameObject"/> is destroyed.
	/// Use <see cref="GetOrAdd(GameObject)"/> on a follow target (or any owned object) to release pooled resources or end sessions
	/// without waiting for the next frame tick.
	/// </summary>
	/// <remarks>
	/// Multiple callbacks can <see cref="Bind"/> to the same watcher (e.g. several SFX lanes on one target).
	/// Call <see cref="Unbind"/> when releasing normally so callbacks do not run on a later destroy.
	/// For lambdas, store and pass the same <see cref="Action"/> reference to <see cref="Unbind"/>.
	/// Instance methods are removed by matching method and target; <c>null</c> is ignored by <see cref="Bind"/> and <see cref="Unbind"/>.
	/// </remarks>
	public sealed class FollowTargetWatcher : MonoBehaviour
	{
		private Action<GameObject> m_onDestroy = null;

		/// <summary>Returns an existing watcher on <paramref name="gameObject"/> or adds one. Returns <c>null</c> when <paramref name="gameObject"/> is null.</summary>
		public static FollowTargetWatcher GetOrAdd(GameObject gameObject)
		{
			if(!gameObject)
			{
				return null;
			}

			return gameObject.GetOrAddComponent<FollowTargetWatcher>();
		}

		/// <summary>Returns an existing watcher on <paramref name="component"/>'s GameObject or adds one. Returns <c>null</c> when <paramref name="component"/> is null.</summary>
		public static FollowTargetWatcher GetOrAdd(Component component)
		{
			if(!component)
			{
				return null;
			}

			return GetOrAdd(component.gameObject);
		}

		/// <summary>Returns an existing watcher on <paramref name="transform"/>'s GameObject or adds one. Returns <c>null</c> when <paramref name="transform"/> is null.</summary>
		public static FollowTargetWatcher GetOrAdd(Transform transform)
		{
			if(!transform)
			{
				return null;
			}

			return GetOrAdd(transform.gameObject);
		}

		/// <summary>Registers a callback invoked from <see cref="OnDestroy"/>. No-op when <paramref name="onDestroy"/> is <c>null</c>. Skips duplicate method-and-target pairs via <see cref="DelegateExtension.HasDelegate"/>.</summary>
		public void Bind(Action<GameObject> onDestroy)
		{
			if(onDestroy == null || m_onDestroy.HasDelegate(onDestroy))
			{
				return;
			}

			m_onDestroy += onDestroy;
		}

		/// <summary>Removes a previously registered callback. No-op when <paramref name="onDestroy"/> is <c>null</c>.</summary>
		public void Unbind(Action<GameObject> onDestroy)
		{
			if(onDestroy == null)
			{
				return;
			}

			m_onDestroy -= onDestroy;
		}

		/// <summary>Removes every registered callback without invoking them.</summary>
		public void Clear()
		{
			m_onDestroy = null;
		}

		private void OnDestroy()
		{
			m_onDestroy?.Invoke(gameObject);

			m_onDestroy = null;
		}
	}
}