using UnityEngine;

namespace KZLib.Utilities
{
	/// <summary>
	/// Per-object time flow: local <see cref="TimeScale"/> and <see cref="DeltaTime"/> independent of <see cref="UnityEngine.Time.timeScale"/>.
	/// Attach to units or other objects that need their own haste, slow, or pause (e.g. 2x move speed, 0.5x slow field, stun).
	/// </summary>
	/// <remarks>
	/// Use <see cref="DeltaTime"/> instead of <see cref="Time.deltaTime"/> in movement and gameplay logic.
	/// <see cref="Advance"/> is optional; call it only when <see cref="ElapsedTime"/> accumulation is required.
	/// Duration timers and buff expiry belong in buff/skill systems, not here.
	/// </remarks>
	public class TimeFlow : MonoBehaviour
	{
		/// <summary>Local elapsed time on this flow's timeline. Increments only when <see cref="Advance"/> is called.</summary>
		public float ElapsedTime { get; private set; }

		/// <summary>Delta time for this frame on the local timeline. Zero while paused or when <see cref="TimeScale"/> is zero.</summary>
		public float DeltaTime => IsTimeFrozen ? 0.0f : m_timeScale*Time.unscaledDeltaTime;

		/// <summary>Local time scale multiplier (1 = normal, 2 = twice as fast, 0.5 = half speed).</summary>
		public float TimeScale
		{
			get => m_timeScale;
			set => m_timeScale = Mathf.Max(0.0f,value);
		}

		/// <summary>True when explicitly paused via <see cref="Pause"/>. Does not change <see cref="TimeScale"/>.</summary>
		public bool IsPaused => m_isPaused;

		private bool IsTimeFrozen => m_isPaused || m_timeScale <= 0.0f;

		[SerializeField]
		private float m_timeScale = 1.0f;

		private bool m_isPaused = false;

		/// <summary>Advances <see cref="ElapsedTime"/> by <see cref="DeltaTime"/>. No-op while paused or frozen.</summary>
		public void Advance()
		{
			if(IsTimeFrozen)
			{
				return;
			}

			ElapsedTime += DeltaTime;
		}

		/// <summary>Freezes local time flow without changing <see cref="TimeScale"/>.</summary>
		public void Pause()
		{
			m_isPaused = true;
		}

		/// <summary>Resumes local time flow after <see cref="Pause"/>.</summary>
		public void Resume()
		{
			m_isPaused = false;
		}

		/// <summary>Sets <see cref="ElapsedTime"/> to zero. Does not affect <see cref="TimeScale"/> or pause state.</summary>
		public void ResetElapsedTime()
		{
			ElapsedTime = 0.0f;
		}

		/// <summary>Resets <see cref="ElapsedTime"/>, clears pause, and sets <see cref="TimeScale"/> to 1.</summary>
		public void Reset()
		{
			ResetElapsedTime();

			m_isPaused = false;
			m_timeScale = 1.0f;
		}
	}
}