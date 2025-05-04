namespace UI.HoverButton
{
    using System;
    using UnityEngine.UIElements;
    public class HoverButton : Button
    {
    
        public new class UxmlFactory : UxmlFactory<HoverButton, UxmlTraits> { } //deprecated and will be removed in a future version but since we will never upgrade the Unity version we dont care
    
    
        public new event Action clicked; // Weiterleitung des clicked-Events
        public event Action hovered;
        private Hoverable hoverable;

        public HoverButton() : base()
        {
            hoverable = new Hoverable(() => OnHover());
            this.AddManipulator(hoverable);

            base.clicked += this.OnClick;
        }
    

        private void OnClick()
        {
            clicked?.Invoke();
        }

        private void OnHover()
        {
            hovered?.Invoke();
        }
    }
}