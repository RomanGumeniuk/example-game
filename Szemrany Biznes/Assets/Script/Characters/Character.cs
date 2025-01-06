using UnityEngine;

public abstract class Character
{
    public PlayerScript playerScript;
    public string name = "blank";

    public string GetName()
    {
        return name;
    }


    public virtual void OnCharacterCreated()
    {

    }

    public virtual void OnPlayerPassBy(TileScript tile)
    {

    }


    public virtual int OnDiceRolled(int diceRoll)
    {
        //Debug.Log("Nie powinienes tego widziec");
        return diceRoll;
    }

    public virtual bool OnPlayerStepped(TileScript tile)
    {
        Debug.Log("Character");
        return false;
    }

    public virtual void Greetings()
    {
        Debug.Log("Character");
    }

    public virtual int ApplyAllModifiersToSpecifiedAmountOfMoney(int amountOfMoney, TypeOfMoneyTransaction typeOfMoneyTransaction, PropertyType propertyType = PropertyType.None)
    {
        return amountOfMoney;
    }

    public virtual void OnOwnedTileChange(TileScript tile)
    {

    }

    public virtual void ClaimDeadDropBox(DeadDropBox deadDropBoxScript)
    {
        deadDropBoxScript.OnPlayerClaimServerRpc(playerScript.playerIndex);
    }
}

public enum TypeOfMoneyTransaction
{ 
    BuyingTown,
    PayingForEnteringTown,
    PayingForPenalty,
    GettingMoney,
    MoneyOnStartTile,
    EarningMoneyFromPropertie
}
