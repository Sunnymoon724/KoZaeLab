using System;
using System.Collections.Generic;
using UnityEngine;

namespace CI.HttpClient.Core
{
    public class Dispatcher : MonoBehaviour, IDispatcher
    {
        private static readonly Queue<System.Action> _queue = new Queue<System.Action>();
        private static readonly object _lock = new object();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Update()
        {
            lock (_lock)
            {
                while (_queue.Count > 0)
                {
                    _queue.Dequeue().Invoke();
                }
            }
        }

        public void Enqueue(System.Action action)
        {
            lock (_lock)
            {
                _queue.Enqueue(action);
            }
        }
    }
}