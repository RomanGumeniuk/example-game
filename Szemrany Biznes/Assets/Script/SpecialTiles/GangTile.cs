using Unity.Netcode;
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
            tileScript.Buy(PlayerScript.LocalInstance.amountOfMoney.Value, tileScript.GetTownCostToBuyIndex(0));
            return;
        }
        if (PlayerScript.LocalInstance.playerIndex == tileScript.ownerId.Value)
        {
            /*int amount = 0;
            foreach(TileScript tile in tileScript.AllTownsToGetMonopol)
            {
                if(tile.ownerId.Value == tileScript.ownerId.Value) amount++;
            }
            if (amount == 4)*/
            PlayerScript.LocalInstance.ShowTownDamageTab();
            //else GameUIScript.OnNextPlayerTurn.Invoke();
            return;
        }
        tileScript.Pay(PlayerScript.LocalInstance.amountOfMoney.Value, CalculatePayAmount());
    }

    public int CalculatePayAmount()
    {
        int multiplier = -1;
        for (int i = 0; i < tileScript.AllTownsToGetMonopol.Count; i++)
        {
            if (tileScript.AllTownsToGetMonopol[i].ownerId.Value == tileScript.ownerId.Value) multiplier++;
        }
        //Debug.Log("a" + tileScript.townCostToPay[0] + (multiplier * tileScript.amountMoneyOnPlayerStep) + " " + multiplier);
        Character character = NetworkManager.Singleton.ConnectedClientsList[tileScript.ownerId.Value].PlayerObject.GetComponent<PlayerScript>().character;

        return tileScript.GetTownCostToPayIndex(0, tileScript.ownerId.Value) + character.ApplyAllModifiersToSpecifiedTypeOfModificator((multiplier * tileScript.amountMoneyOnPlayerStep),TypeOfModificator.EarningMoneyFromPropertie,tileScript.propertyType);
    }


    public override int GetPayAmount()
    {
        if (tileScript.destroyPercentage.Value > 0) return 0;
        return CalculatePayAmount();
    }

    public override int CaluculatePropertyValue(int start = 0, int stop = -1)
    {
        Character character = NetworkManager.Singleton.ConnectedClientsList[tileScript.ownerId.Value].PlayerObject.GetComponent<PlayerScript>().character;
        return tileScript.GetTownCostToBuyIndex(0, character);
    }

    public override void OnTownUpgrade(int ownerID, int townLevel)
    {
        townLevel = -1;
        for (int i = 0; i < tileScript.AllTownsToGetMonopol.Count; i++)
        {
            if (tileScript.AllTownsToGetMonopol[i].ownerId.Value == ownerID) townLevel++;
        }
        foreach (TileScript tile in tileScript.AllTownsToGetMonopol)
        {
            if (tile.ownerId.Value == tileScript.ownerId.Value)
            {
                tile.SetTownLevelServerRpc(townLevel);
                tile.UpdateOwnerTextServerRpc(true);
            }
        }

    }
}
