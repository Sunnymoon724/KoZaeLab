// using ILCG;
// using ILCG.Network;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CGHudRollingBanner : MonoBehaviour
// {
//     [SerializeField] private CGUIPageScroller pageScroller;
//     [SerializeField] private Transform transformBanner;

//     private CGUICellListHandler<CGCellRollingBannerItem, STRollingBannerInfo> mBannerCellListHandler;
//     private List<STRollingBannerInfo> mBannerInfoList;

//     private Coroutine mCoWaitRefresh;

//     private void Awake()
//     {
//         mBannerInfoList = new List<STRollingBannerInfo>();
//         mBannerCellListHandler = new CGUICellListHandler<CGCellRollingBannerItem, STRollingBannerInfo>( transformBanner, nameof( CGCellRollingBannerItem ), onSetBannerData );

//         pageScroller.Initialize();

//         GameActions.ACTION_REFRESH_EVENT += refreshBanner;
//         GameActions.ACTION_REFRESH_SHOP += refreshBanner;
//         GameActions.ACTION_CONTENTS_UNLOCKED += refreshBanner;
//     }

//     private void Start()
//     {
//         refreshBanner();
//     }

//     private void OnDisable()
//     {
//         if( mCoWaitRefresh != null )
//         {
//             StopCoroutine( mCoWaitRefresh );
//             mCoWaitRefresh = null;
//         }
//     }

//     private void OnDestroy()
//     {
//         GameActions.ACTION_REFRESH_EVENT -= refreshBanner;
//         GameActions.ACTION_REFRESH_SHOP -= refreshBanner;
//         GameActions.ACTION_CONTENTS_UNLOCKED -= refreshBanner;
//     }

//     private void refreshBanner()
//     {
//         bool isShopUnlocked = isContentsUnlocked( eContents.CONTENTS_SHOP_TAB );
//         bool isEventUnlocked = isContentsUnlocked( eContents.CONTENTS_EVENT );

//         if( !isShopUnlocked || !isEventUnlocked )
//         {
//             // 둘다 열려야 할 수 있음
//             hideBanner();

//             return;
//         }


//         Dictionary<string, STRollingBannerInfo> rollingBannerInfoDict = CDataManager.Instance.GetRollingBannerInfos();

//         // 배너가 비어 있으면 out
//         if( rollingBannerInfoDict == null || rollingBannerInfoDict.Count < 1 )
//         {
//             // 끄기
//             hideBanner();

//             return;
//         }

//         List<STRollingBannerInfo> bannerInfoList = new List<STRollingBannerInfo>();

//         foreach( KeyValuePair<string, STRollingBannerInfo> pair in rollingBannerInfoDict )
//         {
//             STRollingBannerInfo bannerInfo = pair.Value;
//             string bannerId = bannerInfo.id;

//             switch( bannerInfo.type )
//             {
//                 case eBannerEventType.PICKUP: // 픽업도 이벤트라 이벤트로 분류함
//                     {
//                         if( !isVaildPickUp( bannerId ) )
//                         {
//                             continue;
//                         }
//                         break;
//                     }
//                 case eBannerEventType.EVENT:
//                     {
//                         if( !isVaildEvent( bannerId ) )
//                         {
//                             continue;
//                         }
//                         break;
//                     }
//                 case eBannerEventType.SHOP:
//                     {
//                         if( !isVaildShop( bannerId, ref bannerInfo ) )
//                         {
//                             continue;
//                         }
//                         break;
//                     }

//                 case eBannerEventType.NONE:
//                 default:
//                     continue;
//             }

//             bannerInfoList.Add( bannerInfo );
//         }

//         if( bannerInfoList.Count == 0 )
//         {
//             // 끄기
//             hideBanner();

//             return;
//         }

//         if( bannerInfoList.Count != 1 )
//         {
//             bannerInfoList.Sort( ( x, y ) => x.order.CompareTo( y.order ) );

//             STRollingBannerInfo lastInfo = bannerInfoList[ ^1 ];

//             bannerInfoList.Add( bannerInfoList[ 0 ] );
//             bannerInfoList.Insert( 0, lastInfo );
//         }


//         // 배너 생성 과정에서 노출 기간이 가장 적게 남은
//         // 이벤트 및 상점 아이템의 남은 시간으로 다음 자체 새로고침 타이밍 지정
//         long remainingTimeSelfRefresh = long.MaxValue;
//         bool foundNext = false;

//         if( mCoWaitRefresh != null )
//         {
//             StopCoroutine( mCoWaitRefresh );
//             mCoWaitRefresh = null;
//         }

//         for( int i = 0; i < bannerInfoList.Count; i++ )
//         {
//             long remain = CalculateRemainingSeconds( bannerInfoList[ i ] );

//             if( remain < remainingTimeSelfRefresh )
//             {
//                 remainingTimeSelfRefresh = remain;
//                 foundNext = true;
//             }
//         }

//         if( foundNext == true )
//         {
//             TimeSpan timeSpan = TimeSpan.FromMilliseconds(remainingTimeSelfRefresh);

//             float delaySec = ( float ) timeSpan.TotalSeconds;

//             if( delaySec < 1.0f )
//             {
//                 delaySec = 1.0f;
//             }

//             mCoWaitRefresh = StartCoroutine( IE_WaitRefresh( delaySec ) );
//         }

//         // 데이터가 똑같으면 갱신 X (갯수랑 id만 체크함)
//         if( areListsEqual( mBannerInfoList, bannerInfoList ) )
//         {
//             return;
//         }

//         mBannerInfoList = bannerInfoList;
//         mBannerCellListHandler.SetData( mBannerInfoList );

//         pageScroller.SetCount( mBannerInfoList.Count );
//     }

//     private bool isVaildPickUp( string _bannerId )
//     {
//         DataGameEventMeta eventMeta = CLocalServer.Instance.gameData.playerData.GetPlayerEventMeta( _bannerId );

//         if( eventMeta == null || eventMeta.CheckEventState() != eEventState.VALID )
//         {
//             return false;
//         }

//         switch( eventMeta.eventType )
//         {
//             case eEventType.PICKUP_STAFF:
//                 {
//                     bool isPickUpUnlocked = isContentsUnlocked( eContents.CONTENTS_PICKUP_GACHA );

//                     if( !isPickUpUnlocked )
//                     {
//                         return false;
//                     }

//                     break;
//                 }
//             case eEventType.PICKUP_INTERIOR:
//                 {
//                     bool isPickUpUnlocked = isContentsUnlocked( eContents.CONTENTS_PICKUP_GACHA_INTERIOR );

//                     if( !isPickUpUnlocked )
//                     {
//                         return false;
//                     }

//                     break;
//                 }
//             default: // 그 외의 이벤트는 안됨
//                 return false;
//         }

//         return true;
//     }

//     private bool isVaildEvent( string _bannerId )
//     {
//         if( !CLocalServer.Instance.gameData.playerData.gameEvent.IsShowEvent( _bannerId ) )
//         {
//             return false;
//         }

//         // Prelive 환경에서는 컴포스 안보이게
//         if( NetworkManager.Instance.IsPreliveEnvironment() && _bannerId == GameConstants.CONST_EVENT_COMPOSE )
//         {
//             return false;
//         }

//         return true;
//     }

//     private bool isVaildShop(string _bannerId,ref STRollingBannerInfo _rollingInfo)
//     {
//         DataGameShop shopData = CLocalServer.Instance.gameData.playerData.shop;
//         string[] productIdArray = _bannerId.Split( ',' );

//         for(int i=0;i<productIdArray.Length; i++)
//         {
//             string productId = productIdArray[ i ];
//             CGModelProductInfo productInfo = CDataManager.Instance.FindProductInfo( productId );

//             // 유효하지 않는건 제거 
//             if( !shopData.IsValidProduct( productInfo ) )
//             {
//                 continue;
//             }

//             // 재고 부족 제거
//             if( !shopData.IsProductStockAvailable( productInfo ) )
//             {
//                 continue;
//             }

//             // 일간/주간/월간 초기화 되는 상품은 제외
//             if( productInfo.buyTerm != eTermType.PERMANENT && productInfo.buyTerm != eTermType.NONE )
//             {
//                 continue;
//             }

//             // 안보이는 상품 제거
//             if( !shopData.IsShowProduct( productInfo ) )
//             {
//                 continue;
//             }

//             _rollingInfo.id = productId;

//             return true;
//         }

//         return false;
//     }

//     private bool areListsEqual( List<STRollingBannerInfo> _list1, List<STRollingBannerInfo> _list2 )
//     {
//         if( _list1.Count != _list2.Count )
//         {
//             return false;
//         }

//         for( int i = 0; i < _list1.Count; i++ )
//         {
//             if( _list1[ i ].id != _list2[ i ].id )
//             {
//                 return false;
//             }
//         }

//         return true;
//     }

//     private void onSetBannerData( CGCellRollingBannerItem _cell, STRollingBannerInfo _data )
//     {
//         _cell.SetData( _data );
//     }

//     private bool isContentsUnlocked( eContents _content )
//     {
//         return CLocalServer.Instance.contentsUnlockSystem.GetContentsState( _content ) == eContentsState.UNLOCKED;
//     }

//     private void hideBanner()
//     {
//         mBannerCellListHandler.HideAllCells();
//         pageScroller.HideAll();
//     }


//     /// <summary>
//     /// 특정 배너의 남은 시간 계산
//     /// </summary>
//     private long CalculateRemainingSeconds( STRollingBannerInfo _bannerInfo )
//     {
//         switch( _bannerInfo.type )
//         {
//             case eBannerEventType.PICKUP:
//             case eBannerEventType.EVENT:
//                 {
//                     DataGameEventMeta eventMeta = CLocalServer.Instance.gameData.playerData.GetPlayerEventMeta( _bannerInfo.id );

//                     if( eventMeta == null )
//                     {
//                         return long.MaxValue;
//                     }

//                     if( eventMeta.TryGetRemainingTime( out long remainAt ) )
//                     {
//                         return Math.Max(0,remainAt);
//                     }

//                     break;
//                 }

//             case eBannerEventType.SHOP:
//                 {
//                     CGModelProductInfo productInfo = CDataManager.Instance.FindProductInfo( _bannerInfo.id );

//                     if( productInfo == null )
//                     {
//                         return long.MaxValue;
//                     }

//                     if( productInfo.TryGetRemainingTime( out long remainAt ) )
//                     {
//                         return Math.Max(0,remainAt);
//                     }

//                     break;
//                 }
//         }

//         return long.MaxValue;
//     }

//     /// <summary>
//     /// 일정 시간 대기 후 자체 새로고침
//     /// </summary>
//     private IEnumerator IE_WaitRefresh( float _delaySecond )
//     {
//         yield return new WaitForSeconds( _delaySecond );

//         mCoWaitRefresh = null;

//         refreshBanner();
//     }
// }