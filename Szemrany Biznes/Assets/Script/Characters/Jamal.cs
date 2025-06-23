using System;
using Unity.Netcode;
using UnityEngine;
[Serializable]
public class Jamal : Character
{

    public override void Greetings()
    {
        Debug.Log("Jamal");
        name = "Jamal";
        isWoman = false;
    }

    public override bool OnPlayerStepped(TileScript tile)
    {
        if (tile.tileType != TileType.TownTile && tile.tileType != TileType.SpecialTile && tile.tileType != TileType.GangTile) return false;
        Debug.Log("Jamalll");
        if (tile.ownerId.Value == playerScript.playerIndex || tile.ownerId.Value == -1) return false;
        int randomNumber = UnityEngine.Random.Range(1, 100);
        Debug.Log("Random number: " + randomNumber);
        if(randomNumber < 6)
        {
            tile.SetTownDamageServerRpc(10);
            Debug.Log("Town Destroyed " + tile.name);
        }
        return false;
    }

    public override void OnPlayerPassBy(TileScript tile)
    {
        int randomNumber;
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            PlayerScript player = client.PlayerObject.GetComponent<PlayerScript>();
            //Debug.Log("j " + player.playerIndex);
            if ((int)client.ClientId == playerScript.playerIndex) continue;
            if(client.PlayerObject.GetComponent<PlayerScript>().currentTileIndex != playerScript.currentTileIndex) continue;
            
            randomNumber = UnityEngine.Random.Range(1, 100);
            Debug.Log("Random number stolen: " + randomNumber);
            if (randomNumber > 10) continue;
            
            int stolenAmount = (int)(player.amountOfMoney.Value * ((float)randomNumber / 100));

            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(stolenAmount, player.playerIndex, 1, true, true);
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(stolenAmount, playerScript.playerIndex, 2, false, true);
            
            
            
        }
    }


}
