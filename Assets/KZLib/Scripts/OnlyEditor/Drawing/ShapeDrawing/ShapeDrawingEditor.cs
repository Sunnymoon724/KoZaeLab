#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using KZLib.Utilities;
using UnityEditor;
using UnityEngine;

namespace KZLib.UI
{
	[CustomEditor(typeof(ShapeDrawing))]
	public partial class ShapeDrawingEditor : GraphicDrawingEditor
	{
		protected override Transform CurrentTransform => m_shapeDrawing.transform;

		private ShapeDrawing m_shapeDrawing = null;

		private class ShapeEditorCatalog : StrategyCatalog<ShapeDrawingEditor,ShapeDrawing.ShapePrimitiveType,ShapeEditorStrategy>
		{
			public ShapeEditorCatalog(ShapeDrawingEditor owner) : base(owner) { }

			protected override Dictionary<ShapeDrawing.ShapePrimitiveType,ShapeEditorStrategy> _BindStrategy()
			{
				return new()
				{
					[ShapeDrawing.ShapePrimitiveType.Ellipse] = new(m_owner._Draw_Ellipse,m_owner._CanShowKnot_Ellipse,m_owner._SyncAllKnotInfos_Ellipse,m_owner._RefreshKnotInfo_Ellipse),
					[ShapeDrawing.ShapePrimitiveType.Polygon] = new(m_owner._Draw_Polygon,m_owner._CanShowKnot_Polygon,m_owner._SyncAllKnotInfos_Polygon,m_owner._RefreshKnotInfo_Polygon),
					[ShapeDrawing.ShapePrimitiveType.Triangle] = new(m_owner._Draw_Triangle,m_owner._CanShowKnot_Triangle,m_owner._SyncAllKnotInfos_Triangle,m_owner._RefreshKnotInfo_Triangle),
				};
			}
		}

		private readonly struct ShapeEditorStrategy
		{
			public readonly Action Draw;
			public readonly Func<bool> CanShowKnot;
			public readonly Action SyncAllKnotInfos;
			public readonly Action<int,Vector3> RefreshKnotInfo;

			public ShapeEditorStrategy(Action draw,Func<bool> canShowKnot,Action syncAllKnotInfos,Action<int,Vector3> refreshKnotInfo)
			{
				Draw = draw;
				CanShowKnot = canShowKnot;
				SyncAllKnotInfos = syncAllKnotInfos;
				RefreshKnotInfo = refreshKnotInfo;
			}
		}

		private ShapeEditorCatalog m_shapeEditorCatalog = null;

		private bool _TryGetStrategy(ShapeDrawing.ShapePrimitiveType primitiveType,out ShapeEditorStrategy strategy)
		{
			m_shapeEditorCatalog ??= new ShapeEditorCatalog(this);

			return m_shapeEditorCatalog.TryGetStrategy(primitiveType,out strategy);
		}

		protected override void _DoEnable()
		{
			m_shapeDrawing = target as ShapeDrawing;
		}

		protected override void _DoInspectorGUI()
		{
			void _SetPrimitiveType(ShapeDrawing.ShapePrimitiveType primitiveType)
			{
				m_shapeDrawing.PrimitiveType = primitiveType;
			}

			_DrawEnumInspector("Primitive Type",m_shapeDrawing.PrimitiveType,_SetPrimitiveType);


			void _SetOutlineThickness(float outlineThickness)
			{
				var rect = m_shapeDrawing.rectTransform.rect;
				var thickness = Mathf.Min(rect.width,rect.height);

				m_shapeDrawing.OutlineThickness = Mathf.Clamp(outlineThickness,0.0f,thickness/2.0f);
			}

			_DrawFloatInspector("Outline Thickness",m_shapeDrawing.OutlineThickness,_SetOutlineThickness);


			void _SetOutlineColor(Color outlineColor)
			{
				m_shapeDrawing.OutlineColor = outlineColor;
			}

			_DrawColorInspector("Outline Color",m_shapeDrawing.OutlineColor,_SetOutlineColor);

			_DrawDefaultInspector();

			if(_TryGetStrategy(m_shapeDrawing.PrimitiveType,out var strategy))
			{
				strategy.Draw();
			}


			void _SetFillType(ShapeDrawing.ShapeFillType fillType)
			{
				m_shapeDrawing.FillType = fillType;
			}

			_DrawEnumInspector("Fill Type",m_shapeDrawing.FillType,_SetFillType);

			void _SetFillColor(Color color)
			{
				m_shapeDrawing.FillColor = color;
			}

			_DrawColorInspector("Fill Color",m_shapeDrawing.FillColor,_SetFillColor);


			void _SetAntiAliasing(float antiAliasing)
			{
				m_shapeDrawing.AntiAliasing = antiAliasing;
			}

			_DrawFloatInspector("Anti Aliasing",m_shapeDrawing.AntiAliasing,_SetAntiAliasing);
		}

		private void _DrawDefaultInspector()
		{
			_DrawDefaultInspector_Material(m_shapeDrawing);
			_DrawDefaultInspector_RaycastTarget(m_shapeDrawing);
			_DrawDefaultInspector_RaycastPadding(m_shapeDrawing);
			_DrawDefaultInspector_Maskable(m_shapeDrawing);
		}

		private void OnSceneGUI()
		{
			_SceneGUI();
		}

		protected override bool _CanShowKnot()
		{
			var radius = m_shapeDrawing.Radius;

			if(radius.x <= 0.0f || radius.y <= 0.0f)
			{
				return false;
			}

			return _TryGetStrategy(m_shapeDrawing.PrimitiveType,out var strategy) && strategy.CanShowKnot();
		}

		protected override void _DoUpdateKnotList()
		{
			CurrentTransform.GetPositionAndRotation(out var center,out var _);

			_SyncKnotInfo(0,center,KnotType.Fixed);

			if(_TryGetStrategy(m_shapeDrawing.PrimitiveType,out var strategy))
			{
				strategy.SyncAllKnotInfos();
			}
		}

		protected override void _ChangeKnotPosition(int index,Vector3 newPosition)
		{
			var localPos = CurrentTransform.InverseTransformPoint(newPosition);

			if(_TryGetStrategy(m_shapeDrawing.PrimitiveType,out var strategy))
			{
				strategy.RefreshKnotInfo(index,localPos);
			}
		}

		protected override Vector3 _ConvertToMousePosition(Vector3 mousePosition)
		{
			var plane = new Plane(Vector3.forward,CurrentTransform.position);
			var ray = HandleUtility.GUIPointToWorldRay(mousePosition);

			return plane.Raycast(ray,out float enter) ? ray.GetPoint(enter) : CurrentTransform.position;
		}

		private Vector3 _GetVertexPosition(float degree)
		{
			var radius = m_shapeDrawing.Radius;
			var radian = Mathf.Deg2Rad*degree;

			var cos = Mathf.Cos(radian);
			var sin = Mathf.Sin(radian);

			return new Vector3(cos*radius.x,sin*radius.y);
		}

		private void _SyncWorldKnotInfo(int index,Vector3 position,KnotType knotType)
		{
			CurrentTransform.GetPositionAndRotation(out var center,out var rotation);

			var worldPos = center+rotation*position;

			_SyncKnotInfo(index,worldPos,knotType);
		}
	}
}
#endif