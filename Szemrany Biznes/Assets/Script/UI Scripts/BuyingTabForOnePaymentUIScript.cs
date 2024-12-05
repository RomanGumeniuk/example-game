using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using System;

public class BuyingTabForOnePaymentUIScript : MonoBehaviour
{
    public static BuyingTabForOnePaymentUIScript Instance { get; private set; }
    public Button DontBuyButton;
    public Button BuyButton;
    public TextMeshProUGUI TextLable;
    public TextMeshProUGUI Cost;

    public int currentCost=0;

    public TileScript currentTileScript = null;


    private void Awake()
    {
        Instance = this;

        DontBuyButton.onClick.AddListener(() =>
        {
            if(NetworkManager.Singleton.ConnectedClientsList.Count>2)
            {
                //BiddingTabUIScript.Instance.StartAuctionServerRpc(currentTileScript.townCostToBuy[0],currentTileScript.name,PlayerScript.LocalInstance.playerIndex);
            }
            Hide();
        });

        BuyButton.onClick.AddListener(() =>
        {

            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(currentCost, PlayerScript.LocalInstance.playerIndex, 1);
            currentTileScript.UpgradeTownServerRpc(0, PlayerScript.LocalInstance.playerIndex);
            PlayerScript.LocalInstance.tilesThatPlayerOwnList.Add(currentTileScript);
            Hide();
            
        });
    }

    public void BuyTownForOtherPlayer(int playerIndex,int currentBid,int startPrice,bool moneyForBank)
    {
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(startPrice, playerIndex, 2,true,true);
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(currentBid, playerIndex, 1, true, true);
        if(moneyForBank)currentTileScript.UpgradeTownServerRpc(0, playerIndex);
        else currentTileScript.UpgradeTownServerRpc(-1, playerIndex);
        PlayerScript.LocalInstance.tilesThatPlayerOwnList.Add(currentTileScript);
    }

    public void ShowBuyingUI(int Cost, TileScript tileScript)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        currentTileScript = tileScript;
        currentCost = Cost;
        this.Cost.text = Cost + "PLN";
        this.TextLable.text = tileScript.name;
    }



    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        GameUIScript.OnNextPlayerTurn.Invoke();
    }

}

