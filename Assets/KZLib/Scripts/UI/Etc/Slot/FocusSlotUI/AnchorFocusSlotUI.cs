using Sirenix.OdinInspector;
using UnityEngine;

public abstract class AnchorFocusSlotUI : FocusSlotUI
{
	[VerticalGroup("Focus/0",Order = 0),SerializeField]
	private bool m_vertical = false;

	[VerticalGroup("Focus/0",Order = 0),SerializeField]
	private float m_anchorValue = 0.2f;

	public override void UpdateLocation(float location)
	{
		base.UpdateLocation(location);

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
	protected override void DrawGizmo()
	{
		base.DrawGizmo();

		var cached = Gizmos.color;
		var cubeSize = CubeSize;

		if(m_vertical)
		{
			var height = UIRectTransform.rect.height*(2.0f*m_anchorValue+1.0f)/2.0f;

			Gizmos.color = Color.red;

			Gizmos.DrawCube(transform.position+Vector3.up*height,cubeSize);
			Gizmos.DrawCube(transform.position+Vector3.down*height,cubeSize);
		}
		else
		{
			var width = UIRectTransform.rect.width*(2.0f*m_anchorValue+1.0f)/2.0f;

			Gizmos.color = Color.red;

			Gizmos.DrawCube(transform.position+Vector3.left*width,cubeSize);
			Gizmos.DrawCube(transform.position+Vector3.right*width,cubeSize);
		}

		Gizmos.color = cached;
	}
#endif
}