using Sirenix.OdinInspector;
using UnityEngine;

public abstract class AnchorFocusSlotUI : FocusSlotUI
{
	[VerticalGroup("Focus/0",Order = 0),SerializeField]
	private bool m_Vertical = false;

	[VerticalGroup("Focus/0",Order = 0),SerializeField]
	private float m_AnchorValue = 0.2f;

	public override void UpdateLocation(float _location)
	{
		base.UpdateLocation(_location);

		if(m_Vertical)
		{
			var anchor = Mathf.Lerp(1.0f+m_AnchorValue,0.0f-m_AnchorValue,_location);

			m_Slot.anchorMin = m_Slot.anchorMin.MaskY(anchor);
			m_Slot.anchorMax = m_Slot.anchorMax.MaskY(anchor);
		}
		else
		{
			var anchor = Mathf.Lerp(0.0f-m_AnchorValue,1.0f+m_AnchorValue,_location);

			m_Slot.anchorMin = m_Slot.anchorMin.MaskX(anchor);
			m_Slot.anchorMax = m_Slot.anchorMax.MaskX(anchor);
		}
	}

#if UNITY_EDITOR
	protected override void DrawGizmo()
	{
		base.DrawGizmo();

		var cached = Gizmos.color;
		var cubeSize = CubeSize;

		if(m_Vertical)
		{
			var height = UIRectTransform.rect.height*(2.0f*m_AnchorValue+1.0f)/2.0f;

			Gizmos.color = Color.red;

			Gizmos.DrawCube(transform.position+Vector3.up*height,cubeSize);
			Gizmos.DrawCube(transform.position+Vector3.down*height,cubeSize);
		}
		else
		{
			var width = UIRectTransform.rect.width*(2.0f*m_AnchorValue+1.0f)/2.0f;

			Gizmos.color = Color.red;

			Gizmos.DrawCube(transform.position+Vector3.left*width,cubeSize);
			Gizmos.DrawCube(transform.position+Vector3.right*width,cubeSize);
		}

		Gizmos.color = cached;
	}
#endif
}