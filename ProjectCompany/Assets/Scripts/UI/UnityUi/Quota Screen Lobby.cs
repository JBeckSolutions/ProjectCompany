using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class QuotaScreenLobby : NetworkBehaviour
{
    [SerializeField] private TMP_Text quotaText;

    public override void OnNetworkSpawn()
    {
        StartCoroutine(SetText());
    }

    private IEnumerator SetText()
    {
        while (GameManager.Singelton == null || GameManager.Singelton.Quota.Value == 0 || GameManager.Singelton.Quota.Value.ToString() == quotaText.text)
        {
            yield return null;
        }

        yield return new WaitForSeconds(5f);

        quotaText.text = GameManager.Singelton.Quota.Value.ToString();
    }
}
