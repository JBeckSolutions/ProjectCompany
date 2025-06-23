using Unity.VisualScripting;
using UnityEngine;

public class WwiseAddWallToRoom : MonoBehaviour
{
    [Tooltip("The room to which the wall will be added.")]
    public AkRoom room;

    private void Start()
    {
        AddWallToRoom();
    }

    private void AddWallToRoom()
    {
        
    }
}
