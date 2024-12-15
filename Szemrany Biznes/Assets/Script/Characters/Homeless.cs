using UnityEngine;
using UnityEngine.AI;

public class Homeless : Character
{
    public override void Greetings()
    {
        Debug.Log("Homeless");
    }

    public override void OnPlayerStepped(TileScript tile)
    {
        int number = Random.Range(0, 100);
        Debug.Log("Homeless "+number);
        if (number > 70)
        {
            int coinsAmount = (int)(tile.coinsAmount.Value * ((float)number / 100));
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(coinsAmount, playerScript.playerIndex, 2, true, true);
            tile.RemoveCoinsServerRpc(coinsAmount);
            Debug.Log("coins: " + coinsAmount);
        }
    }
}
