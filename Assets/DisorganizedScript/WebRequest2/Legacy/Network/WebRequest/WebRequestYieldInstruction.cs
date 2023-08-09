// using System;
// using UnityEngine;

// namespace KZLib.Custom
// {
//     public class WebRequestYieldInstruction : CustomYieldInstruction
//     {
//         private Action onComplete;
//         public bool IsDone => CustomRequest.IsDone;
//         public override bool keepWaiting => !IsDone;
//         public float Progress => CustomRequest.Progress;

//         public CustomWebRequest CustomRequest { get; private set; }

//         public WebRequestYieldInstruction(CustomWebRequest _request)
//         {
//             CustomRequest = _request;
//             CustomRequest.SetRequestComplete(OnRequestComplete);
//         }

//         private void OnRequestComplete()
//         {
//             onComplete?.Invoke();
//         }

//         public void AddOnComplete(Action _onComplete)
//         {
//             onComplete += _onComplete;
//         }
//     }

//     public class WebRequestYieldInstruction<T> : CustomYieldInstruction
//     {
//         private Action<T> onComplete;

//         public bool IsDone => CustomRequest.IsDone;
//         public override bool keepWaiting => !IsDone;
//         public float Progress => CustomRequest.Progress;

//         public CustomWebRequest<T> CustomRequest { get; private set; }

//         public WebRequestYieldInstruction(CustomWebRequest<T> _request)
//         {
//             CustomRequest = _request;
//             CustomRequest.SetRequestComplete(OnRequestComplete);
//         }

//         private void OnRequestComplete(T _data)
//         {
//             onComplete?.Invoke(_data);
//         }

//         public void AddOnComplete(Action<T> _onComplete)
//         {
//             onComplete += _onComplete;
//         }
//     }
// }