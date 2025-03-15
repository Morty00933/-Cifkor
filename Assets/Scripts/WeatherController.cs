using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DogApp.Models;
using DogApp.Services;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Zenject;
using Cysharp.Threading.Tasks;
using System;

public class WeatherController : MonoBehaviour
{
    [SerializeField] private Image weatherIcon;
    [SerializeField] private TMP_Text temperatureText;
    [Inject] private RequestQueueService requestQueue;

    private float timer = 0f;
    private bool isActive = false;

    public void Activate()
    {
        isActive = true;
        RequestWeather();
    }

    public void Deactivate()
    {
        isActive = false;
        requestQueue.CancelRequestsByTag("Weather");
    }

    private void Update()
    {
        if (!isActive) return;
        timer += Time.deltaTime;
        if (timer >= 5f)
        {
            RequestWeather();
            timer = 0f;
        }
    }

    private void RequestWeather()
    {
        requestQueue.AddRequest(
            "https://api.weather.gov/gridpoints/TOP/32,81/forecast",
            json => UpdateWeather(ParseWeather(json)),
            error => { },
            "Weather"
        );
    }

    private WeatherModel ParseWeather(string json)
    {
        var forecast = JsonUtility.FromJson<WeatherForecast>(json);
        var period = forecast?.properties?.periods?[0];
        return period != null
            ? new WeatherModel { Temperature = period.temperature, IconUrl = period.icon }
            : new WeatherModel { Temperature = 0f, IconUrl = "" };
    }

    private async void UpdateWeather(WeatherModel data)
    {
        temperatureText.text = $"Сегодня - {data.Temperature}F";
        if (!string.IsNullOrEmpty(data.IconUrl))
        {
            var texture = await LoadIcon(data.IconUrl);
            if (texture != null)
            {
                weatherIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
        }
    }

    private async Task<Texture2D> LoadIcon(string url)
    {
        using (var request = UnityWebRequestTexture.GetTexture(url))
        {
            request.SetRequestHeader("User-Agent", "Unity App/1.0");
            await request.SendWebRequest();
            return request.result == UnityWebRequest.Result.Success
                ? DownloadHandlerTexture.GetContent(request)
                : null;
        }
    }
}

[Serializable]
public class WeatherForecast { public WeatherProperties properties; }
[Serializable]
public class WeatherProperties { public System.Collections.Generic.List<WeatherPeriod> periods; }
[Serializable]
public class WeatherPeriod { public float temperature; public string icon; }