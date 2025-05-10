using UnityEngine;
using UnityEngine.EventSystems;

public class UiHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered UI element.");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse exited UI element.");
    }
}
