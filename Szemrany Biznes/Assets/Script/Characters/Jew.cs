using UnityEngine;

public class Jew : Character
{
    public override void Greetings()
    {
        Debug.Log("Jew");
        name = "¯yd";
    }
    int moves = 0;
    public override void OnPlayerStepped(TileScript tile)
    {
        moves++;
        if(moves==6)
        {
            moves = 0;
            playerScript.ChangeCantMoveValueServerRpc(1);
        }
    }

    const float MULTIPLIER_FOR_EARNINGS = 1.1f;
    const float COST_MULTIPLIER = 0.95f;

    public override int CheckCharacterMultipliersForBuying(int amountOfMoney, PropertyType propertyType)
    {

        return Mathf.CeilToInt(amountOfMoney * COST_MULTIPLIER);
    }

    public override int CheckCharacterMultipliersForPayments(int amountOfMoney, PropertyType propertyType)
    {
        return Mathf.CeilToInt(amountOfMoney * MULTIPLIER_FOR_EARNINGS);
    }
}
