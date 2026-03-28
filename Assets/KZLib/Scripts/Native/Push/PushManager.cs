using System;
using KZLib.Data;
using KZLib.Utilities;

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
		private bool m_isInitialized = false;

#if UNITY_ANDROID
		private const string CHANNEL_ID = "game_notification_channel";
#endif

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
				Id = CHANNEL_ID,
				Name = "Game Notification",
				Importance = Importance.High,
				Description = "Generic notifications for the game",
			};

			AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif

			m_isInitialized = true;
		}

		public void SendLocalPush(int id,string title,string body,long targetTimestamp)
		{
			var serverTime = GameTimeManager.In.GetCurrentTime(false);
			var targetTime = DateTimeOffset.FromUnixTimeSeconds(targetTimestamp).UtcDateTime;

			var second = (targetTime - serverTime).TotalSeconds;

			if(second <= 0)
			{
				return;
			}

			var nativeTune = TuneManager.In.FetchTune<NativeTune>();

			if(!nativeTune.UseNotification)
			{
				return;
			}

			var localTargetTime = targetTime.ToLocalTime();

			if(_IsNightTime(localTargetTime) && !nativeTune.UseNightNotification)
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

			AndroidNotificationCenter.SendNotificationWithExplicitID(notification,CHANNEL_ID,id);
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
			var gameCfg = ConfigManager.In.FetchConfig<GameConfig>();

			return KZTimeKit.IsTimeInRange(localTargetTime,gameCfg.LocalPushBlockStartHour,gameCfg.LocalPushBlockEndHour);
		}
	}
}