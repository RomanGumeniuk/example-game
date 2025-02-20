using UnityEngine;

public class StartTile : Tile
{
    private int gangBonus=100;

    public StartTile(TileScript tileScript)
    {
        this.tileScript = tileScript;
    }

    public override void OnPlayerStepped()
    {
        tileScript.GiveMoney(GetAmountOfMoney());
        ShowButtonsForRepair();
        GameUIScript.OnNextPlayerTurn.Invoke();

    }


    public override void OnPlayerPassBy(int diceRoll)
    {
        if (diceRoll < 0) return;
        tileScript.GiveMoney(GetAmountOfMoney());
        ShowButtonsForRepair();
    }

    private int GetAmountOfMoney()
    {
        int combineAmount=tileScript.amountMoneyOnPlayerStep;

        int gangAmount = 0;
        for(int i=0;i<PlayerScript.LocalInstance.GetTilesThatPlayerOwnList().Count;i++)
        {
            if (PlayerScript.LocalInstance.GetTilesThatPlayerOwnList()[i].tileType == TileType.GangTile) gangAmount++;
        }
        combineAmount += gangBonus * gangAmount;
        combineAmount = PlayerScript.LocalInstance.character.ApplyAllModifiersToSpecifiedTypeOfModificator(combineAmount, TypeOfModificator.MoneyOnStartTile);
        return combineAmount;
    }

    private void ShowButtonsForRepair()
    {
        foreach(TileScript tile in PlayerScript.LocalInstance.GetTilesThatPlayerOwnList())
        {
            if(tile.destroyPercentage.Value > 0)
            {
                tile.displayPropertyUI.ShowButton("Fix " + tile.GetRepairCost() + "PLN");
            }
        }
    }

}
