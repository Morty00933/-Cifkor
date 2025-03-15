using DogApp.Services;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<HttpService>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<RequestQueueService>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<WeatherController>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<BreedsController>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}