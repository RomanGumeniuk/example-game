using Unity.Netcode;
using UnityEngine;

public class Jamal : Character
{

    public override void Greetings()
    {
        Debug.Log("Jamal");
    }

    public override void OnPlayerStepped(TileScript tile)
    {
        Debug.Log("Jamalll");
        int randomNumber;
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            PlayerScript player = client.PlayerObject.GetComponent<PlayerScript>();
            Debug.Log("j " + player.playerIndex);
            if ((int)client.ClientId == playerScript.playerIndex) continue;
            randomNumber = Random.Range(1, 100);
            Debug.Log("Random number stolen: " + randomNumber);
            if (randomNumber < 11)
            {
                int stolenAmount = (int)(player.amountOfMoney.Value * ((float)randomNumber / 100));
                
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(stolenAmount, player.playerIndex, 1, true, true);
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(stolenAmount, playerScript.playerIndex, 2, false, true);
            }
        }
        if (tile.ownerId.Value == playerScript.playerIndex) return;
        randomNumber = Random.Range(1, 100);
        Debug.Log("Random number: " + randomNumber);
        if(randomNumber < 6)
        {
            tile.SetTownDamageServerRpc(10);
            Debug.Log("Town Destroyed " + tile.name);
        }
    }
}
