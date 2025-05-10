using UnityEngine;
using UnityEngine.UI;

public class InventoryTile : MonoBehaviour
{
    public Image ItemImage;
    public Image Selected;

    public void ResetItemImage()
    {
        ItemImage.sprite = null;
        ItemImage.color = new Color(1, 1, 1, 0);
    }

    public void SetItemImage(Sprite imageToSet, Color? color = null)
    {
        Color colorToSet = color ?? new Color(1, 1, 1, 1);
        ItemImage.sprite = imageToSet;
        ItemImage.color = colorToSet;
    }
}
