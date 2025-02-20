using UnityEngine;
using UnityEngine.UI;

public class Drug : Item
{
    public Drug(string name, string description, int amountOfUses, int cost, RawImage icon, ItemType itemType, ItemTier itemTier)
    {
       
        this.name = name;
        this.description = description;
        this.amountOfUses = amountOfUses;
        this.cost = cost;
        this.icon = icon;
        this.itemType = itemType;
        this.itemTier = itemTier;
    
    }


    public override void OnItemUse()
    {
        OnDrugUse(itemTier);
    }

    void OnDrugUse(ItemTier itemTier,int withdrawalDelay = -100)
    {
        PlayerDrugsSystem playerDrugsSystem = PlayerScript.LocalInstance.playerDrugsSystem;
        switch (itemTier)
        {
            case ItemTier.Junk:
                playerDrugsSystem.TakeDrug(itemTier, new Modifiers("Szansa na wygranie w hazardzie", -20, TypeOfModificator.HazardWinChance, ModifiersType.Precentage, 0, -100), withdrawalDelay==-100?-1:withdrawalDelay);
                break;
            case ItemTier.Normal:
                if (playerDrugsSystem.IsModifierMaxedOutByTypeOnlyFirst(TypeOfModificator.DiceResult))
                {
                    OnDrugUse(0,0);
                    break;
                }
                playerDrugsSystem.TakeDrug(itemTier, new Modifiers("Wartœæ wyrzucona na koœci", 1, TypeOfModificator.DiceResult, ModifiersType.WholeNumber, 6, -6), withdrawalDelay == -100 ? 0 : withdrawalDelay);
                break;
            case ItemTier.Decent:
                if (playerDrugsSystem.IsModifierMaxedOutByTypeOnlyFirst(TypeOfModificator.MoneyOnStartTile))
                {
                    OnDrugUse(itemTier - 1,1);
                    break;
                }
                playerDrugsSystem.TakeDrug(itemTier, new Modifiers("Zarobek z przejœcia przez start", 10, TypeOfModificator.MoneyOnStartTile, ModifiersType.Precentage, 100, -80), withdrawalDelay == -100 ? 1 : withdrawalDelay);
                break;
            case ItemTier.Exclusive:
                if (playerDrugsSystem.IsModifierMaxedOutByTypeOnlyFirst(TypeOfModificator.AlcoholTier))
                {
                    OnDrugUse(itemTier - 1,2);
                    break;
                }
                playerDrugsSystem.TakeDrug(itemTier, new Modifiers("Minimalny poziom alkoholu", 1, TypeOfModificator.AlcoholTier, ModifiersType.WholeNumber, 4, 0), withdrawalDelay == -100 ? 2 : withdrawalDelay);
                break;
            case ItemTier.Relic:
                if(playerDrugsSystem.IsModifierMaxedOutByTypeOnlyFirst(TypeOfModificator.LuckLevel))
                {
                    OnDrugUse(itemTier - 1,3);
                    break;
                }
                playerDrugsSystem.TakeDrug(itemTier, new Modifiers("Poziom szczêœcia", 1, TypeOfModificator.LuckLevel, ModifiersType.WholeNumber, 5, -1), withdrawalDelay == -100 ? 3 : withdrawalDelay);
                break;
            case ItemTier.God:
                Modifiers modifier = playerDrugsSystem.FindAndGetModifierByTypeOnlyFirst(TypeOfModificator.LuckLevel);
                if(modifier != null)
                {
                    if(modifier.value == 6)
                    {
                        OnDrugUse(itemTier - 1,4);
                        break;
                    }
                }
                playerDrugsSystem.TakeDrug(itemTier, new Modifiers("Poziom szczêœcia", 2, TypeOfModificator.LuckLevel, ModifiersType.WholeNumber, 6, -1),   withdrawalDelay == -100 ? 4 : withdrawalDelay);
                break;
        }
    }

}
