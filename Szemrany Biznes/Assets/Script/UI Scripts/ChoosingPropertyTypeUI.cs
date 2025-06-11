using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoosingPropertyTypeUI : MonoBehaviour, IQueueWindows
{
    public static ChoosingPropertyTypeUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    [SerializeField] private Button DontBuyButton;
    [SerializeField] private Button BuyButton;
    [SerializeField] private Toggle FirstOptionToBuy;
    [SerializeField] private TextMeshProUGUI titleText;
    
    [SerializeField] private List<Toggle> AllOptions;
    [SerializeField] private List<float> CostMultiplier;
    public TileScript currentTileScript = null;

    [SerializeField] private int selectedOption = -1;



    private void Start()
    {
        BuyButton.onClick.AddListener(() =>
        {
            int baseCost = currentTileScript.GetTownCostToBuyIndex(0, currentTileScript.ownerId.Value);
            if (currentTileScript.ownerId.Value != PlayerScript.LocalInstance.playerIndex) PlayerScript.LocalInstance.AddTilesThatPlayerOwnListServerRpc(currentTileScript.index);
            if (selectedOption==-1)
            {
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(baseCost, PlayerScript.LocalInstance.playerIndex, 1);
                currentTileScript.UpgradeTownServerRpc(0, PlayerScript.LocalInstance.playerIndex);
                Hide();
                return;
            }
            
            int totalCost = GetCostPrice(selectedOption) + ((FirstOptionToBuy.transform.parent.gameObject.activeSelf)?baseCost:0); //here will be some script that calculates cost
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(totalCost, PlayerScript.LocalInstance.playerIndex, 1);
            currentTileScript.SetPropertyTypeServerRpc((PropertyType)(selectedOption+1));
            currentTileScript.UpgradeTownServerRpc(1, PlayerScript.LocalInstance.playerIndex);
            Hide();
        });

        DontBuyButton.onClick.AddListener(() =>
        {
            Hide();
        });

        FirstOptionToBuy.onValueChanged.AddListener((bool value) =>
        {
            if (!value)
            {
                BuyButton.interactable = false;
                BuyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Kup";
                for (int i = 0; i < AllOptions.Count; i++)
                {
                    AllOptions[i].isOn = false;
                }
                selectedOption = -1;
                return;
            }
            BuyButton.interactable = true;
            BuyButton.GetComponentInChildren<TextMeshProUGUI>().text =  $"Kup ({currentTileScript.GetTownCostToBuyIndex(0, currentTileScript.ownerId.Value).ToString()} PLN)";
        });


        for(int i=0; i<AllOptions.Count;i++)
        {
            int index = i;
            AllOptions[i].onValueChanged.AddListener((bool value) =>
            {
                if(!value)
                {
                    if (!FirstOptionToBuy.isOn) return;
                    for (int i = 0; i < AllOptions.Count; i++)
                    {
                        if (AllOptions[i].isOn) return;
                    }
                    if(FirstOptionToBuy.transform.parent.gameObject.activeSelf)BuyButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Kup ({currentTileScript.GetTownCostToBuyIndex(0, currentTileScript.ownerId.Value).ToString()} PLN)";
                    else BuyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Kup";
                    selectedOption = -1;
                    return;
                }
                FirstOptionToBuy.isOn = true;
                BuyButton.interactable = true;
                for (int i=0;i<AllOptions.Count;i++)
                {
                    if (i == index) continue;
                    AllOptions[i].isOn = false;
                }
                selectedOption = index;
                if (FirstOptionToBuy.transform.parent.gameObject.activeSelf) BuyButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Kup ({currentTileScript.GetTownCostToBuyIndex(0, currentTileScript.ownerId.Value) + GetCostPrice(selectedOption)} PLN)";
                else BuyButton.GetComponentInChildren<TextMeshProUGUI>().text = BuyButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Kup ({GetCostPrice(selectedOption)} PLN)";

            });
        }

        
    }

    public void Hide()
    {
        foreach (Transform child in transform) child.gameObject.SetActive(false);
        GameUIScript.OnNextPlayerTurn.Invoke();
        PlayerScript.LocalInstance.GoToNextAction();
    }
    void ShowChoosingUI()
    {
        BuyButton.interactable = false;
        FirstOptionToBuy.transform.parent.GetComponentInChildren<TextMeshProUGUI>().text = currentTileScript.GetTownCostToBuyIndex(0, currentTileScript.ownerId.Value).ToString() + " PLN";
        FirstOptionToBuy.isOn = false;
        for (int i = 0; i < AllOptions.Count; i++)
        {
            AllOptions[i].isOn = false;
            int cost = GetCostPrice(i);
            int baseCost = (currentTileScript.ownerId.Value == PlayerScript.LocalInstance.playerIndex) ? 0 : currentTileScript.GetTownCostToBuyIndex(0, currentTileScript.ownerId.Value);
            AllOptions[i].transform.parent.GetComponentInChildren<TextMeshProUGUI>().text = cost.ToString() + " PLN";
            if (PlayerScript.LocalInstance.amountOfMoney.Value < cost+ baseCost)
            {
                AllOptions[i].transform.parent.GetComponent<RawImage>().color = new Color32(100, 100, 100, 255);
                AllOptions[i].interactable = false;
            }
            else
            {
                AllOptions[i].transform.parent.GetComponent<RawImage>().color = Color.white;
                AllOptions[i].interactable = true;
            }
        }
        foreach (Transform child in transform) child.gameObject.SetActive(true);
        if (currentTileScript.ownerId.Value == PlayerScript.LocalInstance.playerIndex)
        {
            FirstOptionToBuy.transform.parent.gameObject.SetActive(false);
            titleText.text = "Wybierz typ dzia³ki";
        }
        else titleText.text = "Kup dzia³kê oraz wybierz jej typ";
    }


    public void ShowChoosingUI(TileScript currentTileScript)
    {
        this.currentTileScript = currentTileScript;
        PlayerScript.LocalInstance.AddToQueueOfWindows(this);
    }


    public int GetCostPrice(int i)
    {
        return Mathf.RoundToInt((currentTileScript.GetTownCostToBuyIndex(1,currentTileScript.ownerId.Value) * CostMultiplier[i])/10)*10;
    }

    public int GetLowestPrice(TileScript currentTileScript)
    {
        this.currentTileScript = currentTileScript;
        if (currentTileScript.ownerId.Value == PlayerScript.LocalInstance.playerIndex)
        {
            int index = 0;
            for (int i = 1; i < CostMultiplier.Count; i++)
            {
                if (CostMultiplier[i] < CostMultiplier[index]) index = i;
            }
            return GetCostPrice(index);
        }
        return currentTileScript.GetTownCostToBuyIndex(0, currentTileScript.ownerId.Value);


    }

    public void ResumeAction()
    {
        ShowChoosingUI();
    }
}
