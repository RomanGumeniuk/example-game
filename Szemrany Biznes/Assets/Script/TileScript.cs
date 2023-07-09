using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class TileScript : NetworkBehaviour
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

    public NetworkVariable<int> ownerId = new NetworkVariable<int>( -1);
    public NetworkVariable<int> townLevel = new NetworkVariable<int>(-1);
    public int amountMoneyOnPlayerStep = 0;
    public DisplayPropertyUI displayPropertyUI;
    // -1 means that town has no owner

    private void Start()
    {
        displayPropertyUI = transform.GetComponentInChildren<DisplayPropertyUI>();
        if (tileType != 0) return;

        if (townCostToBuy.Count == 0) return;
        //townLevel.Value = -1;
        
        townCostToBuy.Add((int)(townCostToBuy[0] * 1.5f));
        townCostToBuy.Add(townCostToBuy[0] * 3);
        townCostToBuy.Add((int)(townCostToBuy[0] * 4.5f));
        townCostToBuy.Add((int)(townCostToBuy[0] * 6));
        townCostToPay.Add(townCostToBuy[0]/2);
        townCostToPay.Add(townCostToBuy[0] );
        townCostToPay.Add((int)(townCostToBuy[0] * 1.5f));
        townCostToPay.Add(townCostToBuy[0] * 3);
        townCostToPay.Add((int)(townCostToBuy[0] * 4.5f));
        townCostToPay.Add((int)(townCostToBuy[0] * 6.5f));

    }


    public void OnPlayerEnter(int currentAvailableTownUpgrade, bool passTheStartTile = false)
    {
        int playerAmountOfMoney = PlayerScript.LocalInstance.amountOfMoney.Value;
        switch (tileType)
        {
            case 0:
                if (ownerId.Value == -1 || ownerId.Value == PlayerScript.LocalInstance.playerIndex) 
                {
                    int maxLevelThatPlayerCanAfford = 0;
                    //option to buy town
                    if (townLevel.Value == -1)
                    {
                        if (playerAmountOfMoney>=townCostToBuy[0])
                        {
                            BuyingTabForOnePaymentUIScript.Instance.ShowBuyingUI(townCostToBuy[0], this);
                            return;
                        }
                        GameUIScript.OnNextPlayerTurn.Invoke();
                        return;
                    }
                    for (int i= townLevel.Value; i< townLevel.Value+3; i++)
                    {
                        if (i > 4) continue;
                        int currentTownCost = 0;
                        for(int j=townLevel.Value;j<i+1;j++)
                        {
                            currentTownCost += townCostToBuy[j];
                        }
                        if (playerAmountOfMoney >= currentTownCost)
                        {
                            //counting how many town levels can player buy
                            maxLevelThatPlayerCanAfford++;
                        }
                    }
                    if(maxLevelThatPlayerCanAfford!=0 && currentAvailableTownUpgrade>townLevel.Value)
                    {
                        //can buy at lest 1 level town
                        
                        BuyingTabUIScript.Instance.ShowBuyingUI(townLevel.Value, maxLevelThatPlayerCanAfford+townLevel.Value, townCostToBuy,this, currentAvailableTownUpgrade);
                        return;
                    }
                    if (maxLevelThatPlayerCanAfford != 0) AlertTabForPlayerUI.Instance.ShowTab("You can't aford any upgrade!", 3.5f);
                    else AlertTabForPlayerUI.Instance.ShowTab($"To upgrade this town you need minimal town level {townLevel.Value} in your colection of all real estates.\nNow your minimal level town is on {currentAvailableTownUpgrade} level.", 6);
                    //no option of buying return and go to next player
                    GameUIScript.OnNextPlayerTurn.Invoke();
                    return;
                }
                //paying someone for visiting town
                if(playerAmountOfMoney >= townCostToPay[townLevel.Value])
                {
                    //player can pay for visiting town
                    AlertTabForPlayerUI.Instance.ShowTab($"You paid {townCostToPay[townLevel.Value]}", 2);
                    GameLogic.Instance.UpdateMoneyForPlayerServerRpc(townCostToPay[townLevel.Value], PlayerScript.LocalInstance.playerIndex,1,true,true);
                    GameLogic.Instance.UpdateMoneyForPlayerServerRpc( townCostToPay[townLevel.Value], ownerId.Value, 2, false,true);
                    GameUIScript.OnNextPlayerTurn.Invoke();
                    return;
                }
                //player cant aford visiting town there for needs to sold some of his properite to pay for it
                PlayerScript.LocalInstance.ShowSellingTab(townCostToPay[townLevel.Value],ownerId.Value);
                return;
            case 1:
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(amountMoneyOnPlayerStep, PlayerScript.LocalInstance.playerIndex, 2,true,true);
                if(!passTheStartTile)GameUIScript.OnNextPlayerTurn.Invoke();
                break;
            case 5:
            case 8:
                if (playerAmountOfMoney >= amountMoneyOnPlayerStep)
                {
                    AlertTabForPlayerUI.Instance.ShowTab($"You paid {amountMoneyOnPlayerStep}", 2);
                    GameLogic.Instance.UpdateMoneyForPlayerServerRpc(amountMoneyOnPlayerStep, PlayerScript.LocalInstance.playerIndex, 1, true, true);
                    GameUIScript.OnNextPlayerTurn.Invoke();
                    return;
                }
                PlayerScript.LocalInstance.ShowSellingTab(amountMoneyOnPlayerStep, -1);
                return;
                

            case 4:
            case 6:
            case 7:
                if (ownerId.Value == -1)
                {
                    if(playerAmountOfMoney < townCostToBuy[0]) GameUIScript.OnNextPlayerTurn.Invoke();
                    else BuyingTabForOnePaymentUIScript.Instance.ShowBuyingUI(townCostToBuy[0],  this);
                    return;
                }
                if(ownerId.Value != PlayerScript.LocalInstance.playerIndex)
                {
                    if(playerAmountOfMoney >= townCostToPay[0])
                    {
                        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(townCostToPay[0], PlayerScript.LocalInstance.playerIndex, 1, true, true);
                        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(townCostToPay[0], ownerId.Value, 2, false, true);
                    }
                    else
                    {
                        PlayerScript.LocalInstance.ShowSellingTab(townCostToPay[townLevel.Value], ownerId.Value);
                        return;
                    }
                    
                }
                GameUIScript.OnNextPlayerTurn.Invoke();
                break;
            default:
                GameUIScript.OnNextPlayerTurn.Invoke();
                break;


        }

    }
    [ServerRpc(RequireOwnership = false)]
    public void UpgradeTownServerRpc(int currentNewLevel,int ownerId)
    {
        this.ownerId.Value = ownerId;
        townLevel.Value = currentNewLevel;
        UpdateOwnerTextClientRpc(ownerId, townLevel.Value);
    }

    [ServerRpc(RequireOwnership =false)]
    public void UpdateOwnerTextServerRpc()
    {
        UpdateOwnerTextClientRpc(ownerId.Value, townLevel.Value);
    }

    [ClientRpc]
    public void UpdateOwnerTextClientRpc(int ownerId,int townLevel)
    {
        if(displayPropertyUI==null)
        {
            Debug.LogError("No displayPropertyUI script found on:" + this.name);
            return;
        }
        if(townLevel<0) displayPropertyUI.ShowNormalView(ownerId, townLevel, 0);
        else displayPropertyUI.ShowNormalView(ownerId, townLevel, townCostToPay[townLevel]);
    }

    [ServerRpc(RequireOwnership =false)]
    public void SellingTownServerRpc(int playerIndex)
    {
        int townTotalValue = GetCurrentPropertyValue();
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(townTotalValue, playerIndex, 2);
        ownerId.Value = -1;
        townLevel.Value = -1;
        UpdateOwnerTextClientRpc(-1, -1);
    }


    public void ShowSellingView()
    {
        if (displayPropertyUI == null)
        {
            Debug.LogError("No displayPropertyUI script found on:" + this.name);
            return;
        }
        int totalPropertyValue = GetCurrentPropertyValue();
        displayPropertyUI.ShowSellingView(totalPropertyValue);
    }

    public int GetCurrentPropertyValue()
    {
        int totalPropertyValue = 0;
        for (int i = 0; i < townLevel.Value; i++)
        {
            totalPropertyValue += townCostToBuy[i];
        }
        totalPropertyValue += townCostToBuy[0];
        return totalPropertyValue;
    }



}
