using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TabController : MonoBehaviour
{
    [SerializeField] private WeatherController weatherController;
    [SerializeField] private BreedsController breedsController;
    [SerializeField] private GameObject weatherTab;
    [SerializeField] private GameObject breedsTab;
    [SerializeField] private Button weatherButton;
    [SerializeField] private Button breedsButton;

    private void Start()
    {
        weatherButton.onClick.AddListener(ShowWeatherTab);
        breedsButton.onClick.AddListener(ShowBreedsTab);
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(2f);
        ShowWeatherTab();
    }

    private void ShowWeatherTab()
    {
        weatherTab.SetActive(true);
        breedsTab.SetActive(false);
        weatherController.Activate();
        breedsController.Deactivate();
    }

    private void ShowBreedsTab()
    {
        weatherTab.SetActive(false);
        breedsTab.SetActive(true);
        weatherController.Deactivate();
        breedsController.Activate();
    }
}