using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DogApp.Services
{
    public class HttpService : MonoBehaviour
    {
        public async Task<(bool success, string json, string error)> SendRequest(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                try
                {
                    await request.SendWebRequest();
                    return request.result == UnityWebRequest.Result.Success
                        ? (true, request.downloadHandler.text, null)
                        : (false, null, request.error);
                }
                catch (System.Exception e)
                {
                    return (false, null, e.Message);
                }
            }
        }
    }
}