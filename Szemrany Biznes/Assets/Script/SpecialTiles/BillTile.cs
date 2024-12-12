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
            //podatek za posiad³oœci
            int payAmount = 0;
            for(int i=0;i<player.tilesThatPlayerOwnList.Count;i++)
            {
                
                payAmount += player.tilesThatPlayerOwnList[i].townCostToBuy[0] / 20;
                Debug.Log(player.tilesThatPlayerOwnList[i].name + " " + payAmount + " " + player.tilesThatPlayerOwnList[i].townCostToBuy[0] / 20);
            }
            tileScript.Pay(player.amountOfMoney.Value, payAmount, false);
        }
    }


}
