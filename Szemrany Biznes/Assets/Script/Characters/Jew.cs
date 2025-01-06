using UnityEngine;

public class Jew : Character
{
    public override void Greetings()
    {
        Debug.Log("Jew");
        name = "¯yd";
    }
    int moves = 0;
    public override bool OnPlayerStepped(TileScript tile)
    {
        moves++;
        if(moves==6)
        {
            moves = 0;
            playerScript.ChangeCantMoveValueServerRpc(1);
        }
        return false;
    }

    const float MULTIPLIER_FOR_EARNINGS = 1.1f;
    const float COST_MULTIPLIER = 0.95f;


    public override int ApplyAllModifiersToSpecifiedAmountOfMoney(int amountOfMoney, TypeOfMoneyTransaction typeOfMoneyTransaction, PropertyType propertyType=PropertyType.None)
    {
        switch(typeOfMoneyTransaction)
        {
            case TypeOfMoneyTransaction.BuyingTown:
                return Mathf.RoundToInt((amountOfMoney * COST_MULTIPLIER)/10)*10;

            case TypeOfMoneyTransaction.EarningMoneyFromPropertie:
                return Mathf.RoundToInt((amountOfMoney * MULTIPLIER_FOR_EARNINGS)/10)*10; 
            default:
                return amountOfMoney;
        }
    }

    
}
