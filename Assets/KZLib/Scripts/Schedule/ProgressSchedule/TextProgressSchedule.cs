using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace KZLib.KZSchedule
{
	[RequireComponent(typeof(TMP_Text))]
	public class TextProgressSchedule : ProgressSchedule
	{
		[SerializeField,HideInInspector]
		private bool m_RandomMode = false;

		[SerializeField,HideInInspector]
		private List<int> m_IndexList = new();

		[SerializeField,LabelText("텍스트 메쉬")]
		protected TMP_Text m_TextMesh = null;

		[VerticalGroup("Text",Order = 0),ShowInInspector,LabelText("무작위 모드")]
		private bool RandomMode
		{
			get => m_RandomMode;
			set
			{
				m_RandomMode = value;

				InitializeIndexList();
			}
		}

		[VerticalGroup("Text",Order = 0),SerializeField,LabelText("텍스트들"),OnValueChanged(nameof(OnChangeTextList))]
		private List<string> m_TextList = new();

		private int MaxIndex => m_TextList.Count-1;

		private void OnChangeTextList()
		{
			InitializeIndexList();

			Progress = 0.0f;
		}

		protected override void StartSchedule()
		{
			base.StartSchedule();

			InitializeIndexList();
		}

		private void InitializeIndexList()
		{
			m_IndexList.Clear();
			m_IndexList.AddRange(Enumerable.Range(0,MaxIndex+1));

			if(RandomMode)
			{
				m_IndexList.Randomize();
			}
		}

		protected override void SetProgress(float _progress)
		{
			var index = Mathf.RoundToInt(_progress*MaxIndex);

			if(m_TextList.IsNullOrEmpty())
			{
				return;
			}

			m_TextMesh.SetLocalizeText(m_TextList[m_IndexList[index]]);
		}

		protected override void Reset()
		{
			base.Reset();

			if(!m_TextMesh)
			{
				m_TextMesh = GetComponent<TMP_Text>();
			}
		}
	}
}