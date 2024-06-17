using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TextSchedule : Schedule
{
	[SerializeField,LabelText("텍스트 메쉬")]
	protected TMP_Text m_TextMesh = null;

	[SerializeField,LabelText("텍스트들")]
	private List<string> m_TextList = new();
	[SerializeField,LabelText("무작위 모드")]
	private bool m_RandomMode = false;
	[SerializeField,LabelText("보여지는 시간"),MinValue(0.01f)]
	private float m_ShowDuration = 0.01f;

	[SerializeField,LabelText("반복 횟수"),PropertyTooltip("-1은 무한/0은 작동 안함")]
	protected int m_LoopCount = 1;

	private int m_Index = 0;

	[VerticalGroup("인덱스",Order = 99),ShowInInspector,PropertyRange(0,nameof(MaxIndex)),LabelText("인덱스"),ShowIf(nameof(IsPlayable))]
	public virtual int Index
	{
		get => m_Index;
		set
		{
			m_Index = Mathf.Clamp(value,0,MaxIndex);

			SetIndex(value);
		}
	}

	private bool IsPlayable => m_TextList.Any();
	private int MaxIndex => m_TextList.Count-1;

	private readonly List<int> m_IndexList = new();

	protected async override UniTask DoPlayScheduleAsync(ScheduleParam _)
	{
		m_IndexList.Clear();
		m_IndexList.AddRange(Enumerable.Range(0,MaxIndex+1));

		if(m_RandomMode)
		{
			m_IndexList.Randomize();
		}

		await CommonUtility.LoopUniTaskAsync(PlayAsync,m_LoopCount,m_Source.Token);
	}

	private async UniTask PlayAsync()
	{
		for(var i=0;i<m_IndexList.Count;i++)
		{
			SetIndex(m_IndexList[i]);

			await UniTask.WaitForSeconds(m_ShowDuration,cancellationToken : m_Source.Token);

			if(m_Source.IsCancellationRequested)
			{
				return;
			}
		}
	}

	protected void SetIndex(int _index)
	{
		m_TextMesh.SetLocalizeText(m_TextList[_index]);
	}
}