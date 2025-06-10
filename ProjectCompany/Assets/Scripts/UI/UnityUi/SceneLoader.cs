using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// The code of the this script was written by: Beck Jonas
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [SerializeField] string SceneToLoad;
    [SerializeField] private List<GameObject> ObjectsToDestroy;
    [SerializeField] GameObject parent;

    public void LoadScene()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        DestroyDontDestroyOnLoadObjectsInScene();
        Debug.Log("Loading Scene " + SceneToLoad);
        SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Single);
        Destroy(parent);
    }

    void DestroyDontDestroyOnLoadObjectsInScene()
    {
        foreach (var item in ObjectsToDestroy)
        {
            Destroy(item);
        }
    }
}
