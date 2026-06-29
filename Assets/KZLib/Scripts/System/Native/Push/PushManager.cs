using System;
using KZLib.Data;
using KZLib.Utilities;
using R3;

#if UNITY_ANDROID

using UnityEngine.Android;
using Unity.Notifications.Android;

#endif

#if UNITY_IOS

using Unity.Notifications.Ios;

#endif

namespace KZLib.Natives
{
	public class PushManager : Singleton<PushManager>
	{
		private PushManager() { }

		private bool m_isInitialized = false;

#if UNITY_ANDROID
		private const string c_channelId = "game_notification_channel";
#endif

		private string _PrefsKey(string name) => $"[{nameof(VibrationManager)}] {name}";

		private ReactivePrefs<bool> m_useNotification = null;
		public Observable<bool> OnChangedNotification => m_useNotification.OnChanged;
		public bool UseNotification
		{
			get => m_useNotification.Value;
			set => _ApplyNotification(value);
		}

		private ReactivePrefs<bool> m_useNightNotification = null;
		public Observable<bool> OnChangedNightNotification => m_useNightNotification.OnChanged;
		public bool UseNightNotification
		{
			get => m_useNightNotification.Value;
			set => _ApplyNightNotification(value);
		}

		private void _ApplyNotification(bool useNotification)
		{
			m_useNotification.TrySetValue(useNotification);
		}

		private void _ApplyNightNotification(bool useNightNotification)
		{
			m_useNightNotification.TrySetValue(useNightNotification);
		}

		protected override void _Initialize()
		{
			base._Initialize();

			m_useNotification = new ReactivePrefs<bool>(_PrefsKey(nameof(m_useNotification)),bool.TryParse,true);
			m_useNightNotification = new ReactivePrefs<bool>(_PrefsKey(nameof(m_useNightNotification)),bool.TryParse,true);
		}

		public void RequestPermission()
		{
#if UNITY_IOS && !UNITY_EDITOR
			using var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound, true);
#elif UNITY_ANDROID && !UNITY_EDITOR
			using(var version = new AndroidJavaClass("android.os.Build$VERSION"))
			{
				if(version.GetStatic<int>("SDK_INT") >= 33)
				{
					if(!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
					{
						Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
					}
				}
			}

			var channel = new AndroidNotificationChannel()
			{
				Id = c_channelId,
				Name = "Game Notification",
				Importance = Importance.High,
				Description = "Generic notifications for the game",
			};

			AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif

			m_isInitialized = true;
		}

		/// <summary>
		/// Schedules a local notification at <paramref name="targetTimestamp"/> (Unix seconds, UTC).
		/// Skipped when the server clock has not been synced, because delay calculation requires
		/// <see cref="ServerClockManager.UtcNow"/>.
		/// </summary>
		public void SendLocalPush(int id,string title,string body,long targetTimestamp)
		{
			if(!m_isInitialized)
			{
				return;
			}

			// if(!ServerClockManager.In.IsSynced)
			// {
			// 	LogChannel.Kit.W("Local push skipped because server clock is not synced.");

			// 	return;
			// }

			var serverTime = ServerClockManager.In.UtcNow;
			var targetTime = DateTimeOffset.FromUnixTimeSeconds(targetTimestamp).UtcDateTime;

			var second = (targetTime-serverTime).TotalSeconds;

			if(second <= 0)
			{
				return;
			}

			if(!UseNotification)
			{
				return;
			}

			var localTargetTime = targetTime.ToLocalTime();

			if(_IsNightTime(localTargetTime) && !UseNightNotification)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR
			_ScheduleAndroidNotification(id,title,body,targetTime);
#elif UNITY_IOS && !UNITY_EDITOR
			_ScheduleIOSNotification(id,title,body,localTargetTime);
#endif
		}

#if UNITY_ANDROID
		private void _ScheduleAndroidNotification(int id,string title,string body,DateTime targetTime)
		{
			var notification = new AndroidNotification
			{
				Title = title,
				Text = body,
				FireTime = targetTime,
				SmallIcon = "icon_0",
				LargeIcon = "icon_1",
				ShowInForeground = false
			};

			AndroidNotificationCenter.SendNotificationWithExplicitID(notification,c_channelId,id);
		}
#endif

#if UNITY_IOS
		private void _ScheduleIOSNotification(int id,string title,string body,DateTime localTargetTime)
		{
			var timeTrigger = new iOSNotificationCalendarTrigger
			{
				Year = localTargetTime.Year,
				Month = localTargetTime.Month,
				Day = localTargetTime.Day,
				Hour = localTargetTime.Hour,
				Minute = localTargetTime.Minute,
				Second = localTargetTime.Second,
				Repeats = false
			};

			var notification = new iOSNotification()
			{
				Identifier = id.ToString(),
				Title = title,
				Body = body,
				ShowInForeground = false,
				ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
				Trigger = timeTrigger
			};

			iOSNotificationCenter.ScheduleNotification(notification);
		}
#endif

		public void CancelPush(int id)
		{
			if(!m_isInitialized)
			{
				return;
			}

#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidNotificationCenter.CancelNotification(id);
#elif UNITY_IOS && !UNITY_EDITOR
			iOSNotificationCenter.RemoveScheduledNotification(id.ToString());
#endif
		}

		public void ClearNotification()
		{
			if(!m_isInitialized)
			{
				return;
			}
			
#if UNITY_IOS && !UNITY_EDITOR
			iOSNotificationCenter.ApplicationBadge = 0;
#elif UNITY_ANDROID && !UNITY_EDITOR
			AndroidNotificationCenter.CancelAllDisplayedNotifications();
#endif
		}

		private bool _IsNightTime(DateTime localTargetTime)
		{
			var gameCfg = ConfigManager.In.Fetch<GameConfig>();

			return KZTimeKit.IsTimeInRange(localTargetTime,gameCfg.LocalPushBlockStartHour,gameCfg.LocalPushBlockEndHour);
		}
	}
}