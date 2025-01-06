using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ChoosingPropertyTypeUI : MonoBehaviour
{
    public static ChoosingPropertyTypeUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    [SerializeField] private Button DontBuyButton;
    [SerializeField] private Button BuyButton;
    [SerializeField] private List<Toggle> AllOptions;
    [SerializeField] private List<float> CostMultiplier;
    public TileScript currentTileScript = null;

    private int selectedOption = -1;



    private void Start()
    {
        BuyButton.onClick.AddListener(() =>
        {
            int totalCost = GetCostPrice(selectedOption); //here will be some script that calculates cost
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(totalCost, PlayerScript.LocalInstance.playerIndex, 1);
            currentTileScript.SetPropertyTypeServerRpc((PropertyType)(selectedOption+1));
            currentTileScript.UpgradeTownServerRpc(1, PlayerScript.LocalInstance.playerIndex);
            Hide();
        });

        DontBuyButton.onClick.AddListener(() =>
        {
            Hide();
        });


        for(int i=0; i<AllOptions.Count;i++)
        {
            int index = i;
            AllOptions[i].onValueChanged.AddListener((bool value) =>
            {
                if(!value)
                {
                    for (int i = 0; i < AllOptions.Count; i++)
                    {
                        if (AllOptions[i].isOn) return;
                    }
                    BuyButton.interactable = false;
                    return;
                }
                BuyButton.interactable = true;
                for (int i=0;i<AllOptions.Count;i++)
                {
                    if (i == index) continue;
                    AllOptions[i].isOn = false;
                }
                selectedOption = index;
            });
        }
    }

    public void Hide()
    {
        foreach (Transform child in transform) child.gameObject.SetActive(false);
        GameUIScript.OnNextPlayerTurn.Invoke();
    }

    public void ShowChoosingUI(TileScript currentTileScript)
    {
        this.currentTileScript = currentTileScript;
        BuyButton.interactable = false;
        for(int i=0;i<AllOptions.Count;i++)
        {
            AllOptions[i].isOn = false;
            int cost = GetCostPrice(i);
            AllOptions[i].transform.parent.GetComponentInChildren<TextMeshProUGUI>().text = cost.ToString() + " PLN";
            if (PlayerScript.LocalInstance.amountOfMoney.Value < cost)
            {
                Debug.Log("ok " + AllOptions[i].transform.parent.position.x);
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
    }


    public int GetCostPrice(int i)
    {
        return Mathf.RoundToInt((currentTileScript.GetTownCostToBuyIndex(1,currentTileScript.ownerId.Value) * CostMultiplier[i])/10)*10;
    }

    public int GetLowestPrice(TileScript currentTileScript)
    {
        this.currentTileScript = currentTileScript;
        int index = 0;
        for(int i=1;i<CostMultiplier.Count;i++)
        { 
            if(CostMultiplier[i] < CostMultiplier[index]) index = i;
        }
        return GetCostPrice(index);
    }
}
