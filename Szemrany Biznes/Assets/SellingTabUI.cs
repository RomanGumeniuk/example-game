using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellingTabUI : MonoBehaviour
{
    public static SellingTabUI Instance { get; private set; }

    public TextMeshProUGUI textLabel;
    public TextMeshProUGUI subTextLabel;
    public Button SellButton;
    public Button SellAtAuctionButton;
    public Button PayButton;

    public List<TileScript> selectedTiles = new List<TileScript>();

    public int currentAmountOfPlayerMoney;
    public int currentAmountMoneyToPay;

    private void Awake()
    {
        Instance = this;

        SellButton.onClick.AddListener(() =>
        {
            foreach(TileScript tileScript in PlayerScript.LocalInstance.tilesThatPlayerOwnList)
            {
                if (selectedTiles.Contains(tileScript))
                {
                    tileScript.SellingTownServerRpc(PlayerScript.LocalInstance.playerIndex);
                    PlayerScript.LocalInstance.tilesThatPlayerOwnList.Remove(tileScript);
                }
            }
            SellButton.interactable = false;
        });
        SellAtAuctionButton.onClick.AddListener(() =>
        {

        });
        PayButton.onClick.AddListener(() =>
        {

        });
    }

    public void UpdateSelectedTileList(TileScript tileScript,bool selected)
    {
        if (selected) selectedTiles.Add(tileScript);
        else selectedTiles.Remove(tileScript);
        int totalValueOfAllRealEstates = 0;
        foreach(TileScript tileScript1 in selectedTiles)
        {
            totalValueOfAllRealEstates += tileScript1.GetCurrentPropertyValue();
        }
        if (selectedTiles.Count>1)
        {
            subTextLabel.text = "For selling all of selected real estates you will get: " + totalValueOfAllRealEstates + "PLN";
            if(totalValueOfAllRealEstates+currentAmountOfPlayerMoney > currentAmountMoneyToPay) subTextLabel.text += "\nAfter selling you will have enough money to pay and you will be left with: "+ ((totalValueOfAllRealEstates + currentAmountOfPlayerMoney)-currentAmountMoneyToPay) + "PLN";
            else subTextLabel.text += "\nAfter selling you will not have enough money to pay, you need to make: " + (currentAmountMoneyToPay - (totalValueOfAllRealEstates + currentAmountOfPlayerMoney)) + "PLN more";
            SellButton.interactable = true;
            SellAtAuctionButton.interactable = false;
            return;
        }
        if(selectedTiles.Count==1)
        {
            subTextLabel.text = "For selling selected property you will get: " + totalValueOfAllRealEstates + "PLN";
            if (totalValueOfAllRealEstates + currentAmountOfPlayerMoney > currentAmountMoneyToPay) subTextLabel.text += "\nAfter selling you will have enough money to pay and you will be left with: " + ((totalValueOfAllRealEstates + currentAmountOfPlayerMoney) - currentAmountMoneyToPay) + "PLN";
            else subTextLabel.text += "\nAfter selling you will not have enough money to pay, you need to make: " + (currentAmountMoneyToPay - (totalValueOfAllRealEstates + currentAmountOfPlayerMoney)) + "PLN more";
            SellButton.interactable = true;
            SellAtAuctionButton.interactable = true;
            return;
        }
        subTextLabel.text = "Select all real estates that you want to sell"; 
        SellButton.interactable = false;
        SellAtAuctionButton.interactable = false;
    }


    public void Show(int amountToPay,int amountThatPlayerHas)
    {
        currentAmountMoneyToPay = amountToPay;
        currentAmountOfPlayerMoney = amountThatPlayerHas;
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        textLabel.text = "You need to pay: " + amountToPay + "PLN\nYou have to sell for at least: " + (amountToPay - amountThatPlayerHas) + "PLN";
        PayButton.interactable = false;
        SellButton.interactable = false;
        SellAtAuctionButton.interactable = false;
    }
}
