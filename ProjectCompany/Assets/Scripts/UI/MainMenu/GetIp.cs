using System.Net.Sockets;
using System.Net;
using UnityEngine;
using TMPro;

public class GetIp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    void Start()
    {
        Text.text += GetLocalIPAddress();
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
