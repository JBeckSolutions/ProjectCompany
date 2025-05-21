using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Settings")]
public class Settings : ScriptableObject
{
    [SerializeField] VolumeProfile volumeProfile;

    [SerializeField] private int _volume = 100;
    [SerializeField] private float _mouseSensitivity = 0.1f;
    [SerializeField] private Vector4 _gamma = new Vector4 (1, 1, 1, 0);
    public int Volume
    {
        get => _volume;
        set
        {
            _volume = Mathf.Clamp(value, 0, 100);
        }
    }

    public float MouseSensitivity
    {
        get => _mouseSensitivity;
        set => _mouseSensitivity = value;
    }

    public float Gamma
    {
        get => _gamma.w;
        set
        {
            _gamma = new Vector4(1, 1, 1, value);

            // Get the LiftGammaGain component from the VolumeProfile
            if (volumeProfile.TryGet<LiftGammaGain>(out LiftGammaGain component))
            {
                // Set the gamma value for the component
                component.gamma.value = _gamma;
                Debug.Log("Gamma value updated to: " + _gamma); // Add a log to confirm it's changing
            }
            else
            {
                Debug.LogWarning("LiftGammaGain component not found in VolumeProfile.");
            }
        }
    }

}
