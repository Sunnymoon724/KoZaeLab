using UnityEngine;

namespace KZLib.KZDevelop
{
	/// <summary>
	/// Manage pool of GameObjects.
	/// </summary>
	public class GameObjectUIPool<TComponent> : GameObjectPool<TComponent> where TComponent : Component
	{
		public GameObjectUIPool(TComponent pivot,Transform storage,int capacity) : base(pivot,storage,capacity) { }

		protected override void SetChild(Transform parent,Transform child)
		{
			parent.SetUIChild(child);
		}
	}
}