using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DogApp.Models;
using DogApp.Services;
using Zenject;
using System;

public class BreedsController : MonoBehaviour
{
    [SerializeField] private Transform breedListContainer;
    [SerializeField] private GameObject breedButtonPrefab;
    [SerializeField] private GameObject uiContainer;
    [Inject] private RequestQueueService requestQueue;

    private GameObject popup;
    private TMP_Text popupTitle;
    private TMP_Text popupDescription;
    private Button popupOkButton;
    private GameObject loader;

    private bool isActive = false;

    private void Awake()
    {
        popup = uiContainer.transform.Find("Popup").gameObject;
        popupTitle = popup.transform.Find("Title").GetComponent<TMP_Text>();
        popupDescription = popup.transform.Find("Description").GetComponent<TMP_Text>();
        popupOkButton = popup.transform.Find("OkButton").GetComponent<Button>();
        loader = uiContainer.transform.Find("Loader").gameObject;
    }

    public void Activate()
    {
        isActive = true;
        LoadBreeds();
    }

    public void Deactivate()
    {
        isActive = false;
        requestQueue.CancelRequestsByTag("Breeds");
        popup.SetActive(false);
        loader.SetActive(false);
    }

    private void LoadBreeds()
    {
        loader.SetActive(true);
        requestQueue.AddRequest(
            "https://dogapi.dog/api/v2/breeds",
            json => DisplayBreeds(ParseBreeds(json)),
            error => loader.SetActive(false),
            "Breeds"
        );
    }

    private List<BreedModel> ParseBreeds(string json)
    {
        var data = JsonUtility.FromJson<WrappedBreedData>(json);
        var breeds = new List<BreedModel>();
        if (data?.data == null) return breeds;

        foreach (var breed in data.data)
        {
            if (breed?.attributes != null)
            {
                breeds.Add(new BreedModel
                {
                    Id = breed.id,
                    Name = breed.attributes.name ?? "Unknown",
                    Description = breed.attributes.description ?? "No description"
                });
            }
        }
        return breeds;
    }

    private void DisplayBreeds(List<BreedModel> breeds)
    {
        loader.SetActive(false);
        foreach (Transform child in breedListContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < Mathf.Min(breeds.Count, 10); i++)
        {
            var breed = breeds[i];
            var button = Instantiate(breedButtonPrefab, breedListContainer);
            button.GetComponentInChildren<TMP_Text>().text = $"{i + 1} - {breed.Name}";
            button.GetComponent<Button>().onClick.AddListener(() => LoadBreedDetail(breed.Id));
        }
    }

    private void LoadBreedDetail(string breedId)
    {
        loader.SetActive(true);
        requestQueue.AddRequest(
            $"https://dogapi.dog/api/v2/breeds/{breedId}",
            json => ShowPopup(ParseBreedDetail(json)),
            error => loader.SetActive(false),
            "Breeds"
        );
    }

    private BreedModel ParseBreedDetail(string json)
    {
        var data = JsonUtility.FromJson<SingleBreedData>(json);
        return data?.data?.attributes != null
            ? new BreedModel
            {
                Id = data.data.id,
                Name = data.data.attributes.name ?? "Unknown",
                Description = data.data.attributes.description ?? "No description"
            }
            : new BreedModel { Name = "Error", Description = "Failed to load breed" };
    }

    private void ShowPopup(BreedModel data)
    {
        loader.SetActive(false);
        popup.SetActive(true);
        popupTitle.text = data.Name;
        popupDescription.text = data.Description;
        popupOkButton.onClick.RemoveAllListeners();
        popupOkButton.onClick.AddListener(() => popup.SetActive(false));
    }
}

[Serializable]
public class SingleBreedData { public BreedData data; }