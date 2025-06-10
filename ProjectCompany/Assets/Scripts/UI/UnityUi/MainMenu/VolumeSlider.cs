using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Settings settings;
    [SerializeField] private Slider volumeSlider;

    public AK.Wwise.RTPC Wwise_RTPC_MasterVolume;
    public void Awake()
    {
        volumeSlider.value = settings.Volume;
    }
    public void onSliderValueChanged()
    {
        settings.Volume = (int)volumeSlider.value;
        Wwise_RTPC_MasterVolume.SetGlobalValue(settings.Volume);
    }
}
