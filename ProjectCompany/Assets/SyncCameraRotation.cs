using Unity.Netcode;
using UnityEngine;

public class SyncCameraRotation : NetworkBehaviour
{
    private NetworkVariable<float> cameraXRotation = new(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private float smoothedXRotation;

    [SerializeField] private float smoothSpeed = 10f;

    private void Update()
    {
        if (IsOwner)
        {
            float pitch = transform.localEulerAngles.x;
            if (pitch > 180f) pitch -= 360f;

            if (!Mathf.Approximately(cameraXRotation.Value, pitch))
            {
                cameraXRotation.Value = pitch;
            }
        }
        else
        {
            smoothedXRotation = Mathf.LerpAngle(smoothedXRotation, cameraXRotation.Value, Time.deltaTime * smoothSpeed);

            var currentEuler = transform.localEulerAngles;
            currentEuler.x = smoothedXRotation;
            transform.localEulerAngles = currentEuler;
        }
    }

    public override void OnNetworkSpawn()
    {
        smoothedXRotation = transform.localEulerAngles.x;
    }
}
