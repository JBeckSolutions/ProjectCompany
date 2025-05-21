using UnityEngine;

public class SetBrightness : MonoBehaviour
{
    [SerializeField] private Settings settings;
    void Start()
    {
        settings.Gamma = settings.Gamma;
    }
}
