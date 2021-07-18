using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConvexHull : MonoBehaviour
{
    [SerializeField]
    private Text infoText;

    [SerializeField]
    private Transform pointGroup;

    [SerializeField]
    private LineRenderer convexLine;

    private List<Vector3> pointList;

    private bool isAcive;

    private void Start()
    {
        isAcive = pointGroup != null && pointGroup.childCount >= 3;

        if(isAcive)
        {
            pointList = new List<Vector3>();

            UpdatePointList();

            infoText.text = "볼록 다각형 만들기";
        }
    }

    void Update ()
    {
        if(isAcive)
        {
            if(pointList.Count != pointGroup.childCount)
            {
                UpdatePointList();
            }

            UpdateConvexLine();
        }
    }

    void UpdatePointList()
    {
        isAcive = pointGroup != null && pointGroup.childCount >= 3;

        if(isAcive)
        {
            pointList.Clear();

            for(int i=0;i<pointGroup.childCount;i++)
            {
                pointList.Add(pointGroup.GetChild(i).position);
            }
        }

        infoText.text = isAcive ? "볼록 다각형 만들기" : "에러";
    }

    void UpdateConvexLine()
    {
        pointList = GetConvexHull(pointList);

        convexLine.positionCount = pointList.Count;
        convexLine.SetPositions(pointList.ToArray());
    }

    List<Vector3> GetConvexHull(List<Vector3> _pointList)
    {
        if(_pointList.Count == 3)
        {
            return _pointList;
        }

        var convexHull = new List<Vector3>();
        var startPos = _pointList[0];

        for(int i=1;i<_pointList.Count;i++)
        {
            var testPos = _pointList[i];

            if(testPos.x<startPos.x || (Mathf.Approximately(testPos.x,startPos.x) && testPos.y<startPos.y))
            {
                startPos = _pointList[i];
            }
        }

        convexHull.Add(startPos);
        _pointList.Remove(startPos);

        var pivot = convexHull[0];
        var colinearPointList = new List<Vector3>();

        int count = 0;

        while(true)
        {
            if( count==2 )
            {
                _pointList.Add(convexHull[0]);
            }

            var nextPoint = _pointList[Random.Range(0,_pointList.Count)];

            for(int i=0;i<_pointList.Count;i++)
            {
                if( _pointList[i].Equals(nextPoint) )
                {
                    continue;
                }

                var relation = IsAPointLeftOfVectorOrOnTheLine(pivot,nextPoint,_pointList[i]);

                if(relation<+0.00001f && relation>-0.00001f)
                {
                    colinearPointList.Add(_pointList[i]);
                }
                else if( relation<0.0f )
                {
                    nextPoint = _pointList[i];
                    colinearPointList.Clear();
                }
            }

            if( colinearPointList.Count>0 )
            {
                colinearPointList.Add(nextPoint);

                colinearPointList = colinearPointList.OrderBy(p=>Vector2.SqrMagnitude(new Vector2(p.x,p.y)-new Vector2(pivot.x,pivot.y))).ToList();

                convexHull.AddRange(colinearPointList);

                pivot = colinearPointList[colinearPointList.Count-1];

                for(int i=0;i<colinearPointList.Count;i++)
                {
                    _pointList.Remove(colinearPointList[i]);
                }

                colinearPointList.Clear();
            }
            else
            {
                convexHull.Add(nextPoint);
                _pointList.Remove(nextPoint);
                pivot = nextPoint;
            }
            if(pivot.Equals(convexHull[0]))
            {
                break;
            }

            count += 1;
        }

        return convexHull;
    }

    float IsAPointLeftOfVectorOrOnTheLine(Vector2 _pointA,Vector2 _pointB,Vector2 _pointC)
    {
        return (_pointA.x-_pointC.x)*(_pointB.y-_pointC.y)-(_pointA.y-_pointC.y)*(_pointB.x-_pointC.x);
    }
}
