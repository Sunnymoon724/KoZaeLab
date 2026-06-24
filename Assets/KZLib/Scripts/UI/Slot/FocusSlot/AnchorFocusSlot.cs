using Sirenix.OdinInspector;
using UnityEngine;

namespace KZLib.UI
{
	/// <summary>
	/// <see cref="FocusSlot"/> that moves <c>m_focusUI</c> anchors along the scroll axis as
	/// <see cref="RefreshLocation"/> changes.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="m_anchorValue"/> extends travel beyond the 0–1 anchor range so the focus UI can sit slightly
	/// outside the cell at the start/end of the scroll range. <see cref="m_vertical"/> selects whether X or Y anchors
	/// are driven.
	/// </para>
	/// <para>
	/// Editor gizmos draw red cubes at the computed scroll extents on top of the base focus label gizmo.
	/// </para>
	/// </remarks>
	public abstract class AnchorFocusSlot : FocusSlot
	{
		/// <summary>When true, <see cref="RefreshLocation"/> adjusts Y anchors; otherwise X anchors.</summary>
		[VerticalGroup("Focus/0",Order = 0),SerializeField]
		private bool m_vertical = false;

		/// <summary>Extra anchor offset beyond the 0–1 range at location 0 and 1.</summary>
		[VerticalGroup("Focus/0",Order = 0),SerializeField]
		private float m_anchorValue = 0.2f;

		/// <summary>Updates center state, then lerp focus UI anchors for the current scroll location.</summary>
		public override void RefreshLocation(float location)
		{
			base.RefreshLocation(location);

			if(!m_focusUI)
			{
				return;
			}

			if(m_vertical)
			{
				var anchor = Mathf.Lerp(1.0f+m_anchorValue,0.0f-m_anchorValue,location);

				m_focusUI.anchorMin = m_focusUI.anchorMin.SetY(anchor);
				m_focusUI.anchorMax = m_focusUI.anchorMax.SetY(anchor);
			}
			else
			{
				var anchor = Mathf.Lerp(0.0f-m_anchorValue,1.0f+m_anchorValue,location);

				m_focusUI.anchorMin = m_focusUI.anchorMin.SetX(anchor);
				m_focusUI.anchorMax = m_focusUI.anchorMax.SetX(anchor);
			}
		}

#if UNITY_EDITOR
		/// <summary>Draws base gizmo text plus red cubes at horizontal or vertical scroll extents.</summary>
		protected override void _DrawGizmo()
		{
			base._DrawGizmo();

			var cached = Gizmos.color;
			var cubeSize = CubeSize;
			var rect = RootRect.rect;

			if(m_vertical)
			{
				var height = rect.height*(2.0f*m_anchorValue+1.0f)/2.0f;

				Gizmos.color = Color.red;

				Gizmos.DrawCube(transform.position+Vector3.up*height,cubeSize);
				Gizmos.DrawCube(transform.position+Vector3.down*height,cubeSize);
			}
			else
			{
				var width = rect.width*(2.0f*m_anchorValue+1.0f)/2.0f;

				Gizmos.color = Color.red;

				Gizmos.DrawCube(transform.position+Vector3.left*width,cubeSize);
				Gizmos.DrawCube(transform.position+Vector3.right*width,cubeSize);
			}

			Gizmos.color = cached;
		}
#endif
	}
}
