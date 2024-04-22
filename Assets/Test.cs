using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    private async void Start()
    {
        Debug.Log("Start");
        
        await Test1(UniTask.Defer(Test2));
    }

    private async UniTask Test1(UniTask task)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

        await task;
    }

    private async UniTask Test2()
    {
        Debug.Log("Test");
        
        await UniTask.Yield();
    }
}
