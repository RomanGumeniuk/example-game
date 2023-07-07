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
            Debug.Log("dontBuy");
            BiddingTabUIScript.Instance.StartServerRpc(currentTileScript.townCostToBuy[0],currentTileScript.name,PlayerScript.LocalInstance.playerIndex);
            Hide();
        });

        BuyButton.onClick.AddListener(() =>
        {

            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(currentCost, PlayerScript.LocalInstance.playerIndex, 1);
            currentTileScript.UpgradeTownServerRpc(0, PlayerScript.LocalInstance.playerIndex);
            Hide();
            
        });
    }

    public void BuyTownForOtherPlayer(int playerIndex,int currentBid)
    {
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(currentBid, playerIndex, 1);
        currentTileScript.UpgradeTownServerRpc(0, playerIndex);
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

    internal void BuyTownForOtherPlayer(int value, NetworkVariable<int> currentBid)
    {
        throw new NotImplementedException();
    }
}

