using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace DogApp.Services
{
    public class RequestQueueService : MonoBehaviour
    {
        [Inject] private HttpService httpService;
        private Queue<(string url, Action<string> onSuccess, Action<string> onError, string tag)> requestQueue;
        private HashSet<string> activeTags;

        private void Awake()
        {
            requestQueue = new Queue<(string, Action<string>, Action<string>, string)>();
            activeTags = new HashSet<string>();
        }

        public void AddRequest(string url, Action<string> onSuccess, Action<string> onError, string tag)
        {
            requestQueue.Enqueue((url, onSuccess, onError, tag));
            if (requestQueue.Count == 1)
            {
                ProcessQueue();
            }
        }

        public void CancelRequestsByTag(string tag)
        {
            activeTags.Remove(tag);
            var newQueue = new Queue<(string, Action<string>, Action<string>, string)>();
            while (requestQueue.Count > 0)
            {
                var request = requestQueue.Dequeue();
                if (request.tag != tag)
                {
                    newQueue.Enqueue(request);
                }
            }
            requestQueue = newQueue;
        }

        private async void ProcessQueue()
        {
            while (requestQueue.Count > 0)
            {
                var (url, onSuccess, onError, tag) = requestQueue.Dequeue();
                activeTags.Add(tag);
                var (success, json, error) = await httpService.SendRequest(url);
                if (activeTags.Contains(tag))
                {
                    if (success) onSuccess?.Invoke(json);
                    else onError?.Invoke(error);
                }
                activeTags.Remove(tag);
            }
        }
    }
}