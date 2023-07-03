using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public int tileType = 0;
    // 0-> town tile
    // 1-> Start tile
    // 2-> chance tile#1
    // 3-> chance tile#2
    // 4-> train tile
    // 5-> parking tile
    // 6-> lightbulb tile
    // 7-> waterpipes tile
    // 8 -> ring tile
    // 9 -> custody tile
    // 10 -> patrol tile
    // 11 -> party tile

    public List<int> townCostToBuy = new List<int>();
    public List<int> townCostToPay = new List<int>();

    public int ownerId = -1;
    public int townLevel = 0;
    public int curentMaxTownLevelThatCanBeBuy = 3;
    public int amountMoneyGiveOnPlayerStep = 0;
    // -1 means that town has no owner
    

    public void OnPlayerEnter()
    {
        switch (tileType)
        {
            case 0:
                int playerAmountOfMoney = PlayerScript.LocalInstance.amountOfMoney;
                if (ownerId==-1)
                {
                    int maxLevelThatPlayerCanAfford = 0;
                    //option to buy town
                    for(int i=0;i< curentMaxTownLevelThatCanBeBuy;i++)
                    {
                        if (playerAmountOfMoney > townCostToBuy[i])
                        {
                            //counting how many town levels can player buy
                            maxLevelThatPlayerCanAfford++;
                        }
                    }
                    if(maxLevelThatPlayerCanAfford!=0)
                    {
                        //can buy at lest 1 level town
                        BuyingTabUIScript.Instance.ShowBuyingUI(townLevel, maxLevelThatPlayerCanAfford, townCostToBuy,this);
                        return;
                    }
                    //no option of buying return and go to next player
                    GameUIScript.OnNextPlayerTurn.Invoke();
                    return;
                }
                //paying someone for visiting town
                if(playerAmountOfMoney > townCostToPay[townLevel])
                {
                    //player can pay for visiting town
                    PlayerScript.LocalInstance.amountOfMoney -= townCostToPay[townLevel];
                    GameLogic.Instance.GivePlayerMoneyByIdServerRpc(ownerId, townCostToPay[townLevel]);
                    GameUIScript.OnNextPlayerTurn.Invoke();
                }
                else
                {
                    //player cant aford visiting town there for needs to sold some of his properite to pay for it
                    PlayerScript.LocalInstance.amountOfMoney = 0;
                    GameUIScript.OnNextPlayerTurn.Invoke();
                }
                break;
            case 1:
                PlayerScript.LocalInstance.amountOfMoney += amountMoneyGiveOnPlayerStep;
                break;
        
        }

    }

    public void UpgradeTown(int addedLevels,int ownerId)
    {
        this.ownerId = ownerId;
        townLevel += addedLevels+1;
        if (curentMaxTownLevelThatCanBeBuy != 5) curentMaxTownLevelThatCanBeBuy++;
    }



}
