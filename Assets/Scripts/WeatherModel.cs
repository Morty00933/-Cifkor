// Assets/Scripts/Models/WeatherModel.cs
using System;

namespace DogApp.Models
{
    [Serializable]
    public class WeatherModel
    {
        public float Temperature { get; set; }
        public string IconUrl { get; set; }
    }
}