using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public abstract class Character
{
    public PlayerScript playerScript;
    public string name = "blank";
    public int happinesValue=1;
    public List<Modifiers> characterAdvantagesAndDisadvantages;
    public bool isWoman;
    public string GetName()
    {
        return name;
    }


    public virtual void OnCharacterCreated()
    {
        characterAdvantagesAndDisadvantages = new List<Modifiers>();
    }

    public void BeforeOnPlayerPassBy(TileScript tile)
    {
        if (tile.GetComponentInChildren<DeadDropBox>() != null)
        {
            foreach (Transform child in tile.transform)
            {
                if (child.GetComponent<DeadDropBox>() != null) PlayerScript.LocalInstance.character.ClaimDeadDropBox(child.GetComponent<DeadDropBox>());
            }
        }
        OnPlayerPassBy(tile);
    }


    public virtual void OnPlayerPassBy(TileScript tile)
    {
        
    }


    public virtual int OnDiceRolled(int diceRoll)
    {
        //Debug.Log("Nie powinienes tego widziec");
        diceRoll = ApplyAllModifiersToSpecifiedTypeOfModificator(diceRoll, TypeOfModificator.DiceResult);
        return diceRoll;
    }

    public virtual bool OnPlayerStepped(TileScript tile)
    {
        return false;
    }

    public virtual void Greetings()
    {
        Debug.Log("Character");
    }

    public virtual int ApplyAllModifiersToSpecifiedTypeOfModificator(int value, TypeOfModificator typeOfModificator, PropertyType propertyType = PropertyType.None)
    {
        Modifiers currentModifier;
        switch (typeOfModificator)
        {
            case TypeOfModificator.BuyingItem:
            case TypeOfModificator.BuyingTown:
            case TypeOfModificator.PayingForEnteringTown:
            case TypeOfModificator.PayingForPenalty:
                return Mathf.RoundToInt(ApplyCharacterAdvantagesOrDisadvantages(TypeOfCharacterAdvantageOrDisadvantage.HappinesAfterSex,value) / 10) * 10;


            case TypeOfModificator.DiceResult:
                currentModifier = playerScript.playerDrugsSystem.FindAndGetModifierByTypeOnlyFirst(typeOfModificator);
                if (currentModifier == null) break;
                value += currentModifier.value;
                break;
            case TypeOfModificator.HazardWinChance:
                currentModifier = playerScript.playerDrugsSystem.FindAndGetModifierByTypeOnlyFirst(typeOfModificator);
                if (currentModifier == null) break;
                value += currentModifier.value;
                break;
            case TypeOfModificator.MoneyOnStartTile:
                currentModifier = playerScript.playerDrugsSystem.FindAndGetModifierByTypeOnlyFirst(typeOfModificator);
                if (currentModifier == null) break;
                if (currentModifier.modifiersType == ModifiersType.Precentage) value += (int)((float)((float)currentModifier.value / 100) * value);
                else value += currentModifier.value;
                break;
            case TypeOfModificator.AlcoholTier:
                currentModifier = playerScript.playerDrugsSystem.FindAndGetModifierByTypeOnlyFirst(typeOfModificator);
                if (currentModifier == null) break;
                value += currentModifier.value;
                if (value > 4) value = 4;
                break;
            case TypeOfModificator.LuckLevel:
                currentModifier = playerScript.playerDrugsSystem.FindAndGetModifierByTypeOnlyFirst(typeOfModificator);
                if (currentModifier == null) break;
                value += currentModifier.value;
                break;
            default:
                currentModifier = playerScript.playerDrugsSystem.FindAndGetModifierByTypeOnlyFirst(typeOfModificator);
                if (currentModifier == null) break;
                value += currentModifier.value;
                break;
        }
        return value;
    }

    public virtual void OnOwnedTileChange(TileScript tile)
    {

    }

    public virtual void ClaimDeadDropBox(DeadDropBox deadDropBoxScript)
    {
        deadDropBoxScript.OnPlayerClaimServerRpc(playerScript.playerIndex);
    }

    public virtual float ApplyCharacterAdvantagesOrDisadvantages(TypeOfCharacterAdvantageOrDisadvantage modificatorType,float value)
    {
        for (int i = 0; i < characterAdvantagesAndDisadvantages.Count; i++)
        {
            if (characterAdvantagesAndDisadvantages[i].typeOfCharacterProsOrCons != modificatorType) continue;
            switch(modificatorType)
            {
                case TypeOfCharacterAdvantageOrDisadvantage.HappinesAfterSex:
                    value -= (value * ((float)characterAdvantagesAndDisadvantages[i].value / 100));
                    break;
            }
        }
        return value;
    }

    public virtual void SetCharacterAdvantagesOrDisadvantages(TypeOfCharacterAdvantageOrDisadvantage modificatorName,int value)
    {

    }

    public virtual void AfterCharacterPayForSth(TypeOfModificator typeOfTransaction,int amount)
    {
        canRemove = false;
        switch (typeOfTransaction)
        {
            case TypeOfModificator.PayingForEnteringTown:
            case TypeOfModificator.BuyingTown:
                for (int i = 0; i < characterAdvantagesAndDisadvantages.Count; i++)
                {
                    switch (characterAdvantagesAndDisadvantages[i].typeOfCharacterProsOrCons)
                    {
                        case TypeOfCharacterAdvantageOrDisadvantage.HappinesAfterSex:
                            characterAdvantagesAndDisadvantages[i].maxValue--;
                            if (characterAdvantagesAndDisadvantages[i].maxValue == characterAdvantagesAndDisadvantages[i].minValue)
                            {
                                RemoveCharacterAdvantageOrDisadvangtage(i);
                            }

                            break;
                    }

                }
                break;
        }
        canRemove = true;
    }
    bool canRemove = false;
    private async void RemoveCharacterAdvantageOrDisadvangtage(int index)
    {
        while(!canRemove)
        {
            await Awaitable.FixedUpdateAsync();
        }
        characterAdvantagesAndDisadvantages.RemoveAt(index);
        canRemove = false;
    }

}

public enum TypeOfModificator
{ 
    BuyingTown,
    PayingForEnteringTown,
    PayingForPenalty,
    GettingMoney,
    MoneyOnStartTile,
    EarningMoneyFromPropertie,
    BuyingItem,
    HazardWinChance,
    DiceResult,
    AlcoholTier,
    LuckLevel,
    DrugWithdrawalDeley,
    CharacterAdvantages,
    CharacterDisadvantages
}

public enum TypeOfCharacterAdvantageOrDisadvantage
{
    Null,
    HappinesAfterSex,
    MoreEarnings,
    LessCosts,
    FirstBuildingEarnings,
    CantMoveAfterSpecifiedTurnAmount,
    CantTakeAnythingThatIsntKosher
}
