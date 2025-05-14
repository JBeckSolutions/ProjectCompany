using System;
using UnityEngine;

public class CustomInitializer : MonoBehaviour
{
    public AK.Wwise.RTPC Wwise_RTPC_MasterVolume;
    public Settings SettingsScriptableObject;
    public void Awake()
    {
        Wwise_RTPC_MasterVolume.SetGlobalValue(SettingsScriptableObject.Volume);
    }
}
