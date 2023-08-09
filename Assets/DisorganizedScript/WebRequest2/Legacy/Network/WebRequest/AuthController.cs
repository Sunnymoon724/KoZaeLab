// using System;
// using UnityEngine;

// namespace KZLib.Custom
// {
//     public static class AuthController
//     {
//         private static AccessTokenProvider tokenProvider;
//         private static Action<bool> onComplete;

//         public static void InitAuthController(AccessTokenProvider _provider,Action<bool> _onComplete,string _accessToken,string _refreshToken)
//         {
//             onComplete = _onComplete;
//             tokenProvider = _provider;

//             if(string.IsNullOrEmpty(_accessToken))
//             {
//                 tokenProvider.InitProvider(OnProviderComplete,_refreshToken);
//             }
//         }

//         private static void OnProviderComplete(AccessTokenProvider provider)
//         {
//             var failed = provider.IsDone == false || provider.IsError == true;

//             if(failed == true)
//             {
//                 Log.Network.E("UnityGoogleDrive: Failed to execute authorization procedure. Check application settings and credentials.");
//             }

//             onComplete?.Invoke(failed == false);
//         }
//     }
// }