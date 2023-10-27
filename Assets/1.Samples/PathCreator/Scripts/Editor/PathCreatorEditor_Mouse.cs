#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;

namespace KZLib.KZEditor
{
	public partial class PathCreatorEditor : OdinEditor
	{
		private Vector3 GetMouseWorldPosition(SpaceType _type,float _depth = 10.0f)
		{
			var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			var worldMouse = mouseRay.GetPoint(_depth);

			if(_type == SpaceType.xy && mouseRay.direction.z != 0.0f)
			{
				worldMouse = mouseRay.GetPoint(Mathf.Abs(mouseRay.origin.z/mouseRay.direction.z));
			}
			else if(_type == SpaceType.xz && mouseRay.direction.y != 0)
			{
				worldMouse = mouseRay.GetPoint(Mathf.Abs(mouseRay.origin.y/mouseRay.direction.y));
			}

			return worldMouse;
		}
	}
}
#endif