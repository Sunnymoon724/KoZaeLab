using Cysharp.Threading.Tasks;
using KZLib.KZSchedule;
using TransitionPanel;
using UnityEngine;

public class TransitionPanelUI : WindowUI2D
{
	[SerializeField]
	private TransitionSchedule m_Schedule = null;

	public override UITag Tag => UITag.TransitionPanelUI;

	/// <summary>
	/// 이미지의 알파값을 내려서 화면을 밝게 페이드 한다.
	/// </summary>
	public async UniTask PlayTransitionInAsync(TransitionData _data,bool _autoHide)
	{
		await PlayTransitionAsync(_data,_autoHide,true);
	}

	/// <summary>
	/// 이미지의 알파값을 올려서 화면을 어둡게 페이드 한다.
	/// </summary>
	public async UniTask PlayTransitionOutAsync(TransitionData _data,bool _autoHide)
	{
		await PlayTransitionAsync(_data,_autoHide,false);
	}

	private async UniTask PlayTransitionAsync(TransitionData _data,bool _autoHide,bool _reverse)
	{
		m_Schedule.SetData(_data);

		Hide(false);

		//? 이미지가 점점 사라져야 화면이 밝아진다.
		await m_Schedule.PlayScheduleAsync(new ProgressSchedule.ProgressParam(_data.Duration,_reverse));

		if(_autoHide)
		{
			Hide(true);
		}
	}
}