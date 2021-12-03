using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Triangulate : MonoBehaviour
{
    public LineRenderer outLine = null;
    public GameObject polygon = null;
    private Mesh polyMesh = null;
    public Transform pointGroup = null;

    void Start()
    {
        List<Vector2> pointList = new List<Vector2>();

        for(int i=0;i<pointGroup.childCount;i++)
        {
            pointList.Add(pointGroup.GetChild(i).position);
        }

        TriangulatePolygon(pointList.ToArray());

        outLine.positionCount = pointList.Count;
        outLine.SetPositions(System.Array.ConvertAll(pointList.ToArray(),vec2=>new Vector3(vec2.x,vec2.y)));
    }

    void TriangulatePolygon(Vector2[] _posArray)
    {
        polyMesh = polygon.GetComponent<MeshFilter>().mesh;

        int[] triangleArray = GetTriangleIndex(_posArray).ToArray();

        CreateTriangleGroup(triangleArray,_posArray);
    }

    void CreateTriangleGroup(int[] _triArray,Vector2[] _posArray)
    {
        polyMesh.vertices = System.Array.ConvertAll(_posArray,vec2 => new Vector3(vec2.x,vec2.y));
        
        polyMesh.triangles = _triArray;

        polyMesh.RecalculateNormals();
        polyMesh.RecalculateBounds();
    }

    int[] GetTriangleIndex(Vector2[] _pointArray)
    {
        List<int> triangleList = new List<int>();

        List<Vector2> pointList = _pointArray.ToList();
        List<Vector2> earList = new List<Vector2>();
        
        for (int i=0;i<pointList.Count;i++)
        {
            IsPointEar(i,pointList.ToArray(),earList);
        }

        while (true)
        {
            if (pointList.Count == 3)
            {
                AddTrianglePoint(new Vector2[] { pointList[0],pointList[1],pointList[2] },_pointArray,triangleList);
                break;
            }

            // 현재점과 그 앞뒤 점을 구한다.
            Vector2 nowPos = earList[0];
            Vector2 prevPos = pointList[ClampIndex(pointList.FindIndex(value=>value==nowPos)-1,pointList.Count)];
            Vector2 nextPos = pointList[ClampIndex(pointList.FindIndex(value=>value==nowPos)+1,pointList.Count)];

            // 그 점들로 삼각형을 만든다.
            AddTrianglePoint(new Vector2[] { nowPos,prevPos,nextPos },_pointArray,triangleList);

            earList.Remove(nowPos);

            pointList.Remove(nowPos);
            
            earList.Remove(prevPos);
            earList.Remove(nextPos);

            IsPointEar(pointList.FindIndex(value=>value==prevPos),pointList.ToArray(),earList);
            IsPointEar(pointList.FindIndex(value=>value==nextPos),pointList.ToArray(),earList);
        }

        return triangleList.ToArray();
    }

    bool IsOrientedClockwise(Vector2[] _posArray,int _index)
    {
        Vector2 pointA = _posArray[ClampIndex(_index-1,_posArray.Length)];
        Vector2 pointB = _posArray[_index];
        Vector2 pointC = _posArray[ClampIndex(_index+1,_posArray.Length)];
        
        return IsOrientedClockwise(pointA,pointB,pointC);
    }

    bool IsOrientedClockwise(Vector2 _pointA,Vector2 _pointB,Vector2 _pointC)
    {
        float determinant = _pointA.x*_pointB.y+_pointC.x*_pointA.y+_pointB.x*_pointC.y-_pointA.x*_pointC.y-_pointC.x*_pointB.y-_pointB.x*_pointA.y;

        if (determinant>0.0f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    int ClampIndex(int _index, int _maxSize)
    {
        return  ((_index%_maxSize)+_maxSize)%_maxSize;
    }

    void IsPointEar(int _index, Vector2[] _posArray,List<Vector2> _earList)
    {
        // 현재 점과 그 점의 앞뒤 점을 이용하여 
        Vector2 pointA = _posArray[ClampIndex(_index-1,_posArray.Length)];
        Vector2 pointB = _posArray[_index];
        Vector2 pointC = _posArray[ClampIndex(_index+1,_posArray.Length)];

        // 점들의 위치가 cw인지 확인한다.
        if (IsOrientedClockwise(pointA, pointB, pointC))
        {
            return;
        }
        else
        {
            // 해당점이 내부의 점인지 외부의 점인지 파악
            bool hasPointInside = false;

            for (int i=0;i<_posArray.Length;i++)
            {
                if (IsOrientedClockwise(_posArray,i))
                {
                    if (IsPointInTriangle(_posArray[i],_posArray,_index))
                    {
                        hasPointInside = true;

                        break;
                    }
                }
            }

            // 해당점이 외부의 점이므로 이어에 포함한다.
            if (hasPointInside == false)
            {
                _earList.Add(_posArray[_index]);
            }
        }
    }

    bool IsPointInTriangle(Vector2 _pointP,Vector2[] _posArray,int _index)
    {
        // 점 p가 삼각형 안에 있는지 파악.
        Vector2 pointA = _posArray[ClampIndex(_index-1,_posArray.Length)];
        Vector2 pointB = _posArray[_index];
        Vector2 pointC = _posArray[ClampIndex(_index+1,_posArray.Length)];

        float denominator = ((pointB.y-pointC.y)*(pointA.x-pointC.x)+(pointC.x-pointB.x)*(pointA.y-pointC.y));

        float a = ((pointB.y-pointC.y)*(_pointP.x-pointC.x)+(pointC.x-pointB.x)*(_pointP.y-pointC.y))/denominator;
        float b = ((pointC.y-pointA.y)*(_pointP.x-pointC.x)+(pointA.x-pointC.x)*(_pointP.y-pointC.y))/denominator;
        float c = 1.0f-a-b;

        if( (0.0f<a && a<1.0f) && (0.0f<b && b<1.0f) && (0.0f<c && c<1.0f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void AddTrianglePoint(Vector2[] _triangleArray,Vector2[] _pointArray,List<int> _triangleList)
    {
        if(IsOrientedClockwise(_triangleArray[0],_triangleArray[1],_triangleArray[2]))
        {
            _triangleList.Add(Array.FindIndex(_pointArray,value => value==_triangleArray[0]));
            _triangleList.Add(Array.FindIndex(_pointArray,value => value==_triangleArray[1]));
            _triangleList.Add(Array.FindIndex(_pointArray,value => value==_triangleArray[2]));
        }
        else
        {
            _triangleList.Add(Array.FindIndex(_pointArray,value => value==_triangleArray[0]));
            _triangleList.Add(Array.FindIndex(_pointArray,value => value==_triangleArray[2]));
            _triangleList.Add(Array.FindIndex(_pointArray,value => value==_triangleArray[1]));
        }
    }
}