// #if UNITY_EDITOR
// using System;
// using UnityEngine;

// public abstract class GoogleYieldInstruction : CustomYieldInstruction
// {
// 	public event Action OnCompletedNonGeneric;
// 	public abstract bool IsDone { get; }

// 	public void AddOnComplete(Action _onCompleted)
// 	{
// 		OnCompletedNonGeneric += _onCompleted;
// 	}

// 	protected void InvokeOnComplete()
// 	{
// 		OnCompletedNonGeneric?.Invoke();
// 	}
// }

// public class GoogleYieldInstruction<T> : GoogleYieldInstruction
// {
// 	private KZLib.Auth.CustomWebRequest<T> m_CustomRequest;
// 	public event Action<T> OnCompleted;

// 	public override bool IsDone => m_CustomRequest.IsDone;
// 	public override bool keepWaiting => IsDone == false;
// 	public float Progress => m_CustomRequest.Progress;

// 	public GoogleYieldInstruction(KZLib.Auth.CustomWebRequest<T> _request)
// 	{
// 		m_CustomRequest = _request;
// 		m_CustomRequest.OnCompleted += OnRequestComplete;
// 	}

// 	private void OnRequestComplete(T _data)
// 	{
// 		m_CustomRequest.OnCompleted -= OnRequestComplete;

// 		OnCompleted?.Invoke(_data);
		
// 		InvokeOnComplete();
// 	}
// }
// #endif