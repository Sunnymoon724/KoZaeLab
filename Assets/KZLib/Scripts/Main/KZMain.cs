using System;
using DG.Tweening;
using GameData;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KZLib
{
	public abstract class KZMain : SerializedMonoBehaviour
	{
#if UNITY_EDITOR
		private const string MAIN_DATA = "[Main] MainData";

		private class MainData
		{
			public PlayType GamePlayType { get; set; } = PlayType.Normal;
		}
#endif

		private enum PlayType { Test, Normal, }

		private PlayType? m_GamePlayType = null;

		[ShowInInspector,LabelText("현재 플레이 타입")]
		private PlayType GamePlayType
		{
			get
			{
				if(!m_GamePlayType.HasValue)
				{
#if UNITY_EDITOR
					var data = LoadData();

					m_GamePlayType = data.GamePlayType;
#else
					m_GamePlayType = PlayType.Normal;
#endif
				}

				return m_GamePlayType.Value;
			}
			set
			{
#if UNITY_EDITOR
				if(m_GamePlayType == value)
				{
					return;
				}

				var data = LoadData();

				m_GamePlayType = data.GamePlayType = value;

				SaveData(data);
#endif
			}
		}

		private bool IsTestMode => GamePlayType == PlayType.Test;

		protected virtual void Awake()
		{
			LogTag.System.I("메인 생성");
		}

		private async void Start()
		{
			if(!GameSettings.In.IsLiveMode && GameSettings.In.UseHeadUpDisplay)
			{
				UIMgr.In.Open<HudPanelUI>(UITag.HudPanelUI);
			}

			DOTween.Init(false,false,LogBehaviour.ErrorsOnly);
			DOTween.SetTweensCapacity(1000,100);

			// 모바일에서 화면잠김을 방지하기 위한 값.
			Screen.sleepTimeout = SleepTimeout.NeverSleep;

			var option = GameDataMgr.In.Access<GraphicOption>();
			var builder = new StringBuilder();

			InitializeResolution(builder,option);

			InitializeRenderSetting(builder);

			LogTag.System.I(builder.ToString());

			// TODO 메타 데이터 로드 위치 변경하기 (선택창으로 시작할때 로드 or 원할때 로드)
			await MetaDataMgr.In.LoadAllAsync();

			if(IsTestMode)
			{
#if UNITY_EDITOR
				InitializeTestMode();
#else
				throw new Exception("에디터가 아니면 테스트 모드를 할 수 없습니다.");
#endif
			}
			else
			{
				InitializeNormalMode();
			}
		}

#if UNITY_EDITOR
		private void SaveData(MainData _mainData)
		{
			EditorPrefs.SetString(MAIN_DATA,JsonConvert.SerializeObject(_mainData));
		}

		private MainData LoadData()
		{
			var text = EditorPrefs.GetString(MAIN_DATA,"");

			return text.IsEmpty() ? new MainData() : JsonConvert.DeserializeObject<MainData>(text);
		}

		protected virtual void InitializeTestMode() { }
#endif
		protected virtual void InitializeNormalMode() { }

		private void InitializeResolution(StringBuilder _builder,GraphicOption _option)
		{
			var resolution = _option.ScreenResolution;

			_builder.AppendFormat(string.Format("현재 해상도 {0}x{1}\n",resolution.Width,resolution.Width));
		}

		private void InitializeRenderSetting(StringBuilder _builder)
		{

		}

		protected virtual void OnApplicationQuit()
		{
			GameUtility.ReleaseManager();
		}

		protected virtual void Update()
		{
#if UNITY_EDITOR
			if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C) && GameSettings.In.UseHeadUpDisplay)
			{
				if(UIMgr.In.IsOpened(UITag.HudPanelUI))
				{
					UIMgr.In.Close(UITag.HudPanelUI);
				}
				else
				{
					UIMgr.In.Open<HudPanelUI>(UITag.HudPanelUI);
				}
			}

			//? 강제 버그 만들기
			if(Input.GetKeyDown(KeyCode.F4))
			{
				throw new Exception("강제 에러 생성!!");
			}
#endif
		}
	}
}