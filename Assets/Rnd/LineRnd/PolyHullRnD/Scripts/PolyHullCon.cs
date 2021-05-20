using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PolyHullCon : MonoBehaviour
{
    private enum LineState { All,Convex,Concave}

    [SerializeField] private Text stateText;

    [SerializeField] private Transform pointGroup;
    [SerializeField] private LineRenderer convexLine;
    [SerializeField] private LineRenderer concaveLine;

    [SerializeField] private Button deleteButton;

    private List<Transform> pointList;

    private LineState state;

    void Start()
    {
        state = LineState.All;

        pointList = new List<Transform>();

        for(var i=0;i<pointGroup.childCount;i++)
        {
            pointList.Add(pointGroup.GetChild(i));
        }
    }

    void Update()
    {
        deleteButton.interactable = pointGroup.childCount >= 4;

        if(convexLine.gameObject.activeSelf)
        {
            SetConvexLine();
        }

        if(concaveLine.gameObject.activeSelf)
        {
            SetConcaveLine();
        }
    }

    void SetConvexLine()
    {
        var array = PolyHull.GetConvexHull(pointList.ConvertAll(x=>x.position));

        convexLine.positionCount = array.Length;
        convexLine.SetPositions(array);
    }    
    
    void SetConcaveLine()
    {
        var array = PolyHull.GetConcaveHull(pointList.ConvertAll(x=>x.position));

        concaveLine.positionCount = array.Length;
        concaveLine.SetPositions(array);
    }

    public void OnChangeLine()
    {
        state = (LineState) Tools.LoopClamp((int)(state)+1,3);

        switch(state)
        {
            case LineState.All:
                stateText.text = "모두 선택";
                convexLine.gameObject.SetActive(true);
                concaveLine.gameObject.SetActive(true);
                break;
            case LineState.Concave:
                stateText.text = "오목 선택";
                convexLine.gameObject.SetActive(false);
                concaveLine.gameObject.SetActive(true);
                break;
            case LineState.Convex:
                stateText.text = "볼록 선택";
                convexLine.gameObject.SetActive(true);
                concaveLine.gameObject.SetActive(false);
                break;
            default:
                stateText.text = "";
                convexLine.gameObject.SetActive(false);
                concaveLine.gameObject.SetActive(false);
                break;
        }
    }

    public void OnAddPoint()
    {
        var first = pointList.First();

        var point = Instantiate(first.gameObject,pointGroup);

        point.name = $"Cube_{pointList.Count+1}";

        point.transform.position = Vector3.zero;

        pointList.Add(point.transform);
    }

    public void OnDeletePoint()
    {
        var last = pointList.Last();

        pointList.Remove(last);

        Destroy(last.gameObject);
    }
}
