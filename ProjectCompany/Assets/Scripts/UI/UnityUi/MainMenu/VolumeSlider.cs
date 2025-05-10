using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Settings settings;
    [SerializeField] private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider.value = settings.Volume;
    }
    public void onSliderValueChanged()
    {
        settings.Volume = (int)volumeSlider.value;
    }
}
