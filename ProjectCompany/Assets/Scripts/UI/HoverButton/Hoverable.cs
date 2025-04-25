using System;
using UnityEngine.UIElements;

public class Hoverable : PointerManipulator
{
    public event Action hovered;

    public Hoverable(Action hoverHandler)
    {
        if (hoverHandler != null)
        {
            hovered += hoverHandler;
        }
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
    }

    private void OnMouseEnter(MouseEnterEvent evt)
    {
        hovered?.Invoke();
    }
}

