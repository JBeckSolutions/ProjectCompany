using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private GameObject pauseUI;
    private GameObject playerInterface;
    private GameObject deathUI;

    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<UIManager>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void ShowPauseUI()
    {
        if (pauseUI != null)
        {
            pauseUI.GetComponent<PauseUI>().Open();
        }
    }

    public void HidePauseUI()
    {
        if (pauseUI != null)
        {
            pauseUI.GetComponent<PauseUI>().Close();
        }
    }

    public void ShowPlayerInterface()
    {
        if (playerInterface != null)
        {
            playerInterface.GetComponent<Interface>().Open();
        }
    }

    public void HidePlayerInterface()
    {
        if (playerInterface != null)
        {
            playerInterface.GetComponent<Interface>().Close();
        }
    }

    public void ShowDeathUI()
    {
        if (deathUI != null)
        {
            deathUI.GetComponent<DeathUI>().Open();
        }
    }

    public void HideDeathUI()
    {
        if (deathUI != null)
        {
            deathUI.GetComponent<DeathUI>().Close();
        }
    }

    public void AssignUIDocuments(GameObject pause, GameObject playerInterface, GameObject death)
    {
        pauseUI = pause;
        this.playerInterface = playerInterface;
        deathUI = death;
    }
}
