using Unity.Netcode;
using UnityEngine;

public class BrothelKeeper : Character
{

    public override void Greetings()
    {
        Debug.Log("BrothelKeeper");
        name = "Burdel Mama";
    }

    const float EARNINGS_MULTIPLIER = 0.05f;

    const float PAYMENT_PENALTIES_MULTIPLIER = 1.1f;
    public override int ApplyAllModifiersToSpecifiedAmountOfMoney(int amountOfMoney, TypeOfMoneyTransaction typeOfMoneyTransaction, PropertyType propertyType = PropertyType.None)
    {
        switch (typeOfMoneyTransaction)
        {
            case TypeOfMoneyTransaction.EarningMoneyFromPropertie:
                //Debug.Log((amountOfMoney * GetCombineMultiplier()) +" " +((amountOfMoney * GetCombineMultiplier()) / 10) +" "+ Mathf.RoundToInt((amountOfMoney * GetCombineMultiplier()) / 10));
                if (propertyType == PropertyType.Prostitution) return Mathf.RoundToInt((amountOfMoney * GetCombineMultiplier())/10)*10;
                return amountOfMoney;
            case TypeOfMoneyTransaction.PayingForPenalty:
                return Mathf.RoundToInt((amountOfMoney * PAYMENT_PENALTIES_MULTIPLIER) / 10)*10;
            case TypeOfMoneyTransaction.BuyingTown:
                return Mathf.RoundToInt((amountOfMoney * PAYMENT_PENALTIES_MULTIPLIER) / 10)*10;
            default:
                return amountOfMoney;

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
            Debug.Log("town updated " + playerTile.name + " " + playerTile.ownerId.Value + " " + playerScript.playerIndex);
            if (playerTile.propertyType == PropertyType.Prostitution)
            {
                playerTile.UpdateOwnerTextServerRpc();
            }
                
        }
    }

}
