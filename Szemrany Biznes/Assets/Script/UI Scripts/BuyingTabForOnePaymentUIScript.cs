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
    public TextMeshProUGUI TitleScreen;

    public int currentCost=0;

    public TileScript currentTileScript = null;

    bool isBuyingFromOtherPlayer=false;

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
            if(isBuyingFromOtherPlayer)
            {
                currentTileScript.TownBuyoutServerRpc(PlayerScript.LocalInstance.playerIndex,currentCost);
                Hide();
                return;
            }
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(currentCost, PlayerScript.LocalInstance.playerIndex, 1);
            currentTileScript.UpgradeTownServerRpc(0, PlayerScript.LocalInstance.playerIndex);
            PlayerScript.LocalInstance.AddTilesThatPlayerOwnListServerRpc(currentTileScript.index);
            Hide();
            
        });
    }

    public void BuyTownForOtherPlayer(int playerIndex,int currentBid,int startPrice,bool moneyForBank)
    {
        //GameLogic.Instance.UpdateMoneyForPlayerServerRpc(startPrice, playerIndex, 2,true,true);
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(currentBid, playerIndex, 1, true, true);
        if(moneyForBank)currentTileScript.UpgradeTownServerRpc(-1, playerIndex);
        else currentTileScript.UpgradeTownServerRpc(currentTileScript.townLevel.Value, playerIndex);
        PlayerScript.LocalInstance.AddTilesThatPlayerOwnListServerRpc(currentTileScript.index);
    }

    public void ShowBuyingUI(int Cost, TileScript tileScript,string titleScreen = "Do you want to buy this?",bool isBuyingFromOtherPlayer = false)
    {
        this.isBuyingFromOtherPlayer = isBuyingFromOtherPlayer;
        TitleScreen.text = titleScreen;
        currentTileScript = tileScript;
        currentCost = Cost;
        this.Cost.text = Cost + "PLN";
        this.TextLable.text = tileScript.name;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
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

