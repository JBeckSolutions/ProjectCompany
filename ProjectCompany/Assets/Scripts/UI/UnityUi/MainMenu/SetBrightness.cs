using UnityEngine;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class SetBrightness : MonoBehaviour
{
    [SerializeField] private Settings settings;
    void Start()
    {
        settings.Gamma = settings.Gamma;
    }
}
