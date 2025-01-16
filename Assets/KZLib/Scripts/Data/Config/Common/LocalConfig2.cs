// using KZLib;
// using Newtonsoft.Json;
// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Reflection;
// using UnityEngine;
// using UnityEngine.Events;

// namespace ConfigData
// {
//     public class LocalConfig : IConfigData
//     {
//         // 저장되는 놈들 --------------------------------------------------------
//         public string AccountEnv { get; private set; } = string.Empty;
//         public string ChannelUid { get; private set; } = string.Empty;
//         public string Token { get; private set; } = string.Empty;
//         public bool IsAlreadyUsePrivacyPopup { get; private set; } = false;

//         public string LastShowGameOverDateTime { get; private set; } = string.Empty;
//         public string Language { get; private set; } = string.Empty;
//         //public ELanguage VoiceLanguage => UTIL.StrToEnumV2(this.Language, ELanguage.JAPANESE); // HHJ: 나중에 옵션 추가되면 제대로 만들기
//         public string VoiceLanguage { get; private set; } = string.Empty;

//         public string MatchBlockListString { get; private set; } = "[]";

//         public bool IsShowHaveHeroInEquipHeroTab { get; private set; } = true;
//         public bool IsShowHaveItemInEquipArtifactTab { get; private set; } = true;
//         public bool IsShowHaveItemInEquipPetTab { get; private set; } = true;
//         //FireBase 이벤트 보낼때 사용
//         public int GameOpenCnt { get; private set; } = 0;
//         public string CurMailChecksum { get; private set; } = string.Empty;

//         public int SelectedCostumeNum { get; private set; } = -1;


//         public float BgmVolume { get { return _bgmVolume; } private set { _bgmVolume = value; } }
//         public float EffVolume { get { return _effVolume; } private set { _effVolume = value; } }
//         public bool IsUseAlarm { get { return _isUseAlarm; } private set { _isUseAlarm = value; } }
//         public bool IsUseVibrate { get { return _isUseVibrate; } private set { _isUseVibrate = value; } }
//         public bool IsUseAutoScreenSaver { get { return _isUseAutoScreenSaver; } private set { _isUseAutoScreenSaver = value; } }
//         public bool IsUseSleep { get { return _isUseSleep; } private set { _isUseSleep = value; } }
//         public bool IsUseAutoSkill { get { return _isUseAutoSkill; } private set { _isUseAutoSkill = value; } }
//         public bool IsLastUseMaximumChatSize { get => _isLastUseMaximumChatSize; private set => _isLastUseMaximumChatSize = value; }

//         public int CameraType { get { return _cameraType; } private set { _cameraType = value; } }
//         public int ChatSizeMode { get { return _chatSizeMode; } private set { _chatSizeMode = value; } }

//         public int LeaderActorNum { get; private set; } = 0;

//         public string NewEquipItemNumText { get; private set; } = "";
//         public string InvalidGuideGruopNumText { get; private set; } = "";


//         public bool IsShowGoldGrowthDialog { get; private set; } = false;
//         public bool IsShowGachaHeroDialog { get; private set; } = false;
//         public bool IsShowEquipHeroDialog { get; private set; } = false;
//         public bool IsShowFirstBreakDialog { get; private set; } = false;
//         public bool IsShowCashDungeonDialog { get; private set; } = false;

//         public string IsAlreadyShowTutorialList { get; private set; } = "";
//         public bool IsGoogleConnected { get; private set; } = false;
//         public bool IsAppleConnected { get; private set; } = false;

//         public int TargetFrameRateLv { get { return _targetFrameRateLv; } private set { _targetFrameRateLv = value; } }
//         public int ResolutionScaleLv { get { return _resolutionScaleLv; } private set { _resolutionScaleLv = value; } }
//         public bool IsAutoResolution { get { return _isAutoResolution; } private set { _isAutoResolution = value; } }

//         //public bool NotiDayEnable { get { return _notiDayEnable; } private set { _notiDayEnable = value; } }
//         //public bool NotiNightEnable { get { return _notiNightEnable; } private set { _notiNightEnable = value; } }
//         public string SubscribedTopic { get { return _subscribedTopic; } private set { _subscribedTopic = value; } }
//         public string RealSubscribedTopic { get { return _realSubscribedTopic; } private set { _realSubscribedTopic = value; } }

//         public string ChallengeDifficultyData { get; private set; } = ""; // 인코딩 / 디코딩 해서 사용.

//         public bool IsUseCheatPurchase { get; private set; } = false;

//         public bool IsUseCheatWorldChallengeBounty { get; private set; } = false;

//         //public bool IsRepeatMode { get; private set; } = false;

//         public int JapanAgeType { get; private set; } = (int)EJapanAgeType.NONE;
//         public string SelectJapanAgeTimestampeStr { get; private set; } = "";
//         //public string LastShowMatchResetPopupTimestampStr { get; private set; } = "";

//         public bool IsValidUSAAge { get; private set; } = false;

//         public string LastShowNoticePopupTimestampStr { get; private set; } = "";

//         public string LastShowSeasonEndWarningPopupTimeStampStr { get; private set; } = "";

//         //리뷰 조건중 등급 조건 단계
//         public int CurReviewGradeCondStep { get; private set; } = 0;
//         //리뷰 조건 중 스테이지 조건 단계
//         public int CurReviewStageCondStep { get; private set; } = 0;
//         //리뷰 조건중 마지막으로 보여준 등급 조건 단계
//         public int LastShowReviewGradeCondStep { get; private set; } = 0;
//         //리뷰 조건 중 스테이지 조건 단계
//         public int LastShowReviewStageCondStep { get; private set; } = 0;

//         public string LastShowTombElement { get; private set; } = "WATER";
//         public int LastShowTombLevel { get; private set; } = 1;

//         public string LatestPatchUrl { get; private set; } = "";

//         public bool IsAlreadyShowCollaboFrierenPV { get; private set; } = false;
//         public string LastShowShopHeroTabWishListWanringPopupTimestampStr { get; private set; }
//         public int PrevMaxLv { get; private set; } = 50;

//         public string Name => throw new NotImplementedException();

//         public string Type => throw new NotImplementedException();

//         public bool Writable => throw new NotImplementedException();

//         public object Default => throw new NotImplementedException();

//         // 임시 사용 멤버변수 ------------------------------------------------------
//         // Volume...
//         private float _bgmVolume = 1.0f;
//         private float _effVolume = 1.0f;


//         // 탭에서 표시되는 New 마크 List
//         private List<int> _newEquipItemList = new();
//         private List<int> _newGachaItemList = new();

//         public UnityEvent<float> onEffVolume = new();
//         public UnityEvent<float> onBgmVolume = new();

//         public List<int> _invalidGuideGroupNumList = new();

//         public Dictionary<string, DateTime> _lastShowSeasonEndWarningTimeDict = new();

//         // 기타 게임 옵션.
//         private bool _isUseVibrate = false;
//         private bool _isUseAlarm = false;
//         private bool _isUseSleep = false;
//         private bool _isUseAutoSkill = true;
//         private bool _isLastUseMaximumChatSize = false;
//         private bool _isUseAutoScreenSaver = true;

//         private int _cameraType = 1;
//         private int _chatSizeMode = 0;

//         private int _targetFrameRateLv = 1;
//         private int _resolutionScaleLv = 1;
//         private bool _isAutoResolution = false;

//         //private bool _notiDayEnable = false;
//         //private bool _notiNightEnable = false;
//         private string _subscribedTopic = "";
//         private string _realSubscribedTopic = "";


//         public UnityEventWrapper onChangeChallengeDifficulty = new();

//         //-----------------------------------------------------------------------------------------------------
//         private Dictionary<string, string> _diskPrefsDict = new();
//         private Dictionary<string, string> _changedPrefsDict = new();

//         public LocalConfig()
//         {
//             Clear();
//         }

//         private void Clear()    // 초기값.
//         {
//             this.AccountEnv = string.Empty;
//             this.ChannelUid = string.Empty;
//             this.Token = string.Empty;
//             this.IsAlreadyUsePrivacyPopup = false;
//             this.LastShowGameOverDateTime = string.Empty;
//             this.Language = string.Empty;
//             this.MatchBlockListString = "[]";
//             this.GameOpenCnt = 0;
//             this.CurMailChecksum = string.Empty;
//             this.SelectedCostumeNum = -1;

//             this.BgmVolume = 1.0f;
//             this.EffVolume = 1.0f;

//             this.IsUseAlarm = false;
//             this.IsUseVibrate = false;
//             this.IsUseSleep = false;
//             this.IsUseAutoSkill = true;

//             this.CameraType = 1;

//             this.IsShowHaveHeroInEquipHeroTab = true;
//             this.IsShowHaveItemInEquipArtifactTab = true;
//             this.IsShowHaveItemInEquipPetTab = true;
//             this.IsShowGoldGrowthDialog = false;
//             this.IsShowGachaHeroDialog = false;
//             this.IsShowEquipHeroDialog = false;
//             this.IsShowFirstBreakDialog = false;
//             this.IsShowCashDungeonDialog = false;

//             this.NewEquipItemNumText = "";
//             this.InvalidGuideGruopNumText = "";
//             this.IsAlreadyShowTutorialList = "";

//             this.TargetFrameRateLv = 1;
//             this.ResolutionScaleLv = 1;
//             this.IsAutoResolution = false;

//             //this.NotiDayEnable = false;
//             //this.NotiNightEnable = false;
//             this.SubscribedTopic = "";
//             this.RealSubscribedTopic = "";

//             this.IsUseCheatPurchase = false;
//             //this.IsRepeatMode = false;

//             this.IsGoogleConnected = false;
//             this.IsAppleConnected = false;

//             this.ChallengeDifficultyData = "";

//             this.JapanAgeType = (int)EJapanAgeType.NONE;
//             //this.SelectJapanAgeTimestampeStr = DateTime.MinValue.ToString();
//             this.SelectJapanAgeTimestampeStr = "";

//             this.IsValidUSAAge = false;
//             //this.USAAge = 0;

//             this.CurReviewGradeCondStep = 0;
//             this.CurReviewStageCondStep = 0;
//             this.LastShowReviewGradeCondStep = 0;
//             this.LastShowReviewStageCondStep = 0;

//             this.LatestPatchUrl = "";
//             //this.LastShowMatchResetPopupTimestampStr = "";
//             this.LastShowSeasonEndWarningPopupTimeStampStr = "";
//             this.LatestPatchUrl = "";

//             this.IsAlreadyShowCollaboFrierenPV = false;
//             this.LastShowNoticePopupTimestampStr = "";
//             this.LastShowShopHeroTabWishListWanringPopupTimestampStr = "";

//             this.IsUseCheatWorldChallengeBounty = false;
//             this.PrevMaxLv = APP.Cfg.Content.HeroLimitLv;
//         }


//         public void DeleteData()
//         {
//             //언어는 저장해두었다가 다시 셋팅
//             var befLang = this.Language;

//             PropertyInfo[] localPropertyArr = typeof(LocalConfig).GetProperties();

//             foreach (PropertyInfo propInfo in localPropertyArr)
//                 PlayerPrefs.DeleteKey(propInfo.Name);

//             Clear();
//             SetLanguage(befLang);
//         }

//         public bool Create()
//         {
//             return Reload();
//         }

//         public bool Reload()
//         {
//             if (!TryLoadPlayerPrefsAll())
//                 return false;

//             PostLoad();
//             return true;
//         }

//         public bool TryLoadPlayerPrefsAll()
//         {
//             PropertyInfo[] localPropertyArr = typeof(LocalConfig).GetProperties();

//             foreach (PropertyInfo propInfo in localPropertyArr)
//             {
//                 Type propInfoType = propInfo.PropertyType;

//                 if (!PlayerPrefs.HasKey(propInfo.Name))
//                     continue;

//                 if (propInfoType == typeof(int))
//                 {
//                     TryLoadIntPrefs(propInfo);
//                 }
//                 else if (propInfoType == typeof(float))
//                 {
//                     TryLoadFloatPrefs(propInfo);
//                 }
//                 else if (propInfoType == typeof(string))
//                 {
//                     TryLoadStringPrefs(propInfo);
//                 }
//                 else if (propInfoType == typeof(bool))
//                 {
//                     TryLoadBoolPrefs(propInfo);
//                 }
//                 else
//                 {
//                     LOG.E($"No Handling PropInfoType({propInfo.PropertyType}), PropInfoName({propInfo.Name})");
//                 }
//             }
//             return true;
//         }


//         private void PostLoad()
//         {
//             //this.Language = "";

//             if (this.Language == string.Empty)  // 언어
//             {
//                 var systemLanguage = Application.systemLanguage;

//                 if (systemLanguage == SystemLanguage.ChineseTraditional)
//                     this.Language = ELanguage.CHINESE_TRADITIONAL.ToString();
//                 else if (systemLanguage == SystemLanguage.ChineseSimplified)
//                     this.Language = ELanguage.CHINESE_SIMPLIFIED.ToString();
//                 else if (systemLanguage == SystemLanguage.Chinese)
//                     this.Language = ELanguage.CHINESE_SIMPLIFIED.ToString();
//                 else
//                 {
//                     UTIL.TryStrToEnum(out ELanguage outT, systemLanguage.ToString().ToUpperInvariant(), ELanguage.ENGLISH, false);
//                     this.Language = outT.ToString();
//                 }
//             }

//             if (this.VoiceLanguage == string.Empty) // 음성
//             {
//                 // 음성 디폴트 값
//                 ELanguage voiceLangType;
//                 if (string.IsNullOrEmpty(APP.Cfg.App.VoiceDefaultLanguage))
//                 {
//                     // APP.Cfg.App.VoiceDefaultLanguage가 빈 값일 경우의 디폴트값
//                     voiceLangType = ELanguage.JAPANESE;
//                 }
//                 else
//                 {
//                     voiceLangType = UTIL.GetVoiceLanguageType(APP.Cfg.App.VoiceDefaultLanguage);
//                 }

//                 APP.Cfg.Local.SetVoiceLanguage(voiceLangType);
//             }

// #if NP_DEBUG || UNITY_EDITOR
//             SetIsUseCheatPurchase(true);
// #else
//             SetIsUseCheatPurchase(false);
// #endif

//             ParsingNewitemNumText();
//             ParsingInvalidGuideGroupNumList();
//             ParsingLastShowSeasonEndWarningPopupTime();
//         }

//         public void ClearChannelUid()
//         {
//             this.AccountEnv = string.Empty;
//             this.ChannelUid = string.Empty;
//             SetStringPrefs(nameof(this.AccountEnv), string.Empty);
//             SetStringPrefs(nameof(this.ChannelUid), string.Empty);
//         }

//         public void SetChannelUid(string channelUid)
//         {
//             if (string.IsNullOrEmpty(channelUid))
//             {
//                 LOG.E("Set Channel Uid Failed. Value is Null or Empty");
//                 return;
//             }

//             this.ChannelUid = channelUid;
//             SetStringPrefs(nameof(this.ChannelUid), channelUid);
//         }

//         public void SetToken(string token)
//         {
//             if (string.IsNullOrEmpty(token))
//             {
//                 //LOG.E("Set Token Failed. Value is Null or Empty");
//                 return;
//             }

//             this.Token = token;
//             SetStringPrefs(nameof(this.Token), token);
//         }

//         public void SetGoogleConnected(bool connected)
//         {
//             IsGoogleConnected = connected;
//         }

//         public void SetAppleConnected(bool connected)
//         {
//             IsAppleConnected = connected;
//         }
//         public void SetSelectedCostumeNum(int num)
//         {
//             this.SelectedCostumeNum = num;
//             // PlayerPrefs에 굳이 저장할 필요 없어서 안함
//         }

//         public void SetAccountEnv(string accountEnv)
//         {
//             if (string.IsNullOrEmpty(accountEnv))
//             {
//                 LOG.W("Set AccountEnv Failed. Value is Null or Empty");
//                 accountEnv = string.Empty;
//             }

//             this.AccountEnv = accountEnv;
//             SetStringPrefs(nameof(this.AccountEnv), accountEnv);
//         }


//         public void SetAlreadyUsePrivacyPopup(bool isAlreadyUsePopup)
//         {
//             this.IsAlreadyUsePrivacyPopup = isAlreadyUsePopup;
//             SetBoolPrefs(nameof(this.IsAlreadyUsePrivacyPopup), isAlreadyUsePopup);
//         }

//         public void SetGameOpenCnt(int gameOpenCnt)
//         {
//             this.GameOpenCnt = gameOpenCnt;
//             SetIntPrefs(nameof(this.GameOpenCnt), gameOpenCnt);
//         }

//         public bool HasChannelUid()
//         {
//             return !UTIL.IsNullOrEmpty(ChannelUid);
//         }

//         public void SetLanguage(ELanguage lang)
//         {
//             this.Language = UTIL.EnumToStr(lang);
//             SetStringPrefs(nameof(this.Language), this.Language);
//         }

//         private void SetLanguage(string lang)
//         {
//             this.Language = lang;
//             SetStringPrefs(nameof(this.Language), this.Language);
//         }

//         public void SetVoiceLanguage(ELanguage lang)
//         {
//             this.VoiceLanguage = UTIL.EnumToStr(lang);
//             SetStringPrefs(nameof(this.VoiceLanguage), this.VoiceLanguage);
//         }

//         public ELanguage GetVoiceLanguage()
//         {
//             return UTIL.StrToEnum(this.VoiceLanguage, ELanguage.JAPANESE);
//         }

//         public void SetBlockList(List<int> blockList)
//         {
//             this.MatchBlockListString = JsonConvert.SerializeObject(blockList);
//             SetStringPrefs(nameof(this.MatchBlockListString), this.MatchBlockListString);
//         }

//         public List<int> GetBlockList()
//         {
//             if (string.IsNullOrEmpty(this.MatchBlockListString))
//                 return new List<int>();

//             return JsonConvert.DeserializeObject<List<int>>(this.MatchBlockListString);
//         }

//         public void SetIsAlreadyShowTutorialList(List<bool> isAlreadyShowTutorialList)
//         {
//             this.IsAlreadyShowTutorialList = JsonConvert.SerializeObject(isAlreadyShowTutorialList);
//             SetStringPrefs(nameof(this.IsAlreadyShowTutorialList), this.IsAlreadyShowTutorialList);
//         }

//         public List<bool> GetIsAlreadyShowTutorialList()
//         {
//             if (string.IsNullOrEmpty(this.IsAlreadyShowTutorialList))
//                 return Enumerable.Repeat(false, (int)ETutorialType.MAX_NUM).ToList();
//             return JsonConvert.DeserializeObject<List<bool>>(this.IsAlreadyShowTutorialList);
//         }

//         private void _SetFloatOption(ref float refValue, float newValue, UnityEvent<float> evt, string prefsKey)
//         {
//             if (refValue == newValue)
//                 return;

//             refValue = newValue;
//             SetFloatPrefs(prefsKey, newValue);

//             evt.Invoke(newValue);
//         }

//         public void SetBgmVolume(float vol) => _SetFloatOption(ref _bgmVolume, vol, onBgmVolume, nameof(this.BgmVolume));
//         public void SetEffVolume(float vol) => _SetFloatOption(ref _effVolume, vol, onEffVolume, nameof(this.EffVolume));
//         public void SetUseAutoScreenSaver(bool value) => _SetBoolOption(ref _isUseAutoScreenSaver, value, nameof(this.IsUseAutoScreenSaver));
//         //public bool ToggleLockSavePanel() => _ToggleBoolOption(ref _isLockSavePanel, nameof(IsLockSavePanel));
//         public bool ToggleUseVibrate() => _ToggleBoolOption(ref _isUseVibrate, nameof(IsUseVibrate));
//         public bool ToggleUseSleep() => _ToggleBoolOption(ref _isUseSleep, nameof(IsUseSleep));
//         public bool ToggleUseAlarm() => _ToggleBoolOption(ref _isUseAlarm, nameof(IsUseAlarm));
//         public void ToggleAutoSkill() => _ToggleBoolOption(ref _isUseAutoSkill, nameof(IsUseAutoSkill));

//         public void SetCameraType(int type) => _SetIntOption(ref _cameraType, type, nameof(CameraType));


//         public void SetIsLastUseMaximumChatSize(bool isMaximum)
//         {
//             this.IsLastUseMaximumChatSize = isMaximum;
//             SetBoolPrefs(nameof(this.IsLastUseMaximumChatSize), isMaximum);
//         }

//         public void SetChatSizeMode(int type)
//         {
//             _SetIntOption(ref _chatSizeMode, type, nameof(ChatSizeMode));
//         }

//         public void SetMyActorNum(int num)
//         {
//             this.LeaderActorNum = num;
//             SetIntPrefs(nameof(this.LeaderActorNum), this.LeaderActorNum);
//         }

//         public void SetIsShowHaveHeroInEquipHeroTab(bool isActive)
//         {
//             this.IsShowHaveHeroInEquipHeroTab = isActive;
//             SetBoolPrefs(nameof(IsShowHaveHeroInEquipHeroTab), this.IsShowHaveHeroInEquipHeroTab);
//         }

//         public void SetIsShowHaveItemInEquipArtifactTab(bool isActive)
//         {
//             this.IsShowHaveItemInEquipArtifactTab = isActive;
//             SetBoolPrefs(nameof(IsShowHaveItemInEquipArtifactTab), this.IsShowHaveItemInEquipArtifactTab);
//         }

//         public void SetIsShowHaveItemInEquipPetTab(bool isActive)
//         {
//             this.IsShowHaveItemInEquipPetTab = isActive;
//             SetBoolPrefs(nameof(IsShowHaveItemInEquipPetTab), this.IsShowHaveItemInEquipPetTab);
//         }

//         public void SetTrueIsShowGoldGrowthDialog()
//         {
//             this.IsShowGoldGrowthDialog = true;
//             SetBoolPrefs(nameof(IsShowGoldGrowthDialog), true);
//         }

//         public void SetTrueIsShowGachaHeroDialog()
//         {
//             this.IsShowGachaHeroDialog = true;
//             SetBoolPrefs(nameof(IsShowGachaHeroDialog), true);
//         }

//         public void SetTrueIsShowEquipHeroDialog()
//         {
//             this.IsShowEquipHeroDialog = true;
//             SetBoolPrefs(nameof(IsShowEquipHeroDialog), true);
//         }

//         public void SetTrueIsShowFirstBreakDialog()
//         {
//             this.IsShowFirstBreakDialog = true;
//             SetBoolPrefs(nameof(IsShowFirstBreakDialog), true);
//         }

//         public void SetTrueIsShowCashDungeonDialog()
//         {
//             this.IsShowCashDungeonDialog = true;
//             SetBoolPrefs(nameof(IsShowCashDungeonDialog), true);
//         }

//         public void SetTargetFrameRateLv(int lv)
//         {
//             this.TargetFrameRateLv = lv;
//             SetIntPrefs(nameof(this.TargetFrameRateLv), lv);
//         }
//         public bool HasTargetFrameRateLv()
//         {
//             return PlayerPrefs.HasKey(nameof(this.TargetFrameRateLv));
//         }
//         public void SetResolutionScaleLv(int lv)
//         {
//             this.ResolutionScaleLv = lv;
//             SetIntPrefs(nameof(this.ResolutionScaleLv), lv);
//         }
//         public bool HasResolutionScale()
//         {
//             var hasKey = PlayerPrefs.HasKey(nameof(this.ResolutionScaleLv)) || PlayerPrefs.HasKey(nameof(this.IsAutoResolution));
//             return hasKey;
//         }
//         public void SetAutoResolution(bool isAuto)
//         {
//             this.IsAutoResolution = isAuto;
//             SetBoolPrefs(nameof(this.IsAutoResolution), isAuto);
//         }

//         public void SetIsUseCheatPurchase(bool isUse)
//         {
//             this.IsUseCheatPurchase = isUse;
//             SetBoolPrefs(nameof(this.IsUseCheatPurchase), isUse);
//         }

//         public void SetIsUseCheatWorldChallengeBounty(bool isUse)
//         {
//             this.IsUseCheatWorldChallengeBounty = isUse;
//             SetBoolPrefs(nameof(this.IsUseCheatWorldChallengeBounty), isUse);
//         }

//         //public void SetIsRepeatMode(bool isRepeatMode)
//         //{
//         //    this.IsRepeatMode = isRepeatMode;
//         //    SetBoolPrefs(nameof(this.IsRepeatMode), isRepeatMode);
//         //}

//         //public void SetNotiDayEnable(bool enable)
//         //{
//         //    this.NotiDayEnable = enable;
//         //    SetBoolPrefs(nameof(this.NotiDayEnable), enable);
//         //}

//         //public void SetNotiNightEnable(bool enable)
//         //{
//         //    this.NotiNightEnable = enable;
//         //    SetBoolPrefs(nameof(this.NotiNightEnable), enable);
//         //}

//         private bool _SetBoolOption(ref bool refValue, bool newValue, string prefsKey)
//         {
//             refValue = newValue;
//             SetBoolPrefs(prefsKey, refValue);

//             return refValue;
//         }

//         private bool _ToggleBoolOption(ref bool refValue, string prefsKey, UnityEvent evt = null)
//         {
//             refValue = !refValue;
//             SetBoolPrefs(prefsKey, refValue);
//             evt?.Invoke();

//             return refValue;
//         }

//         private void _SetIntOption(ref int refValue, int newValue, string prefsKey)
//         {
//             if (refValue == newValue)
//                 return;

//             refValue = newValue;
//             SetIntPrefs(prefsKey, newValue);
//         }


//         public void SetMailChecksum(string mailChecksum)
//         {
//             LOG.I($"Invoke SetLocal ({mailChecksum})");
//             this.CurMailChecksum = mailChecksum;
//             SetStringPrefs(nameof(CurMailChecksum), mailChecksum);
//         }

//         public void SetFCMTopicList(List<string> topicList)
//         {
//             this.SubscribedTopic = string.Join(";", topicList);
//             SetStringPrefs(nameof(this.SubscribedTopic), this.SubscribedTopic);
//         }

//         public void SetRealFCMTopicList(List<string> topicList)
//         {
//             this.RealSubscribedTopic = string.Join(";", topicList);
//             SetStringPrefs(nameof(this.RealSubscribedTopic), this.RealSubscribedTopic);
//         }

//         public void ResetRealFCMTopicList()
//         {
//             this.RealSubscribedTopic = string.Empty;
//             SetStringPrefs(nameof(this.RealSubscribedTopic), this.RealSubscribedTopic);
//         }

//         public List<string> GetFCMTopicList()
//         {
//             if (string.IsNullOrEmpty(this.SubscribedTopic))
//                 return new List<string>();

//             return this.SubscribedTopic.Split(';').ToList();
//         }
//         public List<string> GetRealFCMTopicList()
//         {
//             if (string.IsNullOrEmpty(this.RealSubscribedTopic))
//                 return new List<string>();

//             return this.RealSubscribedTopic.Split(';').ToList();
//         }

//         public void SetJapanAgeType(EJapanAgeType ageType, DateTime selectTime)
//         {
//             this.JapanAgeType = (int)ageType;
//             this.SelectJapanAgeTimestampeStr = UTIL.DateTimeToStr(selectTime);
//             //this.SelectJapanAgeTimestampeStr = selectTime.ToString(c_timeStampFmt);

//             SetIntPrefs(nameof(this.JapanAgeType), (int)ageType);
//             SetStringPrefs(nameof(this.SelectJapanAgeTimestampeStr), this.SelectJapanAgeTimestampeStr);
//             //SetStringPrefs(nameof(this.SelectJapanAgeTimestampeStr), selectTime.ToString(c_timeStampFmt));
//         }

//         public EJapanAgeType GetJapanAgeType()
//         {
//             return UTIL.IntToEnum<EJapanAgeType>(this.JapanAgeType, EJapanAgeType.NONE);
//         }

//         public DateTime GetSelectJapanAgeTimestamp()
//         {
//             return UTIL.StrToDateTime(this.SelectJapanAgeTimestampeStr, false);
//         }

//         public void SetIsValidUSAAge(bool isValid)
//         {
//             this.IsValidUSAAge = isValid;
//             SetBoolPrefs(nameof(this.IsValidUSAAge), isValid);
//         }

//         public void SetCurReviewGradeCondStep(int step)
//         {
//             this.CurReviewGradeCondStep = step;
//             SetIntPrefs(nameof(this.CurReviewGradeCondStep), step);
//         }

//         public void SetCurReviewStageCondStep(int step)
//         {
//             this.CurReviewStageCondStep = step;
//             SetIntPrefs(nameof(this.CurReviewStageCondStep), step);
//         }

//         public void SetLastShowReviewGradeCondStep(int step)
//         {
//             this.LastShowReviewGradeCondStep = step;
//             SetIntPrefs(nameof(this.LastShowReviewGradeCondStep), step);
//         }

//         public void SetLastShowReviewStageCondStep(int step)
//         {
//             this.LastShowReviewStageCondStep = step;
//             SetIntPrefs(nameof(this.LastShowReviewStageCondStep), step);
//         }

//         public void SetLatestPatchUrl(string url)
//         {
//             this.LatestPatchUrl = url;
//             SetStringPrefs(nameof(this.LatestPatchUrl), url);
//         }

//         //public void SetLastShowMatchResetPopup(DateTime time)
//         //{
//         //    this.LastShowMatchResetPopupTimestampStr = UTIL.DateTimeToStr(time);
//         //    SetStringPrefs(nameof(this.LastShowMatchResetPopupTimestampStr), this.LastShowMatchResetPopupTimestampStr);

//         //    //this.LastShowMatchResetPopupTimestampStr = time.ToString(c_timeStampFmt);
//         //    //SetStringPrefs(nameof(this.LastShowMatchResetPopupTimestampStr), time.ToString(c_timeStampFmt));
//         //}

//         public void SetIsAlreadyShowCollaboFrierenPV(bool isShow)
//         {
//             this.IsAlreadyShowCollaboFrierenPV = isShow;
//             SetBoolPrefs(nameof(this.IsAlreadyShowCollaboFrierenPV), isShow);
//         }



//         //public DateTime GetLastShowMatchResetPopup() => UTIL.StrToDateTime(this.LastShowMatchResetPopupTimestampStr, false);
//         /*
//         {
//             if (UTIL.TryParseDateTimeStr(out DateTime dateTime, this.LastShowMatchResetPopupTimestampStr))
//                 return dateTime;

//             return DateTime.MinValue;
//         }
//         */


//         //public void SetUSAAge(int age)
//         //{
//         //    this.USAAge = age;
//         //    SetIntPrefs(nameof(this.USAAge), age);
//         //}

//         private void ParsingNewitemNumText()
//         {
//             _newEquipItemList = this.NewEquipItemNumText.Split(';')
//                 .Where(x => !string.IsNullOrEmpty(x)) // 필요에 따라 빈 문자열을 제외할 수 있습니다.
//                 .Select(x => int.Parse(x))
//                 .ToList();
//         }

//         // yyyy-MM-dd HH:mm:ss
//         private void ParsingLastShowSeasonEndWarningPopupTime()
//         {
//             List<string> strArr = UTIL.SplitText(this.LastShowSeasonEndWarningPopupTimeStampStr, ";");

//             for (int idx = 0; idx < strArr.Count; idx++)
//             {
//                 string stampStr = strArr[idx];
//                 int splitIndex = stampStr.IndexOf('=');
//                 if (splitIndex == -1)
//                 {
//                     LOG.E($"invalid LastShowSeasoneEndWarningPopup format({stampStr})");
//                     continue;
//                 }

//                 if (!UTIL.TryStrToDateTime(out DateTime lastDateTime, stampStr.Substring(splitIndex + 1)))
//                     continue;

//                 string evtType = stampStr.Substring(0, splitIndex);

//                 if (!_lastShowSeasonEndWarningTimeDict.TryAdd(evtType, lastDateTime))
//                     _lastShowSeasonEndWarningTimeDict[evtType] = lastDateTime;
//             }
//         }


//         public void PushLastShowSeasonEndWarningPopupTime(EventInfo evtInfo, DateTime time)
//         {
//             if (!_lastShowSeasonEndWarningTimeDict.TryAdd(evtInfo.ContentType.ToString(), time))
//                 _lastShowSeasonEndWarningTimeDict[evtInfo.ContentType.ToString()] = time;

//             //LOG.W("[DEBUG_SEASON_END_WARING_SET]" + string.Join(";", _lastShowSeasonEndWarningTimeDict.Select(kvp => $"{kvp.Key}_{UTIL.DateTimeToStr(kvp.Value)}")));
//             SetStringPrefs(nameof(LastShowSeasonEndWarningPopupTimeStampStr), string.Join(";", _lastShowSeasonEndWarningTimeDict.Select(kvp => $"{kvp.Key}={UTIL.DateTimeToStr(kvp.Value)}")));
//         }

//         public bool IsNeedShowSeasonEndWarningPopup(int remainSec, EventInfo evtInfo)
//         {
//             if (remainSec <= 0)  // 이미 끝남
//                 return false;

//             if (remainSec > (int)TimeSpan.FromMinutes(APP.Cfg.Game.GameShowSeasonWaringMin).TotalSeconds)
//                 return false;

//             if (_lastShowSeasonEndWarningTimeDict.TryGetValue(evtInfo.ContentType.ToString(), out DateTime time))
//             {
//                 // 시간이 지금 이후
//                 if (time >= APP.Ctx.CurServerTimeStamp)
//                     return false;

//                 return time.AddMinutes(APP.Cfg.Game.GameIgnoreSeasonWaringMin) <= APP.Ctx.CurServerTimeStamp;
//             }

//             return true;
//         }

//         public bool TryPopNewEquipItemNum(int itemNum)
//         {
//             if (!_newEquipItemList.Remove(itemNum))
//                 return false;

//             SetStringPrefs("NewEquipItemNumText", string.Join(";", _newEquipItemList.Select(n => n.ToString())));
//             return true;
//         }

//         public bool TryPopNewGachaItemNum(int itemNum)
//         {
//             if (!_newGachaItemList.Remove(itemNum))
//                 return false;

//             //SetStringPrefs("NewGachaItemList", string.Join(";", _newGachaItemList.Select(n => n.ToString())));
//             return true;
//         }

//         public bool IsInNewEquipItemNum(int itemNum)
//         {
//             return _newEquipItemList.Contains(itemNum);
//         }

//         public bool IsInNewGachaItemNum(int itemNum)
//         {
//             return _newGachaItemList.Contains(itemNum);
//         }

//         public void PushNewItemNum(int itemNum)
//         {
//             if (_newEquipItemList.Contains(itemNum))
//                 return;

//             if (!GAME.IsValidGameUI())
//                 return;

//             _newEquipItemList.Add(itemNum);
//             _newGachaItemList.Add(itemNum);

//             SetStringPrefs("NewEquipItemNumText", string.Join(";", _newEquipItemList.Select(n => n.ToString())));
//         }


//         private void ParsingInvalidGuideGroupNumList()
//         {
//             _invalidGuideGroupNumList = this.InvalidGuideGruopNumText.Split(';')
//                 .Where(x => !string.IsNullOrEmpty(x))
//                 .Select(x => int.Parse(x))
//                 .ToList();
//         }

//         //public bool TryPopInvalidGuideGroupNum(int itemNum)
//         //{
//         //    if (!_invalidGuideGroupNumList.Remove(itemNum))
//         //        return false;

//         //    SetStringPrefs("InvalidGuideGruopNumText", string.Join(";", _invalidGuideGroupNumList.Select(n => n.ToString())));
//         //    return true;
//         //}


//         public void PushInvalidGuideGroupNum(int itemNum)
//         {
//             if (_invalidGuideGroupNumList.Contains(itemNum))
//                 return;

//             if (!GAME.IsValidGameUI())
//                 return;

//             _invalidGuideGroupNumList.Add(itemNum);

//             SetStringPrefs("InvalidGuideGruopNumText", string.Join(";", _invalidGuideGroupNumList.Select(n => n.ToString())));
//         }

//         public bool IsInInvalidGuideGroupNum(int itemNum)
//         {
//             return _invalidGuideGroupNumList.Contains(itemNum);
//         }

//         public bool CheckGameOverGuideTime()
//         {

//             DateTime curDateTime = APP.Ctx.CurServerTimeStamp;
//             DateTime befDateTime = DateTime.MinValue;

//             if (!UTIL.IsEmptyStr(this.LastShowGameOverDateTime))
//             {
//                 befDateTime = UTIL.StrToDateTime(this.LastShowGameOverDateTime);
//                 //befDateTime = DateTime.ParseExact(this.LastShowGameOverDateTime, c_timeStampFmt, null);
//             }

//             TimeSpan timePassed = curDateTime - befDateTime;

//             if (timePassed < TimeSpan.FromSeconds(APP.Cfg.Game.ShowGameOverDurTime))
//             {
//                 return false;
//             }

//             return true;
//         }

//         public void ResetGameOverGuideTime()
//         {
//             this.LastShowGameOverDateTime = string.Empty;
//         }


//         public void SaveGameOverGuideLastShowTime()
//         {
//             DateTime curDateTime = APP.Ctx.CurServerTimeStamp;
//             this.LastShowGameOverDateTime = UTIL.DateTimeToStr(curDateTime);

//             //this.LastShowGameOverDateTime = curDateTime.ToString(c_timeStampFmt);
//         }

//         public bool CanUseGameNoticePopup()
//         {
//             if (!APP.Cfg.App.CanUseGameNotice())
//                 return false;

//             if (this.LastShowNoticePopupTimestampStr.Length <= 0)   // 처음 보는것.
//                 return true;

//             DateTime dt = UTIL.StrToDateTime(this.LastShowNoticePopupTimestampStr, true);

//             //if (UTIL.TryParseDateTimeStr(out DateTime dt, this.LastShowNoticePopupTimestampStr) == false)
//             //    return true;    // 시간값이 이상하거나 없으면 봐야해!

//             if (dt >= UTIL.GetUTCNowTime())   // 시간이 지금보다 이후면 안봐!
//                 return false;

//             dt = dt.AddMinutes(APP.Cfg.Game.GameNoticeIgnoreMin);
//             return UTIL.GetUTCNowTime() >= dt;
//         }

//         public void SetIgnoreGameNotice()
//         {
//             this.LastShowNoticePopupTimestampStr = UTIL.DateTimeToStr(APP.Ctx.CurServerTimeStamp);
//             SetStringPrefs(nameof(this.LastShowNoticePopupTimestampStr), this.LastShowNoticePopupTimestampStr);
//         }

//         public void SetLastShowTombInfo(string element, int lv)
//         {
//             this.LastShowTombLevel = lv;
//             this.LastShowTombElement = element;
//             SetIntPrefs(nameof(this.LastShowTombLevel), this.LastShowTombLevel);
//             SetStringPrefs(nameof(this.LastShowTombElement), this.LastShowTombElement);
//         }

//         public bool CanShowShopHeroTabWishListWanringPopup()
//         {
//             if (this.LastShowShopHeroTabWishListWanringPopupTimestampStr.Length <= 0)   // 처음 보는것.
//                 return true;

//             DateTime dt = UTIL.StrToDateTime(this.LastShowShopHeroTabWishListWanringPopupTimestampStr, false);
//             return dt.Date != APP.Ctx.CurServerTimeStamp.Date;
//         }

//         public void ResetLastShowShopHeroTabWishListWanringPopupTimestampStr()
//         {
//             SetLastShowShopHeroTabWishListWanringPopupTimestampStr(DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc));
//         }

//         public void SetLastShowShopHeroTabWishListWanringPopupTimestampStr(DateTime dateTime)
//         {
//             this.LastShowShopHeroTabWishListWanringPopupTimestampStr = UTIL.DateTimeToStr(dateTime);
//             SetStringPrefs(nameof(this.LastShowShopHeroTabWishListWanringPopupTimestampStr), this.LastShowShopHeroTabWishListWanringPopupTimestampStr);
//         }

//         public void SetPrevMaxLv(int prevMaxLv)
//         {
//             this.PrevMaxLv = prevMaxLv;
//             SetIntPrefs(nameof(this.PrevMaxLv), this.PrevMaxLv);
//         }

//         public bool IsNeedShowExtendMaxLvNotify(int curMaxLv)
//         {
//             return this.PrevMaxLv != curMaxLv;
//         }




















//         // ============================================================================================


//         private bool TryLoadStringPrefs(PropertyInfo prop)
//         {
//             string val = string.Empty;
//             try
//             {
//                 val = PlayerPrefs.GetString(prop.Name);
//                 prop.SetValue(this, val);
//                 _diskPrefsDict[prop.Name] = val;
//             }
//             catch (Exception exc)
//             {
//                 LOG.E($"Load String PlayerPrefs Failed. PropName({prop.Name}), Message({exc.Message})");
//                 return false;
//             }

//             return true;
//         }

//         private bool TryLoadIntPrefs(PropertyInfo prop)
//         {
//             int val = 0;
//             try
//             {
//                 val = PlayerPrefs.GetInt(prop.Name);
//                 prop.SetValue(this, val);
//                 _diskPrefsDict[prop.Name] = val.ToString();
//             }
//             catch (Exception exc)
//             {
//                 LOG.E($"Load Int PlayerPrefs Failed. PropName({prop.Name}), Message({exc.Message})");
//                 return false;
//             }

//             return true;
//         }

//         private bool TryLoadFloatPrefs(PropertyInfo prop)
//         {
//             float val = 0;
//             try
//             {
//                 val = PlayerPrefs.GetFloat(prop.Name);
//                 prop.SetValue(this, val);
//                 _diskPrefsDict[prop.Name] = val.ToString();
//             }
//             catch (Exception exc)
//             {
//                 LOG.E($"Load Float PlayerPrefs Failed. PropName({prop.Name}), Message({exc.Message})");
//                 return false;
//             }

//             return true;
//         }
//         private bool TryLoadBoolPrefs(PropertyInfo prop)
//         {
//             bool val = false;
//             try
//             {
//                 string strBool = PlayerPrefs.GetString(prop.Name);
//                 val = UTIL.StrToBool(strBool);
//                 prop.SetValue(this, val);
//                 _diskPrefsDict[prop.Name] = val.ToString();
//             }
//             catch (Exception exc)
//             {
//                 LOG.E($"Load Bool PlayerPrefs Failed. PropName({prop.Name}), Message({exc.Message})");
//                 return false;
//             }

//             return true;
//         }

//         private void SetStringPrefs(string key, string value)
//         {
//             try
//             {
//                 PlayerPrefs.SetString(key, value);
//                 OnChangedPrefs(key, value.ToString());
//             }
//             catch (Exception exc)
//             {
//                 LOG.E($"Set String PlayerPrefs Failed. Key({key}), Value({value}), Message({exc.Message})");
//             }
//         }

//         private void SetIntPrefs(string key, int value)
//         {
//             try
//             {
//                 PlayerPrefs.SetInt(key, value);
//                 OnChangedPrefs(key, value.ToString());
//             }
//             catch (Exception exc)
//             {
//                 LOG.E($"Set Int PlayerPrefs Failed. Key({key}), Value({value}), Message({exc.Message})");
//             }
//         }
//         private void SetFloatPrefs(string key, float value)
//         {
//             try
//             {
//                 PlayerPrefs.SetFloat(key, value);
//                 OnChangedPrefs(key, value.ToString());
//             }
//             catch (Exception exc)
//             {
//                 LOG.E($"Set Float PlayerPrefs Failed. Key({key}), Value({value}), Message({exc.Message})");
//             }
//         }
//         private void SetBoolPrefs(string key, bool value)
//         {
//             try
//             {
//                 PlayerPrefs.SetString(key, value.ToString());
//                 OnChangedPrefs(key, value.ToString());
//             }
//             catch (Exception exc)
//             {
//                 LOG.E($"Set Bool PlayerPrefs Failed. Key({key}), Value({value}), Message({exc.Message})");
//             }
//         }

//         private void ForceSavePrefs()
//         {
//             try
//             {
//                 PlayerPrefs.Save();
//                 LOG.I($"Save PlayerPrefs Succeeded.");
//             }
//             catch (Exception exc)
//             {
//                 LOG.E($"Save PlayerPrefs Failed. Message({exc.Message})");
//             }
//         }

//         private void OnChangedPrefs(string prefsKey, string value)
//         {
//             if (!_diskPrefsDict.ContainsKey(prefsKey))
//             {
//                 // 첫 데이터는 무조건 저장되도록 $을 붙힘
//                 _diskPrefsDict[prefsKey] = string.Format("${0}", value);
//             }

//             if (_diskPrefsDict[prefsKey] == value)
//             {
//                 if (_changedPrefsDict.ContainsKey(prefsKey))
//                 {
//                     _changedPrefsDict.Remove(prefsKey);
//                 }
//             }
//             else
//             {
//                 _changedPrefsDict[prefsKey] = value;
//             }
//         }

//         public void SaveChangedPrefs()
//         {
//             if (_changedPrefsDict.Any())
//             {
//                 ForceSavePrefs();

//                 foreach (var pair in _changedPrefsDict)
//                 {
//                     _diskPrefsDict[pair.Key] = pair.Value;
//                 }

//                 LOG.I($"Save Changed prefs count : {_changedPrefsDict.Count}");
//                 _changedPrefsDict.Clear();
//             }
//         }

//         //에디터용
//         [Conditional("UNITY_EDITOR")]
//         public void _EditorSaveAll()
//         {
//             foreach (PropertyInfo info in typeof(LocalConfig).GetProperties())
//             {
//                 Type infoType = info.PropertyType;
//                 if (infoType == typeof(int))
//                     SetIntPrefs(info.Name, (int)info.GetValue(this));
//                 else if (infoType == typeof(float))
//                     SetFloatPrefs(info.Name, (float)info.GetValue(this));
//                 else if (infoType == typeof(string) || infoType == typeof(bool))
//                     SetStringPrefs(info.Name, info.GetValue(this).ToString());
//                 else
//                     LOG.E($"no handling infoType({infoType})");
//             }
//         }

//         public void Initialize()
//         {
//             throw new NotImplementedException();
//         }

//         public void Release()
//         {
//             throw new NotImplementedException();
//         }
//     }
// }