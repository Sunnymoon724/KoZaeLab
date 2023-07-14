using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolyHullCon : MonoBehaviour
{
	private const int MIN_POINT_COUNT = 3;

	[SerializeField] private GameObject m_PivotPoint = null;

	[SerializeField] private Transform m_PointGroup = null;
	[SerializeField] private LineRenderer m_ConvexLine = null;
	[SerializeField] private LineRenderer m_ConcaveLine = null;

	[SerializeField] private Button m_AddButton = null;
	[SerializeField] private Button m_RemoveButton = null;

	private List<Vector2> m_PointList = new List<Vector2>();

	private Transform m_Target = null;

	private void Awake()
	{
		m_AddButton.SetOnClickListener(()=>
		{
			var point = Tools.CopyObject(m_PivotPoint,m_PointGroup);
			point.name = string.Format("Point_{0}",m_PointGroup.childCount);

			point.gameObject.SetActiveSelf(true);

			point.transform.localPosition = Vector3.zero;
		});

		m_RemoveButton.SetOnClickListener(()=>
		{
			var last = m_PointGroup.GetChild(m_PointGroup.childCount-1);

			Tools.DestroyObject(last.gameObject);
		});

		for(var i=0;i<m_PointGroup.childCount;i++)
		{
			var point = m_PointGroup.GetChild(i);

			point.name = string.Format("Point_{0}",i);

			point.gameObject.SetActiveSelf(true);
		}
	}

	private void Update()
	{
		if(m_PointGroup.childCount < MIN_POINT_COUNT)
		{
			m_RemoveButton.interactable = false;

			return;
		}

		m_RemoveButton.interactable = true;

		CheckClick();

		m_PointList.Clear();

		for(var i=0;i<m_PointGroup.childCount;i++)
		{
			var point = m_PointGroup.GetChild(i);

			point.name = string.Format("Point_{0}",i);

			m_PointList.Add(new Vector2(point.position.x,point.position.y));
		}

		SetConvexLine();
		SetConcaveLine();
	}

	private void SetConvexLine()
	{
		var pointArray = Array.ConvertAll(Tools.GetConvexHull(m_PointList.ToArray()),v=>new Vector3(v.x,v.y,0.0f));

		m_ConvexLine.positionCount = pointArray.Length;
		m_ConvexLine.SetPositions(pointArray);
	}

	private void SetConcaveLine()
	{
		var pointArray = Array.ConvertAll(Tools.GetConcaveHull(m_PointList.ToArray()),v=>new Vector3(v.x,v.y,0.0f));

		m_ConcaveLine.positionCount = pointArray.Length;
		m_ConcaveLine.SetPositions(pointArray);
	}

	private void CheckClick()
	{
		if(Input.GetMouseButtonDown(0))
		{
			var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var hit = Physics2D.Raycast(point,Vector2.zero,0.0f);

			if(hit.collider != null)
			{
				m_Target = hit.transform;
			}
		}
		else if(Input.GetMouseButton(0))
		{
			if(m_Target != null)
			{
				var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				m_Target.position = new Vector3(point.x,point.y,0.0f);
			}
		}
		else if(Input.GetMouseButtonUp(0))
		{
			m_Target = null;
		}
	}
}
