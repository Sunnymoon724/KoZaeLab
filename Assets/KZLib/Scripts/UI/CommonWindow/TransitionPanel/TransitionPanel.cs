using System;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.UI
{
	/// <summary>
	/// Full-screen fade overlay driven by <see cref="TransitionStanzaLerp"/>.
	/// Invoked via <see cref="KZLib.UIManager.PlayTransitionInAsync"/> / <see cref="KZLib.UIManager.PlayTransitionOutAsync"/>.
	/// </summary>
	public class TransitionPanel : BasePanel
	{
		[SerializeField,Required]
		private TransitionStanzaLerp m_stanzaLerp = null;

		/// <summary>Prevents overlapping calls from toggling <see cref="Window2D.Hide"/> mid-transition.</summary>
		private bool m_isPlayingTransition = false;

		/// <summary>
		/// Shows the overlay, plays the fade, then optionally hides the panel again.
		/// </summary>
		/// <param name="isAutoHide">
		/// When <c>true</c>, calls <see cref="Hide"/>(<c>true</c>) after the fade.
		/// Scene changes keep the overlay visible between steps with <c>false</c>.
		/// </param>
		/// <param name="isReverse">
		/// <c>false</c> = fade out (cover screen); <c>true</c> = fade in (reveal screen).
		/// Matches <see cref="StanzaLerp.Param.IsReverse"/>.
		/// </param>
		public async UniTask PlayTransitionAsync(bool isAutoHide,bool isReverse)
		{
			if(!m_stanzaLerp)
			{
				throw new InvalidOperationException($"{NameTag} stanza lerp is not assigned.");
			}

			if(!m_stanzaLerp.IsPlayable)
			{
				throw new InvalidOperationException($"{NameTag} transition duration must be greater than zero.");
			}

			if(m_isPlayingTransition)
			{
				// Stanza.PlayAsync also ignores overlap; blocking here avoids a premature Hide(true).
				LogChannel.UI.W($"{NameTag} is already playing a transition.");

				return;
			}

			m_isPlayingTransition = true;

			try
			{
				Hide(false);

				// Duration comes from the serialized TransitionStanzaLerp when null.
				await m_stanzaLerp.PlayAsync(new StanzaLerp.Param(null,isReverse));

				if(isAutoHide)
				{
					Hide(true);
				}
			}
			finally
			{
				m_isPlayingTransition = false;
			}
		}
	}
}
