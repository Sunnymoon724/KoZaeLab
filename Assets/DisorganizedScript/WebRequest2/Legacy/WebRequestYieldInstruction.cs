// using System;
// using UnityEngine;

// public class WebRequestYieldInstruction : CustomYieldInstruction
// {
// 	private CustomWebRequest m_Request;
// 	public event Action OnCompleted;
	
// 	public bool IsDone => m_Request.IsDone;
// 	public override bool keepWaiting => !IsDone;
// 	public float Progress => m_Request.Progress;

// 	public WebRequestYieldInstruction(CustomWebRequest _request)
// 	{
// 		m_Request = _request;
// 		m_Request.OnCompleted += OnRequestComplete;
// 	}

// 	private void OnRequestComplete()
// 	{
// 		m_Request.OnCompleted -= OnRequestComplete;

// 		OnCompleted?.Invoke();
// 	}
// }

// public class WebRequestYieldInstruction<T> : CustomYieldInstruction
// {
// 	private CustomWebRequest<T> m_Request;
// 	public event Action<T> OnCompleted;

// 	public bool IsDone => m_Request.IsDone;
// 	public override bool keepWaiting => !IsDone;
// 	public float Progress => m_Request.Progress;

// 	public WebRequestYieldInstruction(CustomWebRequest<T> _request)
// 	{
// 		m_Request = _request;
// 		m_Request.OnCompleted += OnRequestComplete;
// 	}

// 	private void OnRequestComplete(T _data)
// 	{
// 		m_Request.OnCompleted -= OnRequestComplete;

// 		OnCompleted?.Invoke(_data);
// 	}
// }