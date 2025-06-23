using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static Unity.Cinemachine.CinemachineFreeLookModifier;

public class PlayerDrugsSystem : MonoBehaviour
{
    public int baseWithdrawalCount = 10;

    public int currentWithdrawalCount = 10;

    [SerializeField] bool lowerBaseWithdrawal = false;

    [SerializeField] int currentMaxDrugLevelTaken = 0;

    [SerializeField] bool badTrip = false;

    [SerializeField]bool isInStreak = false;

    public Character currentPlayerCharacter;

    public List<Modifiers> modifiers;

    public int badTripLength=0;
    public void ResetWidthdrawalCount()
    {
        baseWithdrawalCount = 10;
    }

    public void TakeDrug(ItemTier drugTier,Modifiers modifier, int withdrawalDelay)
    {
        withdrawalDelay = PlayerScript.LocalInstance.character.ApplyAllModifiersToSpecifiedTypeOfModificator(withdrawalDelay, TypeOfModificator.DrugWithdrawalDeley);
        if (badTrip)
        {
            if((int)drugTier+1>=currentMaxDrugLevelTaken)
            {
                RemoveAllDrugsModifier(true);
                SetActiveAllDrugsModifier(true);
                badTrip = false;
                lowerBaseWithdrawal = false;
                currentMaxDrugLevelTaken = (int)drugTier + 1;
                if (currentWithdrawalCount < baseWithdrawalCount)
                {
                    currentWithdrawalCount = baseWithdrawalCount;
                }
                currentWithdrawalCount += withdrawalDelay;
            }
            return;
        }

        if(!isInStreak) isInStreak = true;
        if (lowerBaseWithdrawal)
        {
            if (baseWithdrawalCount != 0) baseWithdrawalCount--;
            lowerBaseWithdrawal = !lowerBaseWithdrawal;
        }
        else lowerBaseWithdrawal = !lowerBaseWithdrawal;
        if(currentWithdrawalCount < baseWithdrawalCount)
        {
            currentWithdrawalCount = baseWithdrawalCount;
        }
        currentWithdrawalCount += withdrawalDelay;
        if ((int)drugTier + 1 > currentMaxDrugLevelTaken) currentMaxDrugLevelTaken = (int)drugTier + 1;
        if (currentWithdrawalCount <= 0) BadTrip();
        bool foundModifier = false;
        for(int i=0;i<modifiers.Count;i++)
        {
            if (modifiers[i].name != modifier.name) continue;
            
            if (modifier.maxValue > modifiers[i].value && modifier.minValue < modifiers[i].value) modifiers[i].value += modifier.value;
            foundModifier = true;
            if (modifiers[i].maxValue < modifier.maxValue) modifiers[i].maxValue = modifier.maxValue;
            if (modifiers[i].minValue > modifier.minValue) modifiers[i].minValue = modifier.minValue;
            if ((modifiers[i].value == modifiers[i].maxValue || modifiers[i].value == modifiers[i].minValue) && !modifiers[i].isModifierMaxedOut)
            {
                modifiers[i].isModifierMaxedOut = true;
                if (!CheckIfAllDrugsModifiersAreMaxedOut()) break;
                modifiers.Add(new Modifiers("Przed³u¿enie odstawienia narkotykiem", -1, TypeOfModificator.DrugWithdrawalDeley, ModifiersType.WholeNumber, 0, -1));
            }
            break;
            
        }
        if(!foundModifier)
        {
            modifiers.Add(modifier);
        }


    }
    

    public void BadTrip()
    {
        if(!badTrip)
        {
            badTrip = true;
            SetActiveAllDrugsModifier(false);
            badTripLength = currentMaxDrugLevelTaken * 2;
        }
        Modifiers modifier1;
        Modifiers modifier2;
        Modifiers modifier3;
        bool foundModifier = false;
        switch (badTripLength)
        {
            case 0:
                modifier1 = new Modifiers("Wynik rzutu kostka", -1, TypeOfModificator.DiceResult, ModifiersType.WholeNumber, 0, 0);
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier1.name) continue;
                    modifiers.RemoveAt(i);
                }
                badTrip = false;
                RemoveAllDrugsModifier();
                isInStreak = false;
                ResetWidthdrawalCount();
                currentMaxDrugLevelTaken = 0;
                lowerBaseWithdrawal = false;
                break;
            case 2:
                modifier1 = new Modifiers("Wynik rzutu kostka", -1, TypeOfModificator.DiceResult, ModifiersType.WholeNumber, 0, 0);
                modifier2 = new Modifiers("Kasa na starcie", 0, TypeOfModificator.MoneyOnStartTile, ModifiersType.Precentage, 0, 0);
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier1.name) continue;
                    modifiers[i].value = modifier1.value;
                    foundModifier = true;
                }
                if (!foundModifier) modifiers.Add(modifier1);
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier2.name) continue;
                    modifiers.RemoveAt(i);
                }
                currentMaxDrugLevelTaken = 1;
                break;
            case 4:
                modifier1 = new Modifiers("Wynik rzutu kostka", -2, TypeOfModificator.DiceResult, ModifiersType.WholeNumber, 0, 0);
                modifier2 = new Modifiers("Kasa na starcie", -10, TypeOfModificator.MoneyOnStartTile, ModifiersType.Precentage, 0, 0);
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier1.name) continue;
                    modifiers[i].value = modifier1.value;
                    foundModifier = true;
                }
                if (!foundModifier) modifiers.Add(modifier1);
                foundModifier = false;
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier2.name) continue;
                    modifiers[i].value = modifier2.value;
                    foundModifier = true;
                }
                if (!foundModifier) modifiers.Add(modifier2);
                currentMaxDrugLevelTaken = 2;
                break;
            case 6:
                modifier1 = new Modifiers("Wynik rzutu kostka", -3, TypeOfModificator.DiceResult, ModifiersType.WholeNumber, 0, 0);
                modifier2 = new Modifiers("Kasa na starcie", -20, TypeOfModificator.MoneyOnStartTile, ModifiersType.Precentage, 0, 0);
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier1.name) continue;
                    modifiers[i].value = modifier1.value;
                    foundModifier = true;
                }
                if (!foundModifier) modifiers.Add(modifier1);
                foundModifier = false;
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier2.name) continue;
                    modifiers[i].value = modifier2.value;
                    foundModifier = true;
                }
                if (!foundModifier) modifiers.Add(modifier2);
                currentMaxDrugLevelTaken = 3;
                break;
            case 8:
                modifier1 = new Modifiers("Wynik rzutu kostka", -5, TypeOfModificator.DiceResult, ModifiersType.WholeNumber, 0, 0);
                modifier2 = new Modifiers("Kasa na starcie", -40, TypeOfModificator.MoneyOnStartTile, ModifiersType.Precentage, 0, 0);
                modifier3 = new Modifiers("Szczêœcie zabrane", 0, TypeOfModificator.LuckLevel, ModifiersType.WholeNumber, 0, 0);
                
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier1.name) continue;
                    modifiers[i].value = modifier1.value;
                    foundModifier = true;
                }
                if (!foundModifier) modifiers.Add(modifier1);
                foundModifier = false;
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier2.name) continue;
                    modifiers[i].value = modifier2.value;
                    foundModifier = true;
                }
                if (!foundModifier) modifiers.Add(modifier2);
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier3.name) continue;
                    modifiers.RemoveAt(i);
                }
                currentMaxDrugLevelTaken = 4;
                break;
            case 10:
                modifier1 = new Modifiers("Wynik rzutu kostka", -6, TypeOfModificator.DiceResult, ModifiersType.WholeNumber, 0, 0);
                modifier2 = new Modifiers("Kasa na starcie", -60, TypeOfModificator.MoneyOnStartTile, ModifiersType.Precentage, 0, 0);
                modifier3 = new Modifiers("Szczêœcie zabrane", 0, TypeOfModificator.LuckLevel, ModifiersType.WholeNumber, 0, 0);
                
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier1.name) continue;
                    modifiers[i].value = modifier1.value;
                    foundModifier = true;
                }
                if(!foundModifier)modifiers.Add(modifier1);
                foundModifier = false;
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier2.name) continue;
                    modifiers[i].value = modifier2.value;
                    foundModifier = true;
                }
                if (!foundModifier)modifiers.Add(modifier2);
                foundModifier = false;
                for (int i = 0; i < modifiers.Count; i++)
                {
                    if (modifiers[i].name != modifier3.name) continue;
                    modifiers[i].value = modifier3.value;
                    foundModifier = true;
                }
                if (!foundModifier) modifiers.Add(modifier3);
                currentMaxDrugLevelTaken = 5;
                break;
            case 12:
                modifier1 = new Modifiers("Wynik rzutu kostka", -6, TypeOfModificator.DiceResult, ModifiersType.WholeNumber, 0, 0);
                modifier2 = new Modifiers("Kasa na starcie", -80, TypeOfModificator.MoneyOnStartTile, ModifiersType.Precentage, 0, 0);
                modifier3 = new Modifiers("Szczêœcie zabrane", -1, TypeOfModificator.LuckLevel, ModifiersType.WholeNumber, 0, 0);
                modifiers.Add(modifier1);
                modifiers.Add(modifier2);
                modifiers.Add(modifier3);
                break;
        }
    }


    public void OnPlayerTurnEnded()
    {

        if (!isInStreak) return;
        if(currentWithdrawalCount!=0)currentWithdrawalCount--;
        if (badTrip)
        {
            badTripLength--;
        }
        if (currentWithdrawalCount <= 0) BadTrip();

    }

    public void SetActiveAllDrugsModifier(bool active)
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            if (!(((int)modifiers[i].typeOfModificator > 6 && (int)modifiers[i].typeOfModificator < 11) || modifiers[i].typeOfModificator == TypeOfModificator.MoneyOnStartTile)) continue;
            modifiers[i].SetIsActive(active);
        }
        
    }

    public void RemoveAllDrugsModifier(bool takeIsActiveInToAccount = false)
    {
        List<int> indexToRemove = new List<int>();
        for (int i = 0; i < modifiers.Count; i++)
        {
            if (takeIsActiveInToAccount && !modifiers[i].GetIsActive()) continue;

            if (!(((int)modifiers[i].typeOfModificator > 6 && (int)modifiers[i].typeOfModificator < 11) || modifiers[i].typeOfModificator == TypeOfModificator.MoneyOnStartTile)) continue;
            indexToRemove.Add(i);
        }
        for(int i=indexToRemove.Count - 1; i >= 0; i--)
        {
            modifiers.RemoveAt(indexToRemove[i]);
        }
        
    }

    public bool CheckIfAllDrugsModifiersAreMaxedOut()
    {
        int result = 0;
        for(int i =0;i<modifiers.Count;i++)
        {
            if (!(((int)modifiers[i].typeOfModificator > 6 && (int)modifiers[i].typeOfModificator < 11) || modifiers[i].typeOfModificator == TypeOfModificator.MoneyOnStartTile))continue;
            if (modifiers[i].isModifierMaxedOut)result++;
        }
        return result == 5;
    }

    public List<Modifiers> FindAndGetModifierByType(TypeOfModificator type)
    {
        List<Modifiers> resultModifiers = new List<Modifiers>();
        for(int i = 0; i < modifiers.Count;i++)
        {
            if (modifiers[i].typeOfModificator == type) resultModifiers.Add(new Modifiers(modifiers[i]));
        }
        return resultModifiers;
    }
    public Modifiers FindAndGetModifierByTypeOnlyFirst(TypeOfModificator type)
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            if (modifiers[i].typeOfModificator != type) continue;
            if (!modifiers[i].GetIsActive()) continue;
            return new Modifiers(modifiers[i]);
        }
        return null;
    }

    public bool IsModifierMaxedOutByTypeOnlyFirst(TypeOfModificator type)
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            if (modifiers[i].typeOfModificator == type) return modifiers[i].isModifierMaxedOut;
        }
        return false;
    }
}
[System.Serializable]
public class Modifiers
{
    public string name;
    public int value;
    public TypeOfModificator typeOfModificator;
    public TypeOfCharacterAdvantageOrDisadvantage typeOfCharacterProsOrCons;
    public ModifiersType modifiersType;
    public int maxValue;
    public int minValue;
    public bool isModifierMaxedOut=false;
    [SerializeField]bool isActive = true;

    public Modifiers(string name, int value, TypeOfModificator typeOfModificator,ModifiersType modifiersType, int maxValue,int minValue, bool isActive=true,TypeOfCharacterAdvantageOrDisadvantage typeOfCharacterProsOrCons = 0)
    {
        this.name = name;
        this.value = value;
        this.typeOfModificator = typeOfModificator;
        this.modifiersType = modifiersType;
        this.maxValue = maxValue;
        this.isActive = isActive;
        this.minValue = minValue;
        this.typeOfCharacterProsOrCons = typeOfCharacterProsOrCons;

    }

    public Modifiers(Modifiers copy)
    {
        name = copy.name;
        value = copy.value;
        typeOfModificator = copy.typeOfModificator;
        modifiersType = copy.modifiersType;
        maxValue = copy.maxValue;
        isModifierMaxedOut = copy.isModifierMaxedOut;
        minValue = copy.minValue;
        isActive = copy.isActive;
        typeOfCharacterProsOrCons = copy.typeOfCharacterProsOrCons;
    }
    public void SetIsActive(bool isActive)
    {
        this.isActive=isActive;
    }
    public bool GetIsActive()
    {
        return isActive;
    }
}

public enum ModifiersType
{
    Precentage,
    WholeNumber

}



