using UnityEngine;

public class BillTile : Tile
{
    public BillTile(TileScript tileScript)
    {
        this.tileScript = tileScript;
    }

    public override void OnPlayerStepped()
    {
        PlayerScript player = PlayerScript.LocalInstance;
        if(tileScript.amountMoneyOnPlayerStep == 0)
        {
            //dochodowy podatek 10%
            tileScript.Pay(player.amountOfMoney.Value, player.amountOfMoney.Value / 10, false);
        }
        else
        {
            //podatek za posiadłości
            int payAmount = 0;
            for(int i=0;i<player.GetTilesThatPlayerOwnList().Count;i++)
            {
                
                payAmount += player.GetTilesThatPlayerOwnList()[i].specialTileScript.CaluculatePropertyValue() / 20;
            }
            tileScript.Pay(player.amountOfMoney.Value, payAmount, false);
        }
    }


}
