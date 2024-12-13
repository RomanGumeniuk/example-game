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
        GameUIScript.OnNextPlayerTurn.Invoke();
    }


    public override void OnPlayerPassBy()
    {
        tileScript.GiveMoney(GetAmountOfMoney());
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

        return combineAmount;
    }

}
