// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Sirenix.OdinInspector;
// using UnityEngine;
// using UnityEngine.UI;

// public class UISquircle : UIShape
// {
// 	[VerticalGroup("도형 옵션/2",Order = 2),SerializeField,PropertyRange(2.0f,40.0f)]
// 	private float m_Radius = 4;

// 	protected override void OnPopulateMesh(VertexHelper _vertexHelper)
// 	{
// 		var vertexList = new List<Vector2>();
// 		var width = rectTransform.rect.width/2.0f;
// 		var height = rectTransform.rect.height/2.0f;
// 		var pivot = Mathf.Min(width,height,1000);

// 		var dx = width-pivot;
// 		var dy = height-pivot;

// 		var x = 0.0f;
// 		var y = 1.0f;

// 		vertexList.Add(new Vector2(0,height));

// 		while(x < y)
// 		{
// 			y = Mathf.Pow(1.0f-Mathf.Pow(x/pivot,m_Radius),1.0f/m_Radius)*pivot;
// 			vertexList.Add(new Vector2(dx+x,dy+y));

// 			x += 0.1f;
// 		}

// 		if(float.IsNaN(vertexList.Last().y))
// 		{
// 			vertexList.RemoveAt(vertexList.Count-1);
// 		}

// 		while(y > 0)
// 		{
// 			x = Mathf.Pow(1.0f-Mathf.Pow(y/pivot,m_Radius),1.0f/m_Radius)*pivot;
// 			vertexList.Add(new Vector2(dx+x,dy+y));

// 			y -= 0.1f;
// 		}

// 		vertexList.Add(new Vector2(width,0.0f));

// 		for(var i=1;i<vertexList.Count-1;i++)
// 		{
// 			if(vertexList[i].x < vertexList[i].y)
// 			{
// 				if(vertexList[i-1].y < vertexList[i].y)
// 				{
// 					vertexList.RemoveAt(i);
// 					i -= 1;
// 				}
// 			}
// 			else
// 			{
// 				if(vertexList[i].x < vertexList[i-1].x)
// 				{
// 					vertexList.RemoveAt(i);
// 					i -= 1;
// 				}
// 			}
// 		}

// 		vertexList.AddRange(vertexList.AsEnumerable().Reverse().Select(t => new Vector2(+t.x,-t.y)));
// 		vertexList.AddRange(vertexList.AsEnumerable().Reverse().Select(t => new Vector2(-t.x,+t.y)));

// 		_vertexHelper.Clear();

// 		for(var i=0;i<vertexList.Count-1;i++)
// 		{
// 			_vertexHelper.AddVert(vertexList[i+0],color,Vector2.zero);
// 			_vertexHelper.AddVert(vertexList[i+1],color,Vector2.zero);
// 			_vertexHelper.AddVert(Vector2.zero,color,Vector2.zero);

// 			_vertexHelper.AddTriangle(i*3,i*3+1,i*3+2);
// 		}
// 	}
// }