using UnityEngine;
using UnityEngine.EventSystems;

public class UiHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AK.Wwise.Event WwiseEvent_MainMenu_UI_Button_Hover;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered UI element.");
        WwiseEvent_MainMenu_UI_Button_Hover.Post(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse exited UI element.");
    }
}
