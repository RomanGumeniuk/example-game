using System;
using Unity.Netcode;
using UnityEngine;
[Serializable]
public class BrothelKeeper : Character
{

    public override void Greetings()
    {
        Debug.Log("BrothelKeeper");
        name = "Burdel Mama";
        isWoman = true;
    }

    const float EARNINGS_MULTIPLIER = 0.1f;

    const float PAYMENT_PENALTIES_MULTIPLIER = 1.05f;
    public override int ApplyAllModifiersToSpecifiedTypeOfModificator(int value, TypeOfModificator typeOfMoneyTransaction, PropertyType propertyType = PropertyType.None)
    {
        value = base.ApplyAllModifiersToSpecifiedTypeOfModificator(value, typeOfMoneyTransaction, propertyType);
        switch (typeOfMoneyTransaction)
        {
            case TypeOfModificator.EarningMoneyFromPropertie:
                //Debug.Log((value * GetCombineMultiplier()) +" " +((value * GetCombineMultiplier()) / 10) +" "+ Mathf.RoundToInt((value * GetCombineMultiplier()) / 10));
                if (propertyType == PropertyType.Prostitution) return Mathf.RoundToInt((value * GetCombineMultiplier())/10)*10;
                return value;
            case TypeOfModificator.PayingForPenalty:
                return Mathf.RoundToInt((value * PAYMENT_PENALTIES_MULTIPLIER) / 10)*10;
            case TypeOfModificator.BuyingTown:
                return Mathf.RoundToInt((value * PAYMENT_PENALTIES_MULTIPLIER) / 10)*10;
            default:
                return value;

        }

    }


    private float GetCombineMultiplier()
    {
        float combineMultiplier = 0;
        foreach (TileScript tile in playerScript.GetTilesThatPlayerOwnList())
        {
            if(tile.propertyType == PropertyType.Prostitution)
            {
                combineMultiplier++;
            }

        }
        return (combineMultiplier* EARNINGS_MULTIPLIER) +1;
    }

    public override void OnOwnedTileChange(TileScript tile)
    {
        foreach (TileScript playerTile in playerScript.GetTilesThatPlayerOwnList())
        {
            if (playerTile.propertyType == PropertyType.Prostitution)
            {
                playerTile.UpdateOwnerTextServerRpc();
            }
                
        }
    }

}
