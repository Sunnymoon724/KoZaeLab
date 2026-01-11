// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;
// using System.Collections;
// using System.Collections.Generic;
// using Natris;

// namespace ILCG
// {
//     public class CGUIPageScroller : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
//     {
//         [SerializeField] private ScrollRect scrollRect;
//         [SerializeField] private Transform transformContent;
//         [SerializeField] private float scrollSpeed;
//         [SerializeField] private float threshold;
//         [SerializeField] private float delayTime;
//         [SerializeField] private float intervalTime;

//         private int mMaxCount;
//         private int mPreviousIndex;
//         private int mCurrentIndex;
//         private float[] mPositionArray;
//         private bool mCanDrag;
//         private bool mIsDragging;

//         private IEnumerator mEnumerator;

//         private List<CGCellRollingNavigator> mNavigatorList;

//         public void Initialize()
//         {
//             mNavigatorList = new List<CGCellRollingNavigator>();
//         }

//         public void SetCount( int _count )
//         {
//             mCanDrag = true;

//             if( _count == 0 || _count == 1 )
//             {
//                 HideAll();

//                 return;
//             }

//             mPositionArray = new float[ _count ];

//             for( int i = 0; i < _count; i++ )
//             {
//                 mPositionArray[ i ] = ( float )i / ( _count - 1 );
//             }

//             mMaxCount = _count - 2;
//             mIsDragging = false;
//             mCurrentIndex = 1;
//             mPreviousIndex = 1;

//             string prefabName = nameof( CGCellRollingNavigator );

//             GameObject prefab = CNResourceManager.Instance.GetPrefab( prefabName );

//             if( prefab == null )
//             {
//                 Log.logError( $"{prefabName}의 이름을 가진 프리펩은 없습니다." );

//                 HideAll();

//                 return;
//             }

//             int toCreate = mMaxCount - mNavigatorList.Count;

//             for( int i = 0; i < toCreate; i++ )
//             {
//                 CGCellRollingNavigator navigator = Instantiate( prefab, transformContent ).GetComponent<CGCellRollingNavigator>();
//                 mNavigatorList.Add( navigator );
//             }

//             for( int i = 0; i < mNavigatorList.Count; i++ )
//             {
//                 CGCellRollingNavigator navigator = mNavigatorList[ i ];

//                 if( i < mMaxCount )
//                 {
//                     navigator.gameObject.SetActive( true );
//                 }
//                 else
//                 {
//                     navigator.gameObject.SetActive( false );
//                 }
//             }

//             stopEnumerator();

//             StartCoroutine( mEnumerator = IE_SmoothMove( scrollRect.horizontalNormalizedPosition, mPositionArray[ mCurrentIndex ], 0.0f ) );
//         }

//         public void OnBeginDrag( PointerEventData _eventData )
//         {
//             if( !mCanDrag )
//             {
//                 return;
//             }

//             mIsDragging = true;

//             stopEnumerator();
//         }

//         public void OnDrag( PointerEventData _eventData )
//         {
//             if( !mCanDrag || !mIsDragging )
//             {
//                 return;
//             }

//             float min = ( mCurrentIndex > 0 ) ? mPositionArray[ mCurrentIndex - 1 ] : mPositionArray[ mCurrentIndex ];
//             float max = ( mCurrentIndex < mPositionArray.Length - 1 ) ? mPositionArray[ mCurrentIndex + 1 ] : mPositionArray[ mCurrentIndex ];

//             scrollRect.horizontalNormalizedPosition = Mathf.Clamp( scrollRect.horizontalNormalizedPosition, min, max );
//         }

//         public void OnEndDrag( PointerEventData _eventData )
//         {
//             if( !mCanDrag || !mIsDragging )
//             {
//                 return;
//             }

//             mIsDragging = false;

//             float delta = _eventData.pressPosition.x - _eventData.position.x;

//             mPreviousIndex = mCurrentIndex;

//             if( Mathf.Abs( delta ) > threshold )
//             {
//                 if( delta > 0 && mCurrentIndex < mPositionArray.Length - 1 )
//                 {
//                     mCurrentIndex++;
//                 }
//                 else if( delta < 0 && mCurrentIndex > 0 )
//                 {
//                     mCurrentIndex--;
//                 }
//             }

//             stopEnumerator();

//             StartCoroutine( mEnumerator = IE_SmoothMove( scrollRect.horizontalNormalizedPosition, mPositionArray[ mCurrentIndex ], 0.3f ) );
//         }

//         private IEnumerator IE_SmoothMove( float _from, float _to, float _duration )
//         {
//             if( _duration != 0.0f )
//             {
//                 float time = 0.0f;

//                 while( time < _duration )
//                 {
//                     time += Time.deltaTime;
//                     scrollRect.horizontalNormalizedPosition = Mathf.Lerp( _from, _to, time / _duration );

//                     yield return null;
//                 }
//             }
//             else
//             {
//                 // ui 갱신용
//                 yield return null;
//             }

//             scrollRect.horizontalNormalizedPosition = _to;

//             // 순환 처리
//             if( mCurrentIndex == 0 )
//             {
//                 mCurrentIndex = mMaxCount;
//                 scrollRect.horizontalNormalizedPosition = mPositionArray[ mCurrentIndex ];
//             }
//             else if( mCurrentIndex == mPositionArray.Length - 1 )
//             {
//                 mCurrentIndex = 1;
//                 scrollRect.horizontalNormalizedPosition = mPositionArray[ mCurrentIndex ];
//             }

//             SetNavigator(mCurrentIndex - 1);

//             mEnumerator = null;

//             yield return StartCoroutine( mEnumerator = IE_AutoScroll() );
//         }

//         private void stopEnumerator()
//         {
//             if( mEnumerator != null )
//             {
//                 StopCoroutine( mEnumerator );

//                 mEnumerator = null;
//             }
//         }

//         private IEnumerator IE_AutoScroll()
//         {
//             yield return new WaitForSeconds( intervalTime );

//             mPreviousIndex = mCurrentIndex;
//             mCurrentIndex++;

//             int nextIndex = Mathf.Min( mCurrentIndex, mPositionArray.Length - 1 );

//             mEnumerator = null;

//             yield return StartCoroutine( mEnumerator = IE_SmoothMove( scrollRect.horizontalNormalizedPosition, mPositionArray[ nextIndex ], 0.3f ) );
//         }

//         public void HideAll()
//         {
//             mCanDrag = false;

//             for( int i = 0; i < mNavigatorList.Count; i++ )
//             {
//                 mNavigatorList[ i ].gameObject.SetActive( false );
//             }
//         }

//         public void SetNavigator(int _navigator)
//         {
//             for( int i = 0; i  < mNavigatorList.Count; i++ )
//             {
//                 mNavigatorList[ i ].SetData( i == _navigator );
//             }
//         }
//     }
// }