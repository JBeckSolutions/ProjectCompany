using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class BrightnessSlider : MonoBehaviour
{
    [SerializeField] private Settings settings;
    [SerializeField] private Slider brightnessSlider;

    private void Start()
    {
        brightnessSlider.value = settings.Gamma;
    }
    public void onSliderValueChanged()
    {
        settings.Gamma = brightnessSlider.value;
    }
}
