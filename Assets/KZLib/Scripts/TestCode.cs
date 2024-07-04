using System;
using System.Collections;
using System.Collections.Generic;
using MemoryPack;
using R3;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    private void Start()
    {
        // 1초마다 콘솔에 메시지 출력
        Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => Debug.Log("1초 경과")); // this는 MonoBehaviour로서 Dispose를 자동으로 관리

        Test2();
    }

    void Test2()
    {
        // 데이터 생성
        var playerData = new PlayerData
        {
            Name = "Player1",
            Level = 10,
            Health = 100.0f
        };

        // 직렬화
        var serializedData = MemoryPackSerializer.Serialize(playerData);

        Debug.Log($"Serialized Data: {BitConverter.ToString(serializedData)}");

        // 역직렬화
        var deserializedData = MemoryPackSerializer.Deserialize<PlayerData>(serializedData);
        Debug.Log($"Deserialized Data: Name={deserializedData.Name}, Level={deserializedData.Level}, Health={deserializedData.Health}");
    }
}

[MemoryPackable]
public partial class PlayerData
{
    public string Name { get; set; }
    public int Level { get; set; }
    public float Health { get; set; }
}