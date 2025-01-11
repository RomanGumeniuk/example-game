using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Tile
{
    public TileScript tileScript;

    public virtual void OnPlayerStepped()
    {
        GameUIScript.OnNextPlayerTurn.Invoke();
    }

    public virtual void OnPlayerPassBy(int diceRoll)
    {

    }

    public virtual int GetPayAmount()
    {
        if (tileScript.destroyPercentage.Value > 0) return 0;
        int townLevel = tileScript.townLevel.Value;
        if (townLevel == -1) townLevel = 0;
        int payAmount = tileScript.GetTownCostToPayIndex(townLevel,tileScript.ownerId.Value);
        return payAmount;
    }

    public virtual void OnDestroyPrecentageChange(int prevValue, int newValue)
    {
        tileScript.UpdateOwnerTextServerRpc();
    }


    public virtual void OnTownUpgrade(int ownerID,int townLevel)
    {

    }

    public virtual void OnTownSell(int ownerID,int newOwnerID = -1)
    {

    }

    public virtual void OnTownLevelChanged(int prevValue, int newValue)
    {

    }

    public virtual void OnOwnerIDChanged(int prevValue, int newValue)
    {

    }

    public virtual int CaluculatePropertyValue(int start = 0, int stop=-1)
    {
        int totalPropertyValue = 0;
        if (stop == -1)
        {
            //totalPropertyValue += tileScript.townCostToBuy[0];
            stop = tileScript.townLevel.Value+1;
            //if(stop == 0) totalPropertyValue += tileScript.townCostToBuy[0];
        }
        for (int i = start; i < stop; i++)
        {
            totalPropertyValue += tileScript.GetTownCostToBuyIndex(i, tileScript.ownerId.Value);
        }
        return totalPropertyValue;
    }
}
