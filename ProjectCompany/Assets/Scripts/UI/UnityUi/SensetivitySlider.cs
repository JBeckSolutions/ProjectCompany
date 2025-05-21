using UnityEngine;
using UnityEngine.UI;
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
