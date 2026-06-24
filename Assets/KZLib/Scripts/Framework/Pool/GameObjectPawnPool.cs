using System;
using UnityEngine;

namespace KZLib.Utilities
{
	internal interface IPawnHook : IPawn
	{
		void Release();
		void Destroy();
	}

	public abstract class GameObjectPawn : MonoBehaviour,IPawnHook
	{
		internal void Release() => _Release();
		internal void Destroy() => _Destroy();

		protected abstract void _Release();

		protected abstract void _Destroy();

		void IPawnHook.Release()
		{
			Release();
		}

		void IPawnHook.Destroy()
		{
			gameObject.DestroyObject();
		}
	}

	/// <summary>
	/// Object pool for <see cref="Component"/> instances backed by cloned GameObjects.
	/// Use <see cref="GetOrCreate"/> to retrieve (activates, optional parent change) and <see cref="Put"/> to return (deactivates, moves to storage).
	/// </summary>
	public class GameObjectPawnPool<TComponent> : PawnPool<TComponent> where TComponent : GameObjectPawn
	{
		/// <summary>Parent transform for inactive pooled instances.</summary>
		private readonly Transform m_storage = null;

		/// <summary>Passed to <see cref="TransformExtension.SetChild"/> when changing parent.</summary>
		private readonly bool m_worldPositionStays = true;

		private readonly float m_deleteTime = 0.0f;

		private IDisposable m_delaySubscription = null;

		/// <param name="pivot">Template instance copied for each pool entry.</param>
		/// <param name="storage">Inactive instances are parented here on <see cref="Put"/>.</param>
		/// <param name="capacity">Initial pool size; pre-filled via <c>_Prepare</c>.</param>
		/// <param name="worldPositionStays">Whether changing parent preserves world position/rotation.</param>
		public GameObjectPawnPool(TComponent pivot,Transform storage,int capacity,bool worldPositionStays,float deleteTime = 60.0f) : base(pivot,capacity,_CopyObject,_DestroyObject)
		{
			if(!storage)
			{
				throw new NullReferenceException("Storage is null.");
			}

			if(deleteTime < 0.0f)
			{
				throw new ArgumentOutOfRangeException(nameof(deleteTime),deleteTime,"Delete time cannot be negative.");
			}

			m_storage = storage;
			m_worldPositionStays = worldPositionStays;

			m_deleteTime = deleteTime;
		}

		protected override void _Dispose(bool disposing)
		{
			if(disposing)
			{
				KZExternalKit.KillSubscription(ref m_delaySubscription);
			}

			base._Dispose(disposing);
		}

		private static TComponent _CopyObject(TComponent pivot) 
		{
			return pivot.CopyObject() as TComponent ?? throw new NullReferenceException("Pawn is null.");
		}

		private static void _DestroyObject(TComponent pawn)
		{
			if(pawn is IPawnHook pawnHook)
			{
				pawnHook.Destroy();
			}
		}

		/// <summary>Sets <paramref name="parent"/> as the parent of <paramref name="child"/> when parent is non-null.</summary>
		protected void _SetChild(Transform parent,Transform child)
		{
			if(parent)
			{
				parent.SetChild(child,m_worldPositionStays);
			}
		}

		public override void Put(TComponent pawn)
		{
			if(pawn is IPawnHook pawnHook)
			{
				pawnHook.Release();
			}

			_SetChild(m_storage,pawn.transform);

			pawn.gameObject.EnsureActive(false);

			base.Put(pawn);

			KZExternalKit.KillSubscription(ref m_delaySubscription);

			m_delaySubscription = KZExternalKit.DelayAction(_Purge,m_deleteTime);
		}

		public override TComponent GetOrCreate()
		{
			return GetOrCreate(null);
		}

		/// <summary>
		/// Gets or creates an instance, optionally sets parent to <paramref name="parent"/>, then activates.
		/// </summary>
		public TComponent GetOrCreate(Transform parent)
		{
			var pawn = base.GetOrCreate();

			if(parent)
			{
				_SetChild(parent,pawn.transform);
			}

			pawn.gameObject.EnsureActive(true);

			return pawn;
		}

		public override void PurgeForce()
		{
			KZExternalKit.KillSubscription(ref m_delaySubscription);

			base.PurgeForce();
		}
	}
}