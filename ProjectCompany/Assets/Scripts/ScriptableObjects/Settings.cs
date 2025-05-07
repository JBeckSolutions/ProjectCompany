using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Settings")]
public class Settings : ScriptableObject
{
    [SerializeField] private int _volume = 100;
    public int Volume
    {
        get => _volume;
        set
        {
            _volume = Mathf.Clamp(value, 0, 100);
        }
    }

}
