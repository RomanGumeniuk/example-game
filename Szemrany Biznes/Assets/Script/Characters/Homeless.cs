using System;
using UnityEngine;
[Serializable]
public class Homeless : Character
{
    public override void Greetings()
    {
        Debug.Log("Homeless");
        name = "Mietek";
        isWoman = false;
    }

    public override bool OnPlayerStepped(TileScript tile)
    {
        int number = UnityEngine.Random.Range(0, 100);
        Debug.Log("Homeless "+number);
        if (number > 70)
        {
            int coinsAmount = Mathf.CeilToInt(tile.coinsAmount.Value * ((float)number / 100));
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(coinsAmount, playerScript.playerIndex, 2, true, true);
            tile.RemoveCoinsServerRpc(coinsAmount);
            Debug.Log("coins: " + coinsAmount);
        }
        return false;
    }
}
