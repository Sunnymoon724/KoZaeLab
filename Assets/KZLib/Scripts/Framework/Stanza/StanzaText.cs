using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace KZLib.Utilities
{
	/// <summary>
	/// Cycles through localized strings on a <see cref="TMP_Text"/> at a fixed interval.
	/// Supports sequential or randomized order via <see cref="m_randomMode"/>.
	/// </summary>
	public class StanzaText : Stanza
	{
		[SerializeField]
		protected TMP_Text m_textMesh = null;

		[SerializeField]
		private List<string> m_textList = new();
		[SerializeField]
		private bool m_randomMode = false;
		[SerializeField,MinValue(0.01f)]
		private float m_showDuration = 0.01f;

		private int m_index = 0;

		/// <summary>Current text index; clamped and applied immediately in the inspector when playable.</summary>
		[VerticalGroup("Index",Order = 99),ShowInInspector,PropertyRange(0,nameof(MaxIndex)),ShowIf(nameof(IsPlayable))]
		public virtual int Index
		{
			get => m_index;
			set
			{
				m_index = Mathf.Clamp(value,0,MaxIndex);

				_SetIndex(m_index);
			}
		}

		private bool IsPlayable => m_textList.Count > 0;
		private int MaxIndex => m_textList.Count-1;

		/// <summary>Shuffled or sequential playback order built at play time.</summary>
		private readonly List<int> m_indexList = new();

		protected async override UniTask _DoPlayAsync(Param _)
		{
			m_indexList.Clear();

			for(var i=0;i<=MaxIndex;i++)
			{
				m_indexList.Add(i);
			}

			if(m_randomMode)
			{
				KZRandomKit.Randomize(m_indexList);
			}

			var token = m_tokenSource.Token;

			for(var i=0;i<m_indexList.Count;i++)
			{
				_SetIndex(m_indexList[i]);

				await UniTask.Delay(TimeSpan.FromSeconds(m_showDuration),cancellationToken : token);
			}
		}

		/// <summary>Applies the localized string at <paramref name="index"/> to <see cref="m_textMesh"/>.</summary>
		private void _SetIndex(int index)
		{
			if(!m_textMesh)
			{
				LogChannel.Develop.E($"TextMesh is null in {gameObject.name}. TextMesh must be assigned.");

				return;
			}

			m_textMesh.SetLocalizeText(m_textList[index]);
		}
	}
}
