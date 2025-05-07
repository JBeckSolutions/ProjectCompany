using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class JoinButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipText;

    public void StartClient()
    {
        ConnectToServer(ipText.text);
    }

    private void ConnectToServer(string ip , ushort port = 7777)
    {
        UnityTransport transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        transport.ConnectionData.Address = ip;
        transport.ConnectionData.Port = port;


        Debug.Log("Connecting to ip: " + ip + " and port: " + port.ToString());
        NetworkManager.Singleton.StartClient();
    }
}
