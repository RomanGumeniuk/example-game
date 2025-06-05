using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpecialTile : Tile
{
    public enum Type
    { 
        FieldAirport,
        UnmarkedTrucks,
        PimpCar

    }
    public Type type;

    public SpecialTile(TileScript tileScript)
    {
        this.tileScript = tileScript;
        type = (Type)tileScript.amountMoneyOnPlayerStep;
    }

    public override void OnPlayerStepped()
    {
        if (tileScript.townLevel.Value == -1)
        {
            tileScript.Buy(PlayerScript.LocalInstance.amountOfMoney.Value, tileScript.GetTownCostToBuyIndex(0));
            return;
        }
        if (PlayerScript.LocalInstance.playerIndex == tileScript.ownerId.Value)
        {
            base.OnPlayerStepped();
            return;
        }
        tileScript.Pay(PlayerScript.LocalInstance.amountOfMoney.Value, CalculatePayAmount());
    }

    public int CalculatePayAmount()
    {
        List<TileScript> ownerProperties = NetworkManager.Singleton.ConnectedClientsList[tileScript.ownerId.Value].PlayerObject.GetComponent<PlayerScript>().GetTilesThatPlayerOwnList();
        int calculatedAmount = tileScript.GetTownCostToPayIndex(0, tileScript.ownerId.Value);
        switch (type)
        {
            case Type.FieldAirport:
                for (int i = 0;i<ownerProperties.Count;i++)
                {
                    if (ownerProperties[i].propertyType != PropertyType.Drugs) continue;
                    calculatedAmount += ownerProperties[i].specialTileScript.GetPayAmount() /10;
                }
                return calculatedAmount;
            case Type.UnmarkedTrucks:
                for (int i = 0; i < ownerProperties.Count; i++)
                {
                    if (ownerProperties[i].propertyType != PropertyType.Alcohol) continue;
                    calculatedAmount += ownerProperties[i].specialTileScript.GetPayAmount() / 10;
                }
                return calculatedAmount;
            case Type.PimpCar:
                for (int i = 0; i < ownerProperties.Count; i++)
                {
                    if (ownerProperties[i].propertyType != PropertyType.Prostitution) continue;
                    calculatedAmount += ownerProperties[i].specialTileScript.GetPayAmount() /10;
                }
                return calculatedAmount;
        }
        return calculatedAmount;
    }

    public override int CaluculatePropertyValue(int start = 0, int stop = -1)
    {
        return tileScript.GetTownCostToBuyIndex(0, tileScript.ownerId.Value);
    }

    public override int GetPayAmount()
    {
        if (tileScript.destroyPercentage.Value > 0) return 0;
        return CalculatePayAmount();
    }

    public override void OnOwnerIDChanged(int prevValue, int newValue)
    {
        Debug.Log("owner id changed " + tileScript.name);
        if (newValue != -1) FindAndUpdateAllTiles(newValue);
        if (prevValue != -1) FindAndUpdateAllTiles(prevValue);
    }

    public override void OnTownLevelChanged(int prevValue, int newValue)
    {
        Debug.Log("town level changed " + tileScript.name);
        FindAndUpdateAllTiles(tileScript.ownerId.Value);
    }

    private void FindAndUpdateAllTiles(int ownerID)
    {
        foreach (TileScript tile in NetworkManager.Singleton.ConnectedClientsList[ownerID].PlayerObject.GetComponent<PlayerScript>().GetTilesThatPlayerOwnList())
        {
            switch (type)
            {
                case Type.PimpCar:
                    if (tile.propertyType == PropertyType.Prostitution) UpdateTextOnTiles(tile);
                    break;
                case Type.FieldAirport:
                    if (tile.propertyType == PropertyType.Drugs) UpdateTextOnTiles(tile);
                    break;
                case Type.UnmarkedTrucks:
                    if (tile.propertyType == PropertyType.Alcohol) UpdateTextOnTiles(tile);
                    break;

            }

        }
    }


    private  void UpdateTextOnTiles(TileScript tile)
    {
        Debug.Log("update" + tile.name);
        //await Awaitable.WaitForSecondsAsync(0.2f);
        tile.UpdateOwnerTextServerRpc();
    }
}
