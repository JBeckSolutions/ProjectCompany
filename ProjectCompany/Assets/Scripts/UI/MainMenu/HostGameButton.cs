using System.Net.Sockets;
using System.Net;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UTP;

public class HostGameButton : MonoBehaviour
{
    public void StartHost()
    {
        var transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        transport.SetConnectionData("0.0.0.0", 7777); // Bind to all network interfaces

        NetworkManager.Singleton.StartHost();
        Debug.Log("Host Started");
    }

    public static string GetLocalIPAddress()
    {
        string localIP = "Not found";
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
}
