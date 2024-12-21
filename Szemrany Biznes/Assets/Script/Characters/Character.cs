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

    public virtual int CheckCharacterMultipliersForBuying(int amountOfMoney, PropertyType propertyType)
    {
        return amountOfMoney;
    }

    public virtual int CheckCharacterMultipliersForPayments(int amountOfMoney, PropertyType propertyType)
    {
        return amountOfMoney;
    }

    public virtual int GetRealCostForCharacter(int cost)
    {
        return cost;
    }

    public virtual void OnOwnedTileChange(TileScript tile)
    {

    }
}
