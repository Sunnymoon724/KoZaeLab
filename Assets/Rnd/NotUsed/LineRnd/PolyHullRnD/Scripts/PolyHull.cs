using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PolyHull
{
    public static Vector3[] GetConvexHull(List<Vector3> _pointList)
    {
        return ConvertLineToVector(WrapConvexHull(GetPolyPointList(_pointList)));
    }

    public static Vector3[] GetConcaveHull(List<Vector3> _pointList)
    {
        return ConvertLineToVector(WrapConcaveHull(GetPolyPointList(_pointList)));
    }

    static List<PolyPoint> GetPolyPointList(List<Vector3> _pointList)
    {
        return _pointList.ConvertAll(p=>new PolyPoint(p.x,p.y,_pointList.FindIndex(i=>i==p)));
    }

    #region Convert
    static Vector3[] ConvertLineToVector(List<PolyLine> _lineList)
    {
        var pointList = new List<Vector3>();

        // 시작점을 넣고 라인리스트에서 지움
        var pivot = _lineList[0];

        pointList.Add(pivot.GetPoint(0));

        var poly = pivot.PointArray[1];

        _lineList.Remove(pivot);

        while(_lineList.Count != 0)
        {
            pivot = _lineList.Find(x=>x.IsContainsInArray(poly));

            if(pivot.PointArray[1] == poly)
            {
                pivot.SwapPolyPointArray();
            }

            pointList.Add(pivot.GetPoint(0));

            poly = pivot.PointArray[1];

            _lineList.Remove(pivot);
        }

        return pointList.ToArray();
    }
    #endregion

    #region ConvexHull
    static List<PolyLine> WrapConvexHull(List<PolyPoint> _pointList)
    {
        var convexHullList = new List<PolyPoint>();
        PolyPoint? point = null;

        foreach (var data in _pointList)
        {
            if(point.HasValue)
            {
                if(point.Value.y > data.y)
                {
                    point = data;
                }
            }
            else
            {
                point = data;
            }
        }

        var orderList = new List<PolyPoint>();

        foreach (var data in _pointList)
        {
            if (point.Value != data)
            {
                orderList.Add(data);
            }
        }

        orderList = MergeSort(point.Value,orderList);

        convexHullList.Add(point.Value);
        convexHullList.Add(orderList[0]);
        convexHullList.Add(orderList[1]);
        orderList.RemoveAt(0);
        orderList.RemoveAt(0);

        foreach (PolyPoint value in orderList)
        {
            KeepLeft(convexHullList,value);
        }

        List<PolyLine> polyLineList = new List<PolyLine>();

        for (int i=0;i< convexHullList.Count-1;i++)
        {
            polyLineList.Add(new PolyLine(convexHullList[i],convexHullList[i+1]));
        }

        polyLineList.Add(new PolyLine(convexHullList[0],convexHullList[convexHullList.Count-1]));
        
        return polyLineList;
    }
    #endregion

    #region ConcaveHull
    static List<PolyLine> WrapConcaveHull(List<PolyPoint> _pointList)
    {
        var convexHullList = WrapConvexHull(_pointList);

        foreach (var line in convexHullList)
        {
            foreach (var point in line.PointArray)
            {
                var data = _pointList.Find(x=>x.idx==point.idx);

                if (data != null)
                {
                    _pointList.Remove(data);
                }
            }
        }

        var concaveHullList = new List<PolyLine>(convexHullList.OrderByDescending(a=>a.GetDistance()));
        var concavity = -1.0f;
        var modified = false;

        do
        {
            modified = false;
            int count = 0;
            int listLength = concaveHullList.Count;

            while (count<listLength)
            {
                PolyLine edge = concaveHullList[0];
                concaveHullList.RemoveAt(0);
                var aux = new List<PolyLine>();

                if (edge.IsChecked == false)
                {
                    aux.AddRange(SetConcave(edge,_pointList,concaveHullList, concavity, 40));

                    modified = modified || (aux.Count>1);

                    if (aux.Count > 1)
                    {
                        foreach (var point in aux[0].PointArray)
                        {
                            var data = _pointList.Find(x=>x.idx==point.idx);

                            if (data != null)
                            {
                                _pointList.Remove(data);
                            }
                        }
                    }
                    else
                    {
                        aux[0].Checked();
                    }
                }
                else
                {
                    aux.Add(edge);
                }

                concaveHullList.AddRange(aux);
                count++;
            }

            concaveHullList = concaveHullList.OrderByDescending(a=>a.GetDistance()).ToList();
            listLength = concaveHullList.Count;

        } while (modified);


        return concaveHullList;
    }

    static List<PolyLine> SetConcave(PolyLine _line, List<PolyPoint> _insideList, List<PolyLine> _concaveList, float concavity, int _factor)
    {
        var nearPointList = new List<PolyPoint>();

        for(var i=0;i<2;i++)
        {
            if(nearPointList.Count != 0)
            {
                break;
            }

            foreach (var point in _insideList)
            {
                //Not part of the line
                if ((point.x==_line.PointArray[0].x && point.y==_line.PointArray[0].y || point.x==_line.PointArray[1].x && point.y==_line.PointArray[1].y) == false)
                {                    
                    var boundary = GetBoundary(_line,_factor);

                    var posX = Mathf.FloorToInt(point.x/_factor);
                    var posY = Mathf.FloorToInt(point.y/_factor);

                    //Inside the boundary
                    if (posX>=boundary[0] && posX<=boundary[2] && posY>=boundary[1] && posY<=boundary[3])
                    {
                        nearPointList.Add(point);
                    }
                }
            }

            _factor = _factor*4/3;
        }

        var concaveList = new List<PolyLine>();
        var sumCos = -2.0f;
        PolyPoint? middlePoint = null;
        for(var i=0;i<nearPointList.Count;i++)
        {
            bool isIntersection = false;

            var cos1 = GetCos(nearPointList[i],_line.PointArray[0],_line.PointArray[1]);
            var cos2 = GetCos(nearPointList[i],_line.PointArray[1],_line.PointArray[0]);

            if (cos1+cos2>=sumCos && (cos1>concavity && cos2>concavity))
            {
                int count = 0;

                while (isIntersection == false && count<_concaveList.Count)
                {
                    isIntersection = (LineIntersection(_concaveList[count],new PolyLine(nearPointList[i],_line.PointArray[0])) || (LineIntersection(_concaveList[count],new PolyLine(nearPointList[i],_line.PointArray[1]))));

                    count++;
                }

                if (isIntersection == false)
                {
                    // Prevents from getting sharp angles between middlepoints
                    PolyPoint[] nearNodes = GetHullNearbyNodes(_line,_concaveList);

                    if ( (GetCos(nearPointList[i], nearNodes[0],_line.PointArray[0])<-concavity) && (GetCos(nearPointList[i], nearNodes[1],_line.PointArray[1])<-concavity))
                    {
                        // Prevents inner tangent lines to the concave hull
                        if ((TangentToHull(_line,nearPointList[i],cos1,cos2,_concaveList)) == false)
                        {
                            sumCos = cos1 + cos2;
                            middlePoint = nearPointList[i];
                        }
                    }
                }
            }
        }


        if(middlePoint.HasValue)
        {
            concaveList.Add(new PolyLine(middlePoint.Value,_line.PointArray[0]));
            concaveList.Add(new PolyLine(middlePoint.Value,_line.PointArray[1]));
        }
        else
        {
            concaveList.Add(_line);
        }

        return concaveList;
    }

    static int[] GetBoundary(PolyLine _line,int _scaleFactor)
    {
        int min_x = Mathf.FloorToInt(Math.Min(_line.PointArray[0].x,_line.PointArray[1].x) / _scaleFactor);
        int min_y = Mathf.FloorToInt(Math.Min(_line.PointArray[0].y,_line.PointArray[1].y) / _scaleFactor);

        int max_x = Mathf.FloorToInt(Math.Max(_line.PointArray[0].x,_line.PointArray[1].x) / _scaleFactor);
        int max_y = Mathf.FloorToInt(Math.Max(_line.PointArray[0].y,_line.PointArray[1].y) / _scaleFactor);

        return new int[] { min_x,min_y,max_x,max_y };
    }

    static float GetCos(PolyPoint _pointA,PolyPoint _pointB,PolyPoint _pointC)
    {
        return Mathf.Cos(Vector2.Angle(new Vector2(_pointA.x-_pointC.x,_pointA.y-_pointC.y),new Vector2(_pointB.x-_pointC.x,_pointB.y-_pointC.y))*Mathf.Deg2Rad);
    }

    static bool TangentToHull(PolyLine _line,PolyPoint _point,float _cos1,float _cos2,List<PolyLine> _concaveList)
    {
        var isTangent = false;
        var searchedList = new List<int>();

        var length = GetDistance(_point,_line);

        for(var i=0;i<_concaveList.Count;i++)
        {
            for(var j=0;j<2;j++)
            {
                if (isTangent)
                {
                    break;
                }

                var point = _concaveList[i].PointArray[j];

                if (searchedList.Contains(point.idx) == false)
                {
                    if (point.idx != _line.PointArray[0].idx && point.idx != _line.PointArray[1].idx)
                    {
                        var cos1 = GetCos(point,_line.PointArray[0],_line.PointArray[1]);
                        var cos2 = GetCos(point,_line.PointArray[1],_line.PointArray[0]);

                        if (cos1 == _cos1 || cos2 == _cos2)
                        {
                            isTangent = GetDistance(point,_line) < length;
                        }
                    }
                }

                searchedList.Add(point.idx);
            }
        }

        return isTangent;
    }

    static float GetDistance(PolyPoint _point,PolyLine _line)
    {
        return Vector2.Distance(_point.ToVector(),_line.GetPoint(0)) + Vector2.Distance(_point.ToVector(),_line.GetPoint(1));
    }

    static PolyPoint[] GetHullNearbyNodes(PolyLine _line,List<PolyLine> _concaveList)
    {
        var nearPointArray = new PolyPoint[2];

        var leftIndex = _line.PointArray[0].idx;
        var rightIndex = _line.PointArray[1].idx;

        int nodesFound = 0;
        int line_count = 0;

        while(nodesFound < 2)
        {
            int opposite = 1;

            for(var i = 0;i < 2;i++)
            {
                int currectIndex = _concaveList[line_count].PointArray[i].idx;

                if(currectIndex == leftIndex && _concaveList[line_count].PointArray[opposite].idx != rightIndex)
                {
                    nearPointArray[0] = _concaveList[line_count].PointArray[opposite];
                    nodesFound++;
                }
                else if(currectIndex == rightIndex && _concaveList[line_count].PointArray[opposite].idx != leftIndex)
                {
                    nearPointArray[1] = _concaveList[line_count].PointArray[opposite];
                    nodesFound++;
                }

                opposite--;
            }

            line_count++;
        }

        return nearPointArray;
    }

    static bool LineIntersection(PolyLine _lineA, PolyLine _lineB)
    {
        if (Math.Max(_lineA.PointArray[0].x,_lineA.PointArray[1].x) < Math.Min(_lineB.PointArray[0].x,_lineB.PointArray[1].x))
        {
            return false;
        }

        float denominatorA = GetDenominator(_lineA);
        float denominatorB = GetDenominator(_lineB);

        if (denominatorA == denominatorB)
        {
            return false;
        }
        else if (denominatorA == Mathf.Infinity)
        {
            return VerticalIntersection(_lineA,_lineB);
        }
        else if (denominatorB == Mathf.Infinity)
        {
            return VerticalIntersection(_lineB,_lineA);
        }
        else
        {
            float slopeA = _lineA.PointArray[0].y-(denominatorA*_lineA.PointArray[0].x);
            float slopeB = _lineB.PointArray[0].y-(denominatorB*_lineB.PointArray[0].x);

           float X = Mathf.Round((slopeB-slopeA)/(denominatorA - denominatorB)*1000.0f)/1000.0f;

            if ((X <= (Mathf.Max(Mathf.Min(_lineA.PointArray[0].x,_lineA.PointArray[1].x), Mathf.Min(_lineB.PointArray[0].x,_lineB.PointArray[1].x)))) || (X >= (Mathf.Min(Mathf.Max(_lineA.PointArray[0].x,_lineA.PointArray[1].x),Mathf.Max(_lineB.PointArray[0].x,_lineB.PointArray[1].x)))))
            {
                return false; //Out of bound
            }
            else
            {
                return true;
            }
        }
    }

    static float GetDenominator(PolyLine _line)
    {
        var pointA = _line.GetPoint(0);
        var pointB = _line.GetPoint(1);

        return pointA.x != pointB.x ? (pointA.y - pointB.y) / (pointA.x - pointB.x) : Mathf.Infinity;            
    }

    static bool VerticalIntersection(PolyLine _lineA,PolyLine _lineB)
    {
        if ((_lineB.PointArray[0].x > _lineA.PointArray[0].x) && (_lineA.PointArray[0].x>_lineB.PointArray[1].x) || ((_lineB.PointArray[1].x>_lineA.PointArray[0].x) && (_lineA.PointArray[0].x>_lineB.PointArray[0].x)))
        {
            float intersection = ((_lineB.PointArray[1].y-_lineB.PointArray[0].y)*(_lineA.PointArray[0].x-_lineB.PointArray[0].x)/(_lineB.PointArray[1].x-_lineB.PointArray[0].x))+_lineB.PointArray[0].y;

            return ((_lineA.PointArray[0].y>intersection) && (intersection>_lineA.PointArray[1].y)) || ((_lineA.PointArray[1].y>intersection) && (intersection>_lineA.PointArray[0].y));
        }
        else
        {
            return false;
        }
    }
    #endregion











    //static bool LineIntersection(Vector2 _pointA, Vector2 _pointB, Vector2 _pointC, Vector2 _pointD)
    //{
    //    // 기울기에 필요한 분모를 구한다.
    //    float denominator = (_pointB.x-_pointA.x)*(_pointD.y-_pointC.y)-(_pointB.y-_pointA.y)*(_pointD.x-_pointC.x);

    //    // 분모가 0 이면 두 직선은 평행한 관계이다.
    //    if (denominator == 0.0f)
    //    {
    //        return false;
    //    }

    //    // 점들의 기울기를 구한다.
    //    float slopeA = ((_pointC.x-_pointA.x)*(_pointD.y-_pointC.y)-(_pointC.y-_pointA.y)*(_pointD.x-_pointC.x))/denominator;
    //    float slopeB = ((_pointC.x-_pointA.x)*(_pointB.y-_pointA.y)-(_pointC.y-_pointA.y)*(_pointB.x-_pointA.x))/denominator;

    //    if (slopeA < 0.0f || slopeA > 1.0f || slopeB < 0.0f || slopeB > 1.0f )
    //    {
    //        return false;
    //    }

    //    return true;
    //}

    

    //float FindVectorDirection(Vector2 _pointA,Vector2 _pointB, Vector2 _pointC)
    //{
    //    return (_pointA.x-_pointC.x)*(_pointB.y-_pointC.y)-(_pointA.y-_pointC.y)*(_pointB.x-_pointC.x);
    //}

    

    static List<PolyPoint> MergeSort(PolyPoint _point, List<PolyPoint> _pointList)
    {
        if (_pointList.Count == 1)
        {
            return _pointList;
        }

        var sortedPointList = new List<PolyPoint>();
        int middle = Mathf.FloorToInt(_pointList.Count/2);

        var leftList = _pointList.GetRange(0, middle);
        var rightList = _pointList.GetRange(middle,_pointList.Count-middle);

        leftList = MergeSort(_point,leftList);
        rightList = MergeSort(_point,rightList);

        var left = 0;
        var right = 0;

        for (var i=0;i<leftList.Count+rightList.Count;i++)
        {
            if (left==leftList.Count)
            {
                sortedPointList.Add(rightList[right]);
                right++;
            }
            else if (right==rightList.Count)
            {
                sortedPointList.Add(leftList[left]);
                left++;
            }
            else if (GetDegree(_point,leftList[left]) < GetDegree(_point,rightList[right]))
            {
                sortedPointList.Add(leftList[left]);
                left++;
            }
            else
            {
                sortedPointList.Add(rightList[right]);
                right++;
            }
        }

        return sortedPointList;
    }

    static float GetDegree(PolyPoint _pointA,PolyPoint _pointB)
    {
        return Mathf.Atan2(_pointB.y-_pointA.y, _pointB.x-_pointA.x)*Mathf.Rad2Deg;
    }

    static void KeepLeft(List<PolyPoint> _convexList, PolyPoint _point)
    {
        while(_convexList.Count > 1 && Turn(_convexList[_convexList.Count-2],_convexList[_convexList.Count-1],_point) != 1)
        {
            _convexList.RemoveAt(_convexList.Count-1);
        }
        if (_convexList.Count == 0 || _convexList[_convexList.Count-1] != _point)
        {
            _convexList.Add(_point);
        }
    }

    static int Turn(PolyPoint _pointA, PolyPoint _pointB, PolyPoint _pointC)
    {
        return ((_pointB.x-_pointA.x)*(_pointC.y-_pointA.y)-(_pointC.x-_pointA.x)*(_pointB.y-_pointA.y)).CompareTo(0);
    }
}

#region PolyLine
public class PolyLine
{
    //public bool isChecked;
    //public PolyPoint[] pointArray;

    public bool IsChecked { get; private set; }

    public PolyPoint[] PointArray { get; private set; }

    public PolyLine(PolyPoint _pointA,PolyPoint _pointB)
    {
        IsChecked = false;
        PointArray = new PolyPoint[2];
        PointArray[0] = _pointA;
        PointArray[1] = _pointB;
    }

    public PolyLine(Vector2 _pointA,Vector2 _pointB)
    {
        IsChecked = false;
        PointArray = new PolyPoint[2];
        PointArray[0] = new PolyPoint(_pointA);
        PointArray[1] = new PolyPoint(_pointB);
    }

    public Vector3 GetPoint(int _idx)
    {
        return new Vector3(PointArray[_idx].x,PointArray[_idx].y,0.0f);
    }

    public float GetDistance()
    {
        return Vector3.Distance(GetPoint(0),GetPoint(1));
    }

    public bool IsContainsInArray(PolyPoint _point)
    {
        return PointArray.Contains(_point);
    }

    public void Checked()
    {
        IsChecked = true;
    }

    public void SwapPolyPointArray()
    {
        Tools.Swap(ref PointArray[0],ref PointArray[1]);

        //PolyPoint tmp = pointArray[0];
        //pointArray[0] = pointArray[1];
        //pointArray[1] = tmp;
    }
}
#endregion


#region PolyPoint
public struct PolyPoint
{
    public int idx;
    public float x;
    public float y;

    public PolyPoint(float _x,float _y)
    {
        idx = 0;
        x = _x;
        y = _y;
    }

    public PolyPoint(Vector2 _point)
    {
        idx = 0;
        x = _point.x;
        y = _point.y;
    }

    public PolyPoint(float _x,float _y,int _id)
    {
        x = _x;
        y = _y;
        idx = _id;
    }

    public Vector3 ToVector()
    {
        return new Vector3(x,y,0.0f);
    }

    public static bool operator ==(PolyPoint _point1,PolyPoint _point2)
    {
        return Equal(_point1,_point2);
    }

    public static bool operator !=(PolyPoint _point1,PolyPoint _point2)
    {
        return !Equal(_point1,_point2);
    }

    static bool Equal(PolyPoint _point1,PolyPoint _point2)
    {
        return _point1.x == _point2.x && _point1.y == _point2.y && _point1.idx == _point2.idx;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
#endregion