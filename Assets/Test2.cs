using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KZLib;
using KZLib.KZFiles.Converter;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

public class MapCoordinate
{
    public List<Vector3Int> coordinateList;

    // public Vector2 GetLocation()
    // {
    //     return new Vector2(Coordinate.x,Coordinate.y);
    // }
}

public class Test2 : MonoBehaviour
{
    private readonly CircularQueue<int> m_Queue = new(100);

    [Button("Check")]
    void Button()
    {
        m_Queue.Enqueue(1);
        m_Queue.Enqueue(1);

        if(m_Queue.All(x=>x == 1))
        {
            Debug.Log("All 1");
        }
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            Broadcaster.SendEvent(EventTag.ChangeLanguageOption);
        }
    }
}
