using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpecialTile : Tile
{
    enum Type
    { 
        FieldAirport,
        UnmarkedTrucks,
        PimpCar

    }
    private Type type;

    public SpecialTile(TileScript tileScript)
    {
        this.tileScript = tileScript;
        type = (Type)tileScript.amountMoneyOnPlayerStep;
    }

    public override void OnPlayerStepped()
    {
        if (tileScript.townLevel.Value == -1)
        {
            tileScript.Buy(PlayerScript.LocalInstance.amountOfMoney.Value, tileScript.townCostToBuy[0]);
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
        List<TileScript> ownerProperties = NetworkManager.Singleton.ConnectedClientsList[tileScript.ownerId.Value].PlayerObject.GetComponent<PlayerScript>().tilesThatPlayerOwnList;
        int calculatedAmount = tileScript.townCostToPay[0];
        switch (type)
        {
            case Type.FieldAirport:
                for (int i = 0;i<ownerProperties.Count;i++)
                {
                    if (ownerProperties[i].propertyType != PropertyType.Drugs) continue;
                    calculatedAmount += ownerProperties[i].townCostToPay[ownerProperties[i].townLevel.Value]/10;
                }
                return calculatedAmount;
            case Type.UnmarkedTrucks:
                for (int i = 0; i < ownerProperties.Count; i++)
                {
                    if (ownerProperties[i].propertyType != PropertyType.Alcohol) continue;
                    calculatedAmount += ownerProperties[i].townCostToPay[ownerProperties[i].townLevel.Value]/10;
                }
                return calculatedAmount;
            case Type.PimpCar:
                for (int i = 0; i < ownerProperties.Count; i++)
                {
                    if (ownerProperties[i].propertyType != PropertyType.Prostitution) continue;
                    calculatedAmount += ownerProperties[i].townCostToPay[ownerProperties[i].townLevel.Value]/10;
                }
                return calculatedAmount;
        }
        return calculatedAmount;
    }

    public override int CaluculatePropertyValue(int start = 0, int stop = -1)
    {
        return tileScript.townCostToBuy[0];
    }

    public override int GetPayAmount()
    {
        return CalculatePayAmount();
    }
}
