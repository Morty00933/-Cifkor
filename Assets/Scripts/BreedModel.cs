using System;
using System.Collections.Generic;

namespace DogApp.Models
{
    [Serializable]
    public class BreedModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [Serializable]
    public class BreedData
    {
        public string id;
        public Attributes attributes;
    }

    [Serializable]
    public class Attributes
    {
        public string name;
        public string description;
    }

    [Serializable]
    public class WrappedBreedData
    {
        public List<BreedData> data;
    }
}