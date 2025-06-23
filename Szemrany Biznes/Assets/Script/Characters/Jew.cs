using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Jew : Character
{
    public override void Greetings()
    {
        Debug.Log("Jew");
        name = "¯yd";
        isWoman = false;
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


    public override int ApplyAllModifiersToSpecifiedTypeOfModificator(int value, TypeOfModificator typeOfMoneyTransaction, PropertyType propertyType=PropertyType.None)
    {
        value = base.ApplyAllModifiersToSpecifiedTypeOfModificator(value,typeOfMoneyTransaction, propertyType);
        int result;
        switch(typeOfMoneyTransaction)
        {
            case TypeOfModificator.BuyingTown:
                result = Mathf.RoundToInt((value * ApplyCharacterAdvantagesOrDisadvantages(TypeOfCharacterAdvantageOrDisadvantage.LessCosts, 0)) / 10) * 10;
                return result;

            case TypeOfModificator.EarningMoneyFromPropertie:
                result = Mathf.RoundToInt((value * ApplyCharacterAdvantagesOrDisadvantages(TypeOfCharacterAdvantageOrDisadvantage.MoreEarnings, 0)) / 10) * 10;
                return result;
            case TypeOfModificator.LuckLevel:
                Modifiers modifiers = playerScript.playerDrugsSystem.FindAndGetModifierByTypeOnlyFirst(TypeOfModificator.LuckLevel);
                if (modifiers == null) return happinesValue;
                Debug.Log(modifiers.value);
                return Mathf.Min(happinesValue + modifiers.value, 6);
            default:
                return value;
        }
    }


    public override void OnCharacterCreated()
    {
        base.OnCharacterCreated();
        characterAdvantagesAndDisadvantages.Add(new Modifiers("MoreEarnings",10,TypeOfModificator.CharacterAdvantages,ModifiersType.Precentage,0,0, true,TypeOfCharacterAdvantageOrDisadvantage.MoreEarnings));
        characterAdvantagesAndDisadvantages.Add(new Modifiers("LessCosts", -5, TypeOfModificator.CharacterAdvantages, ModifiersType.Precentage, 0, 0,true, TypeOfCharacterAdvantageOrDisadvantage.LessCosts));
        characterAdvantagesAndDisadvantages.Add(new Modifiers("FirstBuildingEarnings", 2, TypeOfModificator.CharacterAdvantages, ModifiersType.WholeNumber, 0, 0,true, TypeOfCharacterAdvantageOrDisadvantage.FirstBuildingEarnings));
        characterAdvantagesAndDisadvantages.Add(new Modifiers("CantMoveAfterSpecifiedTurnAmount", 7, TypeOfModificator.CharacterDisadvantages, ModifiersType.WholeNumber, 0, 0,true, TypeOfCharacterAdvantageOrDisadvantage.CantMoveAfterSpecifiedTurnAmount));
        characterAdvantagesAndDisadvantages.Add(new Modifiers("CantTakeAnythingThatIsntKosher", 1, TypeOfModificator.CharacterDisadvantages, ModifiersType.WholeNumber, 0, 0,true, TypeOfCharacterAdvantageOrDisadvantage.CantTakeAnythingThatIsntKosher));
    }

    public override float ApplyCharacterAdvantagesOrDisadvantages(TypeOfCharacterAdvantageOrDisadvantage modificatorName, float value)
    {
        int index=0;
        int defaultValue = 0;
        int happinesValue = ApplyAllModifiersToSpecifiedTypeOfModificator(this.happinesValue, TypeOfModificator.LuckLevel);
        for (int i = 0; i < characterAdvantagesAndDisadvantages.Count; i++)
        {
            if (characterAdvantagesAndDisadvantages[i].typeOfCharacterProsOrCons != modificatorName) continue;
            index = i;
            break;
        }
        switch (modificatorName)
        {
            case TypeOfCharacterAdvantageOrDisadvantage.MoreEarnings:
            case TypeOfCharacterAdvantageOrDisadvantage.LessCosts:
                defaultValue = characterAdvantagesAndDisadvantages[index].value;
                if (happinesValue > 1)
                {
                    if(defaultValue>=0) defaultValue += happinesValue-1;
                    else defaultValue -= happinesValue-1;
                }
                if (happinesValue == 0) defaultValue = 0;
                if (happinesValue == -1) defaultValue *= -1;
                float multiplier = 1 + (float)(defaultValue / 100f);
                return multiplier;
            case TypeOfCharacterAdvantageOrDisadvantage.FirstBuildingEarnings:
                defaultValue = 2;
                if (happinesValue == -1) return value *= 0.5f;
                if (happinesValue == 0) defaultValue = 1;
                if (happinesValue > 2) defaultValue +=1;
                if(happinesValue >5) defaultValue +=1;
                value *= defaultValue;
                return value;
            case TypeOfCharacterAdvantageOrDisadvantage.CantMoveAfterSpecifiedTurnAmount:
                if (happinesValue > 3) value += 2;
                if (happinesValue > 5) value += 2;
                return value;
            case TypeOfCharacterAdvantageOrDisadvantage.CantTakeAnythingThatIsntKosher:
                if (happinesValue == 6) value = 0;
                return value;
            default:
                return value;
        }
        
    }

    public override void SetCharacterAdvantagesOrDisadvantages(TypeOfCharacterAdvantageOrDisadvantage modificatorName, int value)
    {
        for(int i=0;i<characterAdvantagesAndDisadvantages.Count;i++)
        {
            if (characterAdvantagesAndDisadvantages[i].typeOfCharacterProsOrCons != modificatorName) continue;
            characterAdvantagesAndDisadvantages[i].value = value;
            return;
        }
    }

}
