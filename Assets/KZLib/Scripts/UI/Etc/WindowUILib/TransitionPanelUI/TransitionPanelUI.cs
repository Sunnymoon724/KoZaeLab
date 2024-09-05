using Cysharp.Threading.Tasks;
using KZLib.KZSchedule;
using TransitionPanel;
using UnityEngine;

public class TransitionPanelUI : WindowUI2D
{
	[SerializeField]
	private TransitionSchedule m_Schedule = null;

	public override UITag Tag => UITag.TransitionPanelUI;

	public async UniTask PlayTransitionAsync(TransitionData _data,bool _autoHide,bool _reverse)
	{
		m_Schedule.SetTransitionData(_data);

		Hide(false);

		//? 이미지가 점점 사라져야 화면이 밝아진다.
		await m_Schedule.PlayScheduleAsync(new ProgressSchedule.ProgressParam(_data.Duration,_reverse));

		if(_autoHide)
		{
			Hide(true);
		}
	}
}