using UnityEngine;
using KZLib.KZAttribute;
using KZLib.KZData;
using System.Collections.Generic;

namespace KZLib
{
	[RequireComponent(typeof(Animator))]
	public class MotionCon : MonoBehaviour
	{
		[SerializeField]
		protected Animator m_animator = null;

		[SerializeField,KZRichText]
		protected string m_currentStateName = null;

		private readonly Dictionary<int,MotionEvent> m_motionEventDict = new();

		public void PlayAnimation(string stateName,float normalizedTime = 0.0f)
		{
			if(!m_animator)
			{
				return;
			}

			m_currentStateName = stateName;

			m_animator.Play(stateName,0,normalizedTime);
		}

		public void PlayMotion(int motionNum)
		{
			if(!m_animator)
			{
				return;
			}

			var motionPrt = ProtoMgr.In.GetProto<MotionProto>(motionNum);

			m_motionEventDict.Clear();

			foreach(var motionEvent in motionPrt.EventArray)
			{
				m_motionEventDict.Add(motionEvent.Order,motionEvent);
			}

			PlayAnimation(motionPrt.StateName,0.0f);
		}

		protected void OnPlayEffect(int order)
		{
			if(m_motionEventDict.TryGetValue(order,out var motionEvent))
			{
				PlayMotionEvent(motionEvent);
			}
		}

		protected virtual void PlayMotionEvent(MotionEvent motionEvent) { }

		protected void Reset()
		{
			if(!m_animator)
			{
				m_animator = GetComponent<Animator>();
			}
		}
	}
}