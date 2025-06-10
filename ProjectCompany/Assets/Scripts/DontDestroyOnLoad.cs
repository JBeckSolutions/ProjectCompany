using UnityEngine;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class DontDestroyOnLoad : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    
}
