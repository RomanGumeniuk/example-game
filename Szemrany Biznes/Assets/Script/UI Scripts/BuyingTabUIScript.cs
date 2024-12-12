using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyingTabUIScript : MonoBehaviour
{

    public static BuyingTabUIScript Instance { get; private set; }
    public Button DontBuyButton;
    public Button BuyButton;

    public List<Toggle> LevelsOfToggle = new List<Toggle>();
    
    public List<Transform> Levels = new List<Transform>();

    public List<int> currentTownCostToBuy = new List<int>();

    public TileScript currentTileScript = null;
    public int currentTownLevel;
    public int currentMaxTownLevelThanCanBeBuy;
    public int currentAvailableTownUpgrade;

    private void Awake()
    {
        Instance = this;

        DontBuyButton.onClick.AddListener(() =>
        {
            Hide();
        });
        
        BuyButton.onClick.AddListener(() =>
        {
            int newCurrentTownLevel = GetNewCurrentTownLevel();
            int totalCost = GetTotalAmountOfMoneyToPayForTown();
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(totalCost, PlayerScript.LocalInstance.playerIndex, 1);
            currentTileScript.UpgradeTownServerRpc(newCurrentTownLevel, PlayerScript.LocalInstance.playerIndex);
            PlayerScript.LocalInstance.OnTownUpgrade();
            Hide();
        });
        
        LevelsOfToggle[0].onValueChanged.AddListener((state) =>
        {
            if(state) RefreshLevelsOnToggleOn(0);
            else RefreshLevelsOnToggleOff(0);
        });
        LevelsOfToggle[1].onValueChanged.AddListener((state) =>
        {
            if (state) RefreshLevelsOnToggleOn(1);
            else RefreshLevelsOnToggleOff(1);
        });
        LevelsOfToggle[2].onValueChanged.AddListener((state) =>
        {
            if (state) RefreshLevelsOnToggleOn(2);
            else RefreshLevelsOnToggleOff(2);
        });
        LevelsOfToggle[3].onValueChanged.AddListener((state) =>
        {
            if (state) RefreshLevelsOnToggleOn(3);
            else RefreshLevelsOnToggleOff(3);
        });
        LevelsOfToggle[4].onValueChanged.AddListener((state) =>
        {
            if (state) RefreshLevelsOnToggleOn(4);
            else RefreshLevelsOnToggleOff(4);
        });


    }
    
    public void RefreshLevelsOnToggleOn(int index)
    {
        for(int i=0;i<index;i++) LevelsOfToggle[i].isOn = true;
        RefreshBuyButtonText();
    }
    
    public void RefreshLevelsOnToggleOff(int index)
    {
        for (int i = index; i < 5; i++) if (LevelsOfToggle[i].interactable) LevelsOfToggle[i].isOn = false;
        RefreshBuyButtonText();
    }

    public void RefreshBuyButtonText()
    {
        for (int i = 0; i < 5; i++)
        {
            if (LevelsOfToggle[i].interactable)
            {
                LevelsOfToggle[i].isOn = true;
                break;
            }
        }
        int totalCost = GetTotalAmountOfMoneyToPayForTown();
        BuyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy for \n(" + totalCost + "PLN)";
    }
    
    
    public void ShowBuyingUI(int townLevel, int maxTownLevelThatCanBeBuy, List<int> townAllLevelsCost,TileScript tileScript,int currentAvailableTownUpgrade)
    {
        currentTownLevel = townLevel;
        currentMaxTownLevelThanCanBeBuy = maxTownLevelThatCanBeBuy;
        currentTileScript = tileScript;
        currentTownCostToBuy.Clear();
        foreach(int levelCost in townAllLevelsCost) currentTownCostToBuy.Add(levelCost);
        this.currentAvailableTownUpgrade = currentAvailableTownUpgrade;
        //Debug.Log($"maxTownLevel: {maxTownLevelThatCanBeBuy}, currentAvailable:{currentAvailableTownUpgrade}");
        for (int i = 0; i < 5; i++) Levels[i].GetComponentInChildren<TextMeshProUGUI>().text = currentTownCostToBuy[i].ToString() + "PLN";
        for(int i=0;i<5;i++)
        {
            Levels[i].GetComponent<RawImage>().color = Color.gray;
            LevelsOfToggle[i].isOn = false;
            LevelsOfToggle[i].interactable = false;
        }
        for(int i=0;i<currentTownLevel; i++) LevelsOfToggle[i].isOn = true;
        for(int i= Mathf.Max(currentTownLevel, currentAvailableTownUpgrade -2); i<Mathf.Min(currentMaxTownLevelThanCanBeBuy, currentAvailableTownUpgrade+1);i++)
        {
            Levels[i].GetComponent<RawImage>().color = Color.white;
            LevelsOfToggle[i].interactable = true;
        }
        RefreshBuyButtonText();
        foreach (Transform child in transform) child.gameObject.SetActive(true);
    }

    public void Hide()
    {
        foreach (Transform child in transform) child.gameObject.SetActive(false);
        GameUIScript.OnNextPlayerTurn.Invoke();
    }

    public int GetNewCurrentTownLevel()
    {
        int currentIndex = 0;
        for(int i=currentTownLevel;i<5;i++) if (LevelsOfToggle[i].isOn) currentIndex = i;
        return currentIndex+1;
    }

    public int GetTotalAmountOfMoneyToPayForTown()
    {
        int totalCost = 0;
        for (int i = currentTownLevel; i < Mathf.Min(currentMaxTownLevelThanCanBeBuy, currentAvailableTownUpgrade+1); i++)
        {
            if (LevelsOfToggle[i].isOn) totalCost += currentTownCostToBuy[i];
        }
        return totalCost;
    }

}