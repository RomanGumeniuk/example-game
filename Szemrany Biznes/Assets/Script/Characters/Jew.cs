using NUnit.Framework;
using System.Collections.Generic;
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


    public override int ApplyAllModifiersToSpecifiedTypeOfModificator(int value, TypeOfModificator typeOfMoneyTransaction, PropertyType propertyType=PropertyType.None)
    {
        value = base.ApplyAllModifiersToSpecifiedTypeOfModificator(value,typeOfMoneyTransaction, propertyType);
        int result;
        switch(typeOfMoneyTransaction)
        {
            case TypeOfModificator.BuyingTown:
                result = Mathf.RoundToInt((value * ApplyCharacterAdvantagesOrDisadvantages("LessCosts", 0)) / 10) * 10;
                return result;

            case TypeOfModificator.EarningMoneyFromPropertie:
                result = Mathf.RoundToInt((value * ApplyCharacterAdvantagesOrDisadvantages("MoreEarnings", 0)) / 10) * 10;
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
        characterAdvantagesAndDisadvantages = new List<Modifiers>();
        characterAdvantagesAndDisadvantages.Add(new Modifiers("MoreEarnings",10,TypeOfModificator.CharacterAdvantages,ModifiersType.Precentage,0,0));
        characterAdvantagesAndDisadvantages.Add(new Modifiers("LessCosts", -5, TypeOfModificator.CharacterAdvantages, ModifiersType.Precentage, 0, 0));
        characterAdvantagesAndDisadvantages.Add(new Modifiers("FirstBuildingEarnings", 2, TypeOfModificator.CharacterAdvantages, ModifiersType.WholeNumber, 0, 0));
        characterAdvantagesAndDisadvantages.Add(new Modifiers("CantMoveAfterSpecifiedTurnAmount", 7, TypeOfModificator.CharacterDisadvantages, ModifiersType.WholeNumber, 0, 0));
        characterAdvantagesAndDisadvantages.Add(new Modifiers("CantTakeAnythingThatIsntKosher", 1, TypeOfModificator.CharacterDisadvantages, ModifiersType.WholeNumber, 0, 0));
    }

    public override float ApplyCharacterAdvantagesOrDisadvantages(string modificatorName, float value)
    {
        int index=0;
        int defaultValue = 0;
        int happinesValue = ApplyAllModifiersToSpecifiedTypeOfModificator(this.happinesValue, TypeOfModificator.LuckLevel);
        for (int i = 0; i < characterAdvantagesAndDisadvantages.Count; i++)
        {
            if (characterAdvantagesAndDisadvantages[i].name != modificatorName) continue;
            index = i;
            break;
        }
        switch (modificatorName)
        {
            case "MoreEarnings":
            case "LessCosts":
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
            case "FirstBuildingEarnings":
                defaultValue = 2;
                if (happinesValue == -1) return value *= 0.5f;
                if (happinesValue == 0) defaultValue = 1;
                if (happinesValue > 2) defaultValue +=1;
                if(happinesValue >5) defaultValue +=1;
                value *= defaultValue;
                return value;
            case "CantMoveAfterSpecifiedTurnAmount":
                if (happinesValue > 3) value += 2;
                if (happinesValue > 5) value += 2;
                return value;
            case "CantTakeAnythingThatIsntKosher":
                if (happinesValue == 6) value = 0;
                return value;
            default:
                return value;
        }
        
    }

    public override void SetCharacterAdvantagesOrDisadvantages(string modificatorName, int value)
    {
        for(int i=0;i<characterAdvantagesAndDisadvantages.Count;i++)
        {
            if (characterAdvantagesAndDisadvantages[i].name != modificatorName) continue;
            characterAdvantagesAndDisadvantages[i].value = value;
            return;
        }
    }

}
