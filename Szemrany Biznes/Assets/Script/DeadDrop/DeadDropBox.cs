using Unity.Netcode;
using UnityEngine;

public class DeadDropBox : NetworkBehaviour
{
    int amountOfMoney = 0;
    public int index;
    private void Start()
    {
        WaitForInpact();
    }

    async void WaitForInpact()
    {
        Vector3 previousPosition;
        while (true)
        {
            previousPosition = transform.position;
            await Awaitable.WaitForSecondsAsync(0.1f);
            if(previousPosition == transform.position && transform.position.y <8)
                break;
        }
        transform.GetChild(0).gameObject.SetActive(false);
        if (!IsOwner) return;
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject.GetComponent<PlayerScript>().currentTileIndex != index) continue;
            OnPlayerClaimServerRpc((int)client.ClientId);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    public void OnPlayerClaimServerRpc(int playerIndex)
    {
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(amountOfMoney, playerIndex,2,true,true);
        Destroy(gameObject);
    }

    public void ChangeAmountOfMoney(float multiplier)
    {
        amountOfMoney = Mathf.CeilToInt(amountOfMoney*multiplier);
    }

    public void SetAmountOfMoney(int amountOfMoney)
    {
        this.amountOfMoney = amountOfMoney;
    }
}
