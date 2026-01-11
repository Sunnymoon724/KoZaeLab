// using Natris;

// using System;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// namespace ILCG
// {
//     public class CGCellRollingBannerItem : MonoBehaviour
//     {
//         [SerializeField] private TextMeshProUGUI textBanner;
//         [SerializeField] private Image imageBanner;
//         [SerializeField] private ButtonEx buttonBanner;

//         private string mBannerID;
//         private eBannerEventType mBannerType;

//         private void Awake()
//         {
//             buttonBanner.onClick.AddListener( onClicked );
//         }

//         public void SetData( STRollingBannerInfo _bannerInfo )
//         {
//             if( textBanner != null )
//             {
//                 textBanner.text = CDataManager.Instance.GetText( _bannerInfo.nameKey );
//             }

//             Sprite sprite = CNResourceManager.Instance.GetSprite( eAtlas.Atlas_UI_Event_Thumbnail, string.Format( "image_event_thumbnail_rolling_{0}", _bannerInfo.imagePath ) );

//             if( sprite != null )
//             {
//                 imageBanner.sprite = sprite;
//             }

//             mBannerID = _bannerInfo.id;
//             mBannerType = _bannerInfo.type;
//         }

//         private void onClicked()
//         {
//             switch( mBannerType )
//             {
//                 case eBannerEventType.SHOP:
//                     {
//                         moveToShop();

//                         break;
//                     }
//                 case eBannerEventType.EVENT:
//                     {
//                         moveToEvent();

//                         break;
//                     }
//                 case eBannerEventType.PICKUP:
//                     {
//                         moveToPickUp();

//                         break;
//                     }

//                 case eBannerEventType.NONE:
//                 default:
//                     {
//                         return;
//                     }
//             }
//         }

//         private void moveToShop()
//         {
//             CGModelProductInfo productInfo = CDataManager.Instance.FindProductInfo( mBannerID );

//             eShopMainCategoryType mainCategoryType = productInfo.mainCategoryType;

//             if( mainCategoryType == eShopMainCategoryType.NONE )
//             {
//                 return;
//             }

//             int bannerIndex = int.TryParse( mBannerID, out int index ) ? index : 0;

//             GameUtils.OpenShopTab( mainCategoryType, bannerIndex );
//         }

//         private void moveToEvent()
//         {
//             DataGameEventMeta eventMeta = CLocalServer.Instance.gameData.playerData.GetPlayerEventMeta( mBannerID );

//             GameUtils.OpenEventLayer( mBannerID, eventMeta.eventType );
//         }

//         private void moveToPickUp()
//         {
//             DataGameEventMeta eventMeta = CLocalServer.Instance.gameData.playerData.GetPlayerEventMeta( mBannerID );

//             List<string> eventIdList = CLocalServer.Instance.gameData.playerData.ExtractPlayerEventMetaList( eventMeta.eventType );

//             int index = eventIdList.FindIndex( x => x == mBannerID );

//             if( index == -1 )
//             {
//                 return;
//             }

//             string eventString = eventMeta.eventType.ToString();
//             string gachaString = eventString.Replace( "PICKUP", "GACHA" );
            
//             if(!Enum.IsDefined(typeof(eGachaType),gachaString))
//             {
//                 return;
//             }

//             eGachaType gachaType = CNUtils.ConvertEnumData<eGachaType>(gachaString);

//             index += (int) gachaType + 1; // +1 하는 이유는 픽업은 1부터 이므로

//             GameUtils.OpenShopTab( eShopMainCategoryType.SHOP_GACHA, index );
//         }
//     }
// }