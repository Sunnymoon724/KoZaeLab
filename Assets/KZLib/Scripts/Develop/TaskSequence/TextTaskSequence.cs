using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using KZLib.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace KZLib.Development
{
	public class TextTaskSequence : TaskSequence
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

		[VerticalGroup("Index",Order = 99),ShowInInspector,PropertyRange(0,nameof(MaxIndex)),ShowIf(nameof(IsPlayable))]
		public virtual int Index
		{
			get => m_index;
			set
			{
				m_index = Mathf.Clamp(value,0,MaxIndex);

				_SetIndex(value);
			}
		}

		private bool IsPlayable => m_textList.Any();
		private int MaxIndex => m_textList.Count-1;

		private readonly List<int> m_indexList = new();

		protected async override UniTask _DoPlaySequenceAsync(Param _)
		{
			m_indexList.Clear();
			m_indexList.AddRange(Enumerable.Range(0,MaxIndex+1));

			if(m_randomMode)
			{
				KZRandomKit.Randomize(m_indexList);
			}

			var token = m_tokenSource.Token;

			for(var i=0;i<m_indexList.Count;i++)
			{
				_SetIndex(m_indexList[i]);

				await UniTask.Delay(TimeSpan.FromSeconds(m_showDuration),cancellationToken : token).SuppressCancellationThrow();

				if(token.IsCancellationRequested)
				{
					return;
				}
			}
		}

		private void _SetIndex(int index)
		{
			m_textMesh.SetLocalizeText(m_textList[index]);
		}
	}
}