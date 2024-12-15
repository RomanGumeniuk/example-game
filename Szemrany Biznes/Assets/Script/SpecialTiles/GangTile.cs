using UnityEngine;

public class GangTile : Tile
{
    public GangTile(TileScript tileScript)
    {
        this.tileScript = tileScript;
    }

    public override void OnPlayerStepped()
    {
        if(tileScript.townLevel.Value==-1)
        {
            tileScript.Buy(PlayerScript.LocalInstance.amountOfMoney.Value, tileScript.townCostToBuy[0]);
            return;
        }
        if (PlayerScript.LocalInstance.playerIndex == tileScript.ownerId.Value)
        {
            int amount = 0;
            foreach(TileScript tile in tileScript.AllTownsToGetMonopol)
            {
                if(tile.ownerId.Value == tileScript.ownerId.Value) amount++;
            }
            if (amount == 4) PlayerScript.LocalInstance.ShowTownDamageTab();
            else GameUIScript.OnNextPlayerTurn.Invoke();
            return;
        }
        tileScript.Pay(PlayerScript.LocalInstance.amountOfMoney.Value, CalculatePayAmount());
    }

    public int CalculatePayAmount()
    {
        int multiplier = 0;
        for (int i = 0; i < tileScript.AllTownsToGetMonopol.Count; i++)
        {
            if (tileScript.AllTownsToGetMonopol[i].ownerId.Value == tileScript.ownerId.Value) multiplier++;
        }
        //Debug.Log("a" + tileScript.townCostToPay[0] + (multiplier * tileScript.amountMoneyOnPlayerStep) + " " + multiplier);
        return tileScript.townCostToPay[0] + (multiplier * tileScript.amountMoneyOnPlayerStep);
    }


    public override int GetPayAmount()
    {
        if (tileScript.destroyPercentage.Value > 0) return 0;
        return CalculatePayAmount();
    }

    public override int CaluculatePropertyValue(int start = 0, int stop = -1)
    {
        return tileScript.townCostToBuy[0];
    }
}
