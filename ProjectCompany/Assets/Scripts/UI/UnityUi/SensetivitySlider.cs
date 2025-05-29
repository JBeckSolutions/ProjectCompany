using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class SensetivitySlider : MonoBehaviour
{
    [SerializeField] private Settings settings;
    [SerializeField] private Slider sensitivitySlider;

    private void Start()
    {
        sensitivitySlider.value = settings.MouseSensitivity;
    }

    public void OnValueChange()
    {
        settings.MouseSensitivity = sensitivitySlider.value;
    }
}
