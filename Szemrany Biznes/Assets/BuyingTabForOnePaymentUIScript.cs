using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            Hide();
        });

        BuyButton.onClick.AddListener(() =>
        {

            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(currentCost, PlayerScript.LocalInstance.playerIndex, 1);
            currentTileScript.UpgradeTownServerRpc(0, PlayerScript.LocalInstance.playerIndex);
            Hide();
            
        });
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

